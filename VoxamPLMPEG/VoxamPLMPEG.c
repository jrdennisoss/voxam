/*
 *  Copyright (C) 2022 Jon Dennis
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */



#include <Windows.h>
#include <stdlib.h>

#pragma warning(disable : 4996)
#define PL_MPEG_IMPLEMENTATION
#include "reelmagic_pl_mpeg.h"


BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}



#include "VoxamPLMPEG.h"



static void vpm_vesdec_set_mem_buffer(vpm_vesdec_handle vesdec, const void* const buf, unsigned off, unsigned len);
static int vpm_vesdec_compute_result_size_for_picformat(vpm_vesdec_handle vesdec, const int picformat);
static plm_frame_t* vpm_vesdec_get_frame_buffer_ptr(vpm_vesdec_handle vesdec, const int picbuf);

struct vpm_vesdec {
    plm_buffer_t plm_buffer;
    plm_video_t plm_video;
    plm_frame_t* last_decoded_frame;

    int force_has_reference_frame;
};

VEXPORT vpm_vesdec_handle
vpm_vesdec_alloc(void) {
    vpm_vesdec_handle rv;

    rv = calloc(1, sizeof(*rv));
    vpm_vesdec_reset(rv);

    return rv;
}

VEXPORT void
vpm_vesdec_free(vpm_vesdec_handle vesdec) {
    if (!vesdec) return;
    vpm_vesdec_reset(vesdec);
    free(vesdec);
}

VEXPORT void
vpm_vesdec_reset(vpm_vesdec_handle vesdec) {
    if (!vesdec) return;
    free(vesdec->plm_video.frames_data);
    memset(&vesdec->plm_video, 0, sizeof(vesdec->plm_video));
    vpm_vesdec_set_mem_buffer(vesdec, NULL, 0, 0);
    vesdec->plm_video.buffer = &vesdec->plm_buffer;
    vesdec->plm_video.start_code = -1;
    vesdec->plm_video.decode_tag = -1;
    vesdec->plm_video.frame_current.decode_tag = -1;
    vesdec->plm_video.frame_forward.decode_tag = -1;
    vesdec->plm_video.frame_backward.decode_tag = -1;

    vesdec->last_decoded_frame = NULL;

    vesdec->force_has_reference_frame = -1;
}

VEXPORT int
vpm_vesdec_initialize_seqh(vpm_vesdec_handle vesdec, const void* const seqhdrbuf, const unsigned seqhdrbufoff, unsigned seqhdrbuflen) {

    int rv;
    uint8_t buf[142]; //hacky way to work around issue in plm_video_decode_sequence_header() which requires at least 138 bytes in the buffer (+ 4 bytes of startcode)...

    rv = -1;
    if (!vesdec) return rv;
    if (seqhdrbuf == NULL) return rv;
    if (seqhdrbuflen < 12) return rv;

    if (seqhdrbuflen > sizeof(buf)) seqhdrbuflen = sizeof(buf);
    memcpy(buf, ((uint8_t*)seqhdrbuf) + seqhdrbufoff, seqhdrbuflen);

    vpm_vesdec_reset(vesdec);
    vpm_vesdec_set_mem_buffer(vesdec, buf, 0, sizeof(buf));
    if (plm_video_has_header(&vesdec->plm_video)) rv = 0;
    vpm_vesdec_set_mem_buffer(vesdec, NULL, 0, 0);

    return rv;
}

VEXPORT int
vpm_vesdec_initialize_manual(vpm_vesdec_handle vesdec, const struct vpm_vesdec_manual_init_params* const params) {
    //
    //this function is an alternate implementation of "plm_video_decode_sequence_header()"
    //

    int i;
    plm_video_t* self; //shortcut to make c/p from the original function easier...

    if (!vesdec) return -1;
    if (params == NULL) return -1;

    self = &vesdec->plm_video;

    vpm_vesdec_reset(vesdec);

    self->width = params->width  & 0x0FFF;
    self->height = params->height & 0x0FFF;

    if (self->width <= 0 || self->height <= 0) {
        return -1;
    }

    self->framerate = 1.00;

    // Load custom intra quant matrix?
    if (!params->use_standard_intra_quant_matrix) {
        for (i = 0; i < 64; i++)
            self->intra_quant_matrix[PLM_VIDEO_ZIG_ZAG[i]]
                = params->intra_quant_matrix[i];
    }
    else {
        memcpy(self->intra_quant_matrix, PLM_VIDEO_INTRA_QUANT_MATRIX, 64);
    }

    // Load custom non intra quant matrix?
    if (!params->use_standard_non_intra_quant_matrix) {
        for (i = 0; i < 64; i++)
            self->non_intra_quant_matrix[PLM_VIDEO_ZIG_ZAG[i]]
                = params->non_intra_quant_matrix[i];
    }
    else {
        memcpy(self->non_intra_quant_matrix, PLM_VIDEO_NON_INTRA_QUANT_MATRIX, 64);
    }

    self->mb_width = (self->width + 15) >> 4;
    self->mb_height = (self->height + 15) >> 4;
    self->mb_size = self->mb_width * self->mb_height;

    self->luma_width = self->mb_width << 4;
    self->luma_height = self->mb_height << 4;

    self->chroma_width = self->mb_width << 3;
    self->chroma_height = self->mb_height << 3;


    // Allocate one big chunk of data for all 3 frames = 9 planes
    size_t luma_plane_size = self->luma_width * self->luma_height;
    size_t chroma_plane_size = self->chroma_width * self->chroma_height;
    size_t frame_data_size = (luma_plane_size + 2 * chroma_plane_size);

    self->frames_data = (uint8_t*)malloc(frame_data_size * 3);
    plm_video_init_frame(self, &self->frame_current, self->frames_data + frame_data_size * 0);
    plm_video_init_frame(self, &self->frame_forward, self->frames_data + frame_data_size * 1);
    plm_video_init_frame(self, &self->frame_backward, self->frames_data + frame_data_size * 2);

    self->has_sequence_header = TRUE;

    return 0; //success
}

VEXPORT unsigned
vpm_vesdec_get_width(vpm_vesdec_handle vesdec) {
    if (!vesdec) return 0;
    return vesdec->plm_video.width;
}

VEXPORT unsigned
vpm_vesdec_get_height(vpm_vesdec_handle vesdec) {
    if (!vesdec) return 0;
    return vesdec->plm_video.height;
}

VEXPORT unsigned
vpm_vesdec_get_mb_width(vpm_vesdec_handle vesdec) {
    if (!vesdec) return 0;
    return vesdec->plm_video.mb_width;
}

VEXPORT unsigned
vpm_vesdec_get_mb_height(vpm_vesdec_handle vesdec) {
    if (!vesdec) return 0;
    return vesdec->plm_video.mb_height;
}

VEXPORT unsigned
vpm_vesdec_get_frames_decoded(vpm_vesdec_handle vesdec) {
    if (!vesdec) return 0;
    return (unsigned)vesdec->plm_video.frames_decoded;
}

VEXPORT int
vpm_vesdec_get_decoder_feed_parameter(vpm_vesdec_handle vesdec, const int feed_param_type) {
    if (vesdec == NULL) return -1;
    switch (feed_param_type)
    {
    case VPM_VESDEC_FEEDPARAM_DECODE_TAG: return vesdec->plm_video.decode_tag;
    case VPM_VESDEC_FEEDPARAM_FORCE_HAS_REFERENCE_FRAME: return vesdec->force_has_reference_frame;
    }
    return -1;
}

VEXPORT int
vpm_vesdec_set_decoder_feed_parameter(vpm_vesdec_handle vesdec, const int feed_param_type, const int value) {
    if (vesdec == NULL) return -1;
    switch (feed_param_type)
    {
    case VPM_VESDEC_FEEDPARAM_DECODE_TAG: return vesdec->plm_video.decode_tag = value;
    case VPM_VESDEC_FEEDPARAM_FORCE_HAS_REFERENCE_FRAME: return vesdec->force_has_reference_frame = value;
    }
    return -1;
}

VEXPORT int
vpm_vesdec_feed_decoder(vpm_vesdec_handle vesdec, int max_iterations, const void* const buf, const unsigned off, const unsigned len) {
    int rv;
    
    if (vesdec == NULL) return -1;
    if (buf == NULL) return -1;
    if (len < 1) return -1;

    rv = 0;
    vesdec->last_decoded_frame = NULL;
    vesdec->plm_video.start_code = -1;

    vpm_vesdec_set_mem_buffer(vesdec, buf, off, len);
    while (max_iterations && !plm_buffer_has_ended(&vesdec->plm_buffer)) {
        if (max_iterations > 0) --max_iterations;
        if (vesdec->force_has_reference_frame >= 0)
            vesdec->plm_video.has_reference_frame = vesdec->force_has_reference_frame ? TRUE : FALSE;
        vesdec->last_decoded_frame = plm_video_decode(&vesdec->plm_video);
        if (vesdec->last_decoded_frame == NULL) break;
        ++rv;
    }
    vpm_vesdec_set_mem_buffer(vesdec, NULL, 0, 0);

    return rv;
}

VEXPORT int
vpm_vesdec_collect_picture_buffer(vpm_vesdec_handle vesdec, const int picformat, const int picbuf, void* const buf, const unsigned off, const unsigned len) {
    int result_size;
    plm_frame_t* frame;
    
    if (vesdec == NULL) return -1;
    if (!vesdec->plm_video.has_sequence_header) return -1;
    if (buf == NULL) return -1;
    if (len < 1) return -1;

    result_size = vpm_vesdec_compute_result_size_for_picformat(vesdec, picformat);
    if (result_size < 1) return -1;

    frame = vpm_vesdec_get_frame_buffer_ptr(vesdec, picbuf);
    if (frame == NULL) return -1;

    switch (picformat) {
    case VPM_VESDEC_PICFORMAT_YUV:
        return -1;
        break;
    case VPM_VESDEC_PICFORMAT_RGB:
        plm_frame_to_rgb(frame, ((uint8_t*)buf) + off, vesdec->plm_video.width * 3);
        break;
    case VPM_VESDEC_PICFORMAT_BGR:
        plm_frame_to_bgr(frame, ((uint8_t*)buf) + off, vesdec->plm_video.width * 3);
        break;
    default:
        return -1;
    }

    return result_size;
}

VEXPORT int
vpm_vesdec_get_last_feed_picture_buffer_type(vpm_vesdec_handle vesdec) {
    if (vesdec == NULL) return -1;
    if (vesdec->last_decoded_frame == &vesdec->plm_video.frame_current)   return VPM_VESDEC_PICBUF_CURRENT;
    if (vesdec->last_decoded_frame == &vesdec->plm_video.frame_forward)   return VPM_VESDEC_PICBUF_FORWARD;
    if (vesdec->last_decoded_frame == &vesdec->plm_video.frame_backward)  return VPM_VESDEC_PICBUF_BACKWARD;
    return -1;
}

VEXPORT unsigned
vpm_vesdec_get_picture_buffer_data_size(vpm_vesdec_handle vesdec) {
    size_t luma_plane_size;
    size_t chroma_plane_size;

    if (vesdec == NULL) return 0;

    luma_plane_size = vesdec->plm_video.luma_width * vesdec->plm_video.luma_height;
    chroma_plane_size = vesdec->plm_video.chroma_width * vesdec->plm_video.chroma_height;

    return (int)(luma_plane_size + 2 * chroma_plane_size);
}

VEXPORT int
vpm_vesdec_get_picture_buffer_decode_tag(vpm_vesdec_handle vesdec, const int picbuf) {
    plm_frame_t* frame;

    if (vesdec == NULL) return -1;

    frame = vpm_vesdec_get_frame_buffer_ptr(vesdec, picbuf);
    if (frame == NULL) return -1;
    return frame->decode_tag;
}


VEXPORT int
vpm_vesdec_memcpy_in_picture_buffer_data(vpm_vesdec_handle vesdec, const int picbuf, const void* const buf, const unsigned off, const unsigned len) {
    unsigned picbuf_size;
    plm_frame_t* frame;

    if (vesdec == NULL) return -1;
    if (buf == NULL) return -1;

    picbuf_size = vpm_vesdec_get_picture_buffer_data_size(vesdec);
    if (len < picbuf_size) return -1;
    if (picbuf_size < 1) return -1;

    frame = vpm_vesdec_get_frame_buffer_ptr(vesdec, picbuf);
    if (frame == NULL) return -1;
    if (frame->y.data == NULL) return -1; // y.data is the buffer base...

    memcpy(frame->y.data, ((uint8_t*)buf) + off, picbuf_size);
    return (int)picbuf_size;
}

VEXPORT int
vpm_vesdec_memcpy_out_picture_buffer_data(vpm_vesdec_handle vesdec, const int picbuf, void* const buf, const unsigned off, const unsigned len) {
    unsigned picbuf_size;
    plm_frame_t* frame;

    if (vesdec == NULL) return -1;
    if (buf == NULL) return -1;

    picbuf_size = vpm_vesdec_get_picture_buffer_data_size(vesdec);
    if (len < picbuf_size) return -1;
    if (picbuf_size < 1) return -1;

    frame = vpm_vesdec_get_frame_buffer_ptr(vesdec, picbuf);
    if (frame == NULL) return -1;
    if (frame->y.data == NULL) return -1; // y.data is the buffer base...

    memcpy(((uint8_t*)buf) + off, frame->y.data, picbuf_size);
    return (int)picbuf_size;
}





static int
vpm_vesdec_compute_result_size_for_picformat(vpm_vesdec_handle vesdec, const int picformat) {
    switch (picformat) {
    case VPM_VESDEC_PICFORMAT_RGB:
    case VPM_VESDEC_PICFORMAT_BGR:
        return vesdec->plm_video.width * vesdec->plm_video.height * 3;
    }
    return 0;
}

static plm_frame_t *
vpm_vesdec_get_frame_buffer_ptr(vpm_vesdec_handle vesdec, const int picbuf) {
    switch (picbuf) {
    case VPM_VESDEC_PICBUF_LASTFEED: return vesdec->last_decoded_frame;
    case VPM_VESDEC_PICBUF_CURRENT:  return &vesdec->plm_video.frame_current;
    case VPM_VESDEC_PICBUF_BACKWARD: return &vesdec->plm_video.frame_backward;
    case VPM_VESDEC_PICBUF_FORWARD:  return &vesdec->plm_video.frame_forward;
    }
    return NULL;
}








static void
vpm_vesdec_set_mem_buffer(vpm_vesdec_handle vesdec, const void * const buf, unsigned off, unsigned len) {
    memset(&vesdec->plm_buffer, 0, sizeof(vesdec->plm_buffer));
    vesdec->plm_buffer.capacity = len;
    vesdec->plm_buffer.length = len;
    vesdec->plm_buffer.total_size = len;
    vesdec->plm_buffer.bytes = (uint8_t*)buf;
    vesdec->plm_buffer.bytes += off;
    vesdec->plm_buffer.mode = PLM_BUFFER_MODE_FIXED_MEM;
}
