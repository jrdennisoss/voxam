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


#ifndef VOXAMPLMPEG_H_INCLUDED
#define VOXAMPLMPEG_H_INCLUDED


#define VEXPORT __declspec(dllexport)

//VES decoder functions
typedef struct vpm_vesdec* vpm_vesdec_handle;
VEXPORT vpm_vesdec_handle vpm_vesdec_alloc(void);
VEXPORT void vpm_vesdec_free(vpm_vesdec_handle vesdec);
VEXPORT void vpm_vesdec_reset(vpm_vesdec_handle vesdec);
VEXPORT int vpm_vesdec_initialize_seqh(vpm_vesdec_handle vesdec, const void* const seqhdrbuf, const unsigned seqhdrbufoff, unsigned seqhdrbuflen);
struct vpm_vesdec_manual_init_params {
	unsigned width;
	unsigned height;
	unsigned use_standard_intra_quant_matrix;
	unsigned use_standard_non_intra_quant_matrix;
	unsigned char intra_quant_matrix[64];
	unsigned char non_intra_quant_matrix[64];
};
VEXPORT int vpm_vesdec_initialize_manual(vpm_vesdec_handle vesdec, const struct vpm_vesdec_manual_init_params* const params);
VEXPORT unsigned vpm_vesdec_get_width(vpm_vesdec_handle vesdec);
VEXPORT unsigned vpm_vesdec_get_height(vpm_vesdec_handle vesdec);
VEXPORT unsigned vpm_vesdec_get_mb_width(vpm_vesdec_handle vesdec);
VEXPORT unsigned vpm_vesdec_get_mb_height(vpm_vesdec_handle vesdec);
VEXPORT unsigned vpm_vesdec_get_frames_decoded(vpm_vesdec_handle vesdec);


#define VPM_VESDEC_FEEDPARAM_DECODE_TAG 0
#define VPM_VESDEC_FEEDPARAM_FORCE_HAS_REFERENCE_FRAME 1
VEXPORT int vpm_vesdec_get_decoder_feed_parameter(vpm_vesdec_handle vesdec, const int feed_param_type);
VEXPORT int vpm_vesdec_set_decoder_feed_parameter(vpm_vesdec_handle vesdec, const int feed_param_type, const int value);
VEXPORT int vpm_vesdec_feed_decoder(vpm_vesdec_handle vesdec, int max_iterations, const void* const buf, const unsigned off, const unsigned len);


#define VPM_VESDEC_PICFORMAT_YUV 0
#define VPM_VESDEC_PICFORMAT_RGB 1
#define VPM_VESDEC_PICFORMAT_BGR 2

#define VPM_VESDEC_PICBUF_LASTFEED  0
#define VPM_VESDEC_PICBUF_CURRENT   1
#define VPM_VESDEC_PICBUF_FORWARD   2
#define VPM_VESDEC_PICBUF_BACKWARD  3
VEXPORT int vpm_vesdec_collect_picture_buffer(vpm_vesdec_handle vesdec, const int picformat, const int picbuf, void* const buf, const unsigned off, const unsigned len);
VEXPORT int vpm_vesdec_get_last_feed_picture_buffer_type(vpm_vesdec_handle vesdec);

VEXPORT unsigned vpm_vesdec_get_picture_buffer_data_size(vpm_vesdec_handle vesdec);
VEXPORT int vpm_vesdec_memcpy_in_picture_buffer_data(vpm_vesdec_handle vesdec, const int picbuf, const void* const buf, const unsigned off, const unsigned len);
VEXPORT int vpm_vesdec_memcpy_out_picture_buffer_data(vpm_vesdec_handle vesdec, const int picbuf, void* const buf, const unsigned off, const unsigned len);


#endif // #ifndef VOXAMPLMPEG_H_INCLUDED