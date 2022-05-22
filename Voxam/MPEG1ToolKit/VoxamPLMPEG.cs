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



using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using Voxam.MPEG1ToolKit.Objects;
using Voxam.MPEG1ToolKit.Threading;


namespace Voxam.MPEG1ToolKit
{
    public class PLMPEG
    {
        internal class DLL
        {
            private const string DLL_FILENAME = "VoxamPLMPEG.dll";


            public const int VPM_VESDEC_PICFORMAT_YUV = 0;
            public const int VPM_VESDEC_PICFORMAT_RGB = 1;
            public const int VPM_VESDEC_PICFORMAT_BGR = 2;

            public const int VPM_VESDEC_PICBUF_LASTFEED = 0;
            public const int VPM_VESDEC_PICBUF_CURRENT = 1;
            public const int VPM_VESDEC_PICBUF_FORWARD = 2;
            public const int VPM_VESDEC_PICBUF_BACKWARD = 3;

            public static string Version => FileVersionInfo.GetVersionInfo(DLL_FILENAME).FileVersion;


            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static IntPtr vpm_vesdec_alloc();

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static void vpm_vesdec_free(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static void vpm_vesdec_reset(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_initialize_seqh(IntPtr vesdec, byte[] seqhdrbuf, uint seqhdrbufoff, uint seqhdrbuflen);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static uint vpm_vesdec_get_width(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static uint vpm_vesdec_get_height(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static uint vpm_vesdec_get_mb_width(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static uint vpm_vesdec_get_mb_height(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static uint vpm_vesdec_get_frames_decoded(IntPtr vesdec);




            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_feed_decoder(IntPtr vesdec, int force_has_reference_frame, int max_iterations,
                byte[] buf, uint off, uint len);
            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_feed_decoder(IntPtr vesdec, int force_has_reference_frame, int max_iterations,
                IntPtr buf, uint off, uint len);
            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_collect_picture_buffer(IntPtr vesdec, int picformat, int picbuf,
                byte[] buf, uint off, uint len);
            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_collect_picture_buffer(IntPtr vesdec, int picformat, int picbuf,
                IntPtr buf, uint off, uint len);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_get_last_feed_picture_buffer_type(IntPtr vesdec);

            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static uint vpm_vesdec_get_picture_buffer_data_size(IntPtr vesdec);
            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_memcpy_in_picture_buffer_data(IntPtr vesdec, int picbuf,
                byte[] buf, uint off, uint len);
            [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
            internal extern static int vpm_vesdec_memcpy_out_picture_buffer_data(IntPtr vesdec, int picbuf,
                byte[] buf, uint off, uint len);
        }

        public class VESDec : IDisposable
        {
            private readonly IntPtr _vesdec;

            public VESDec()
            {
                _vesdec = DLL.vpm_vesdec_alloc();
                if (_vesdec == null) throw new Exception("vpm_vesdec_alloc() failed");
            }

            public void Reset()
            {
                DLL.vpm_vesdec_reset(_vesdec);
            }

            public bool InitializeWithSequenceHeader(byte[] buf, int off, int len)
            {
                return DLL.vpm_vesdec_initialize_seqh(_vesdec, buf, (uint)off, (uint)len) == 0;
            }

            public int Width { get { return (int)DLL.vpm_vesdec_get_width(_vesdec); } }
            public int Height { get { return (int)DLL.vpm_vesdec_get_height(_vesdec); } }

            public int MacroblockWidth { get { return (int)DLL.vpm_vesdec_get_mb_width(_vesdec); } }
            public int MacroblockHeight { get { return (int)DLL.vpm_vesdec_get_mb_height(_vesdec); } }

            public int FramesDecoded { get { return (int)DLL.vpm_vesdec_get_frames_decoded(_vesdec); } }

            public int FeedDecoder(byte[] buf, int off, int len, int max_iterations = -1, int force_has_reference_frame = -1)
            {
                if (off < 0) return -1;
                if (len < 0) return -1;

                return DLL.vpm_vesdec_feed_decoder(_vesdec, force_has_reference_frame, max_iterations,
                    buf, (uint)off, (uint)len);
            }

            public enum PictureFormat
            {
                YUV,
                RGB,
                BGR,
            }
            private static int toDllEnum(PictureFormat pictureFormat)
            {
                switch (pictureFormat)
                {
                    case PictureFormat.YUV: return DLL.VPM_VESDEC_PICFORMAT_YUV;
                    case PictureFormat.RGB: return DLL.VPM_VESDEC_PICFORMAT_RGB;
                    case PictureFormat.BGR: return DLL.VPM_VESDEC_PICFORMAT_BGR;
                }
                return -1;
            }

            public enum DecoderPictureBuffer
            {
                Invalid,
                LastFeed,
                Current,
                Forward,
                Backward,
            }
            private static int toDllEnum(DecoderPictureBuffer pictureBuffer)
            {
                switch (pictureBuffer)
                {
                    case DecoderPictureBuffer.LastFeed: return DLL.VPM_VESDEC_PICBUF_LASTFEED;
                    case DecoderPictureBuffer.Current:  return DLL.VPM_VESDEC_PICBUF_CURRENT;
                    case DecoderPictureBuffer.Forward:  return DLL.VPM_VESDEC_PICBUF_FORWARD;
                    case DecoderPictureBuffer.Backward: return DLL.VPM_VESDEC_PICBUF_BACKWARD;
                }
                return -1;
            }

            public int CollectPictureBuffer(byte[] outbuf, int outbufOff, int outbufLen, PictureFormat pictureFormat, DecoderPictureBuffer pictureBuffer)
            {
                if (outbufOff < 1) return -1;
                if (outbufLen < 1) return -1;

                return DLL.vpm_vesdec_collect_picture_buffer(_vesdec, toDllEnum(pictureFormat), toDllEnum(pictureBuffer),
                    outbuf, (uint)outbufOff, (uint)outbufLen);
            }

            public bool CollectPictureBuffer(Bitmap output, DecoderPictureBuffer pictureBuffer)
            {
                if (output == null) return false;
                if (output.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb) return false;

                var bmpbuf = output.LockBits(new Rectangle(new Point(), output.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int outputSize = output.Width * output.Height * 3;

                int bytesCollected = DLL.vpm_vesdec_collect_picture_buffer(_vesdec,
                    DLL.VPM_VESDEC_PICFORMAT_BGR, toDllEnum(pictureBuffer),
                    bmpbuf.Scan0, 0, (uint)outputSize);

                output.UnlockBits(bmpbuf);
                return bytesCollected == outputSize;
            }

            public DecoderPictureBuffer LastFeedPictureBufferType
            {
                get
                {
                    switch (DLL.vpm_vesdec_get_last_feed_picture_buffer_type(_vesdec))
                    {
                        case DLL.VPM_VESDEC_PICBUF_LASTFEED: return DecoderPictureBuffer.LastFeed;
                        case DLL.VPM_VESDEC_PICBUF_CURRENT:  return DecoderPictureBuffer.Current;
                        case DLL.VPM_VESDEC_PICBUF_BACKWARD: return DecoderPictureBuffer.Backward;
                        case DLL.VPM_VESDEC_PICBUF_FORWARD:  return DecoderPictureBuffer.Forward;
                    }
                    return DecoderPictureBuffer.Invalid;
                }
            }

            public int PictureBufferDataSize { get { return (int)DLL.vpm_vesdec_get_picture_buffer_data_size(_vesdec); } }

            public int CopyInPictureBufferData(DecoderPictureBuffer pictureBuffer, byte[] buf, int off, int len)
            {
                return DLL.vpm_vesdec_memcpy_in_picture_buffer_data(_vesdec, toDllEnum(pictureBuffer), buf, (uint)off, (uint)len);
            }
            public int CopyOutPictureBufferData(DecoderPictureBuffer pictureBuffer, byte[] buf, int off, int len)
            {
                return DLL.vpm_vesdec_memcpy_out_picture_buffer_data(_vesdec, toDllEnum(pictureBuffer), buf, (uint)off, (uint)len);
            }

            public int DecodeSinglePicture(
                byte[] outbuf, int outbufOff, int outbufLen,
                PictureFormat pictureFormat,
                byte[] inbuf, int inbufOff, int inbufLen,
                int forceHasReferenceFrame = -1)
            {
                if (FeedDecoder(inbuf, inbufOff, inbufLen, 1, forceHasReferenceFrame) != 1)
                    return -1;
                return CollectPictureBuffer(outbuf, outbufOff, outbufLen, pictureFormat, DecoderPictureBuffer.LastFeed);
            }

            public int DecodeSinglePicture(
                 byte[] outbuf, int outbufOff, int outbufLen,
                 PictureFormat pictureFormat,
                 byte[] inbuf, int inbufOff, int inbufLen,
                 bool forceHasReferenceFrame)
            {
                return DecodeSinglePicture(outbuf, outbufOff, outbufLen,
                    pictureFormat,
                    inbuf, inbufOff, inbufLen,
                    forceHasReferenceFrame ? 1 : 0);
            }

            public bool DecodeSinglePicture(
                Bitmap output,
                byte[] inbuf, int inbufOff, int inbufLen,
                int forceHasReferenceFrame = -1)
            {
                if (FeedDecoder(inbuf, inbufOff, inbufLen, 1, forceHasReferenceFrame) != 1)
                    return false;

                return CollectPictureBuffer(output, DecoderPictureBuffer.LastFeed);
            }
            public bool DecodeSinglePicture(
                Bitmap output,
                byte[] inbuf, int inbufOff, int inbufLen,
                bool forceHasReferenceFrame)
            {
                return DecodeSinglePicture(output,
                    inbuf, inbufOff, inbufLen,
                    forceHasReferenceFrame ? 1 : 0);
            }


            public Bitmap AllocateBitmap(PictureFormat format)
            {
                if ((this.Width + this.Height) == 0) return null;
                switch (format)
                {
                    case PictureFormat.BGR: //currently the only suported format...
                        return new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    default:
                        return null;
                }
            }

            public void Dispose()
            {
                DLL.vpm_vesdec_free(_vesdec);
            }
        }

        public class AutoVESImageDecoder : IDisposable
        {
            public delegate void DecoderChangedEventHandler(object sender);
            public event DecoderChangedEventHandler DecoderChangedEvent;
            public delegate void PictureDecodedEventHandler(object sender);
            public event PictureDecodedEventHandler PictureDecodedEvent;

            private const VESDec.PictureFormat ALLOCATE_PICTURE_FORMAT = VESDec.PictureFormat.BGR;

            private readonly VESDec _vesdec = new VESDec();
            private MPEG1Sequence _loadedSequence = null;

            private VESDec.DecoderPictureBuffer _lastDecodedPictureBufferType = VESDec.DecoderPictureBuffer.Invalid;
            private Bitmap _decodedPicture = null;
            private Bitmap _currentPictureBuffer = null;
            private Bitmap _forwardPictureBuffer = null;
            private Bitmap _backwardPictureBuffer = null;

            public ThreadWorkerPool ThreadWorkerPool = null;

            public Image DecodedPicture { get { return _decodedPicture; } }

            public Image CurrentPictureBuffer
            {
                get
                {
                    if (_currentPictureBuffer == null)
                    {
                        _currentPictureBuffer = _vesdec.AllocateBitmap(ALLOCATE_PICTURE_FORMAT);
                        collectDecoderPictureBuffers();
                    }
                    return _currentPictureBuffer;
                }
            }
            public Image ForwardPictureBuffer
            {
                get
                {
                    if (_forwardPictureBuffer == null)
                    {
                        _forwardPictureBuffer = _vesdec.AllocateBitmap(ALLOCATE_PICTURE_FORMAT);
                        collectDecoderPictureBuffers();
                    }
                    return _forwardPictureBuffer;
                }
            }
            public Image BackwardPictureBuffer
            {
                get
                {
                    if (_backwardPictureBuffer == null)
                    {
                        _backwardPictureBuffer = _vesdec.AllocateBitmap(ALLOCATE_PICTURE_FORMAT);
                        collectDecoderPictureBuffers();
                    }
                    return _backwardPictureBuffer;
                }
            }


            public bool Decode(MPEG1Picture picture)
            {
                if (picture == null) return false;

                var sequence = lookupSequence(picture);
                if ((sequence != null) && (_loadedSequence != sequence))
                {
                    if (!loadSequence(sequence)) return false;
                }
                if (_loadedSequence == null) return false;

                var iter = picture.Source.Iterator;
                iter.SeekSourceTo(picture.Source.IteratorSourceStreamPosition);
                if (!iter.MPEGObjectValid) return false;
                if (iter.MPEGObjectType != MPEG1Picture.STREAM_ID_TYPE) return false;

                var picbuf = new MPEG1PictureBufferBuilder(iter);
                var mainp = new MPEG1PictureBufferManipulator(picbuf.Buffer, 0, picbuf.Length);
                mainp.OverrideFCode(4, 4);
                if (_vesdec.FramesDecoded == 0)
                {
                    if (!_vesdec.DecodeSinglePicture(_decodedPicture, picbuf.Buffer, 0, picbuf.Length)) return false;
                }
                else
                {
                    if (!_vesdec.DecodeSinglePicture(_decodedPicture, picbuf.Buffer, 0, picbuf.Length, true)) return false;
                }
                _lastDecodedPictureBufferType = _vesdec.LastFeedPictureBufferType;
                if (PictureDecodedEvent != null) PictureDecodedEvent(this);

                collectDecoderPictureBuffers();
                return true;
            }

            public void Reset()
            {
                _vesdec.Reset();
                _lastDecodedPictureBufferType = VESDec.DecoderPictureBuffer.Invalid;
                _decodedPicture = null;
                _currentPictureBuffer = null;
                _forwardPictureBuffer = null;
                _backwardPictureBuffer = null;
                if (DecoderChangedEvent != null) DecoderChangedEvent(this);
            }

            public void Dispose()
            {
                _vesdec.Dispose();
            }

            public VESDec.DecoderPictureBuffer LastDecodePictureBufferType => _lastDecodedPictureBufferType;

            private static MPEG1Sequence lookupSequence(IMPEG1Object obj)
            {
                for (; obj != null; obj = obj.Parent)
                {
                    if (obj is MPEG1Sequence)
                        return (MPEG1Sequence)obj;
                }
                return null;
            }

            private bool loadSequence(MPEG1Sequence sequence)
            {
                var iter = sequence.Source.Iterator;
                iter.SeekSourceTo(sequence.Source.IteratorSourceStreamPosition);
                if (!iter.MPEGObjectValid) return false;
                if (iter.MPEGObjectType != MPEG1Sequence.STREAM_ID_TYPE) return false;

                _loadedSequence = null;
                if (_vesdec.InitializeWithSequenceHeader(iter.MPEGObjectBuffer, iter.AbsoluteBufferOffset, iter.AbsoluteBufferLength))
                {
                    _loadedSequence = sequence;
                    reallocateBitmaps();
                    if (DecoderChangedEvent != null) DecoderChangedEvent(this);
                }
                else
                {
                    Reset();
                }

                return _loadedSequence != null;
            }

            private void reallocateBitmaps()
            {
                if (_decodedPicture != null)
                {
                    if (!((_decodedPicture.Width == _vesdec.Width) && (_decodedPicture.Height == _vesdec.Height)))
                        _decodedPicture = null;
                }
                if (_decodedPicture == null)
                    _decodedPicture = _vesdec.AllocateBitmap(ALLOCATE_PICTURE_FORMAT);

                _currentPictureBuffer = null;
                _forwardPictureBuffer = null;
                _backwardPictureBuffer = null;
            }

            private void collectDecoderPictureBuffers()
            {
                if (this.ThreadWorkerPool != null)
                {
                    var currentWorker  = this.ThreadWorkerPool.EnqueueJob(delegate { _vesdec.CollectPictureBuffer(_currentPictureBuffer,  VESDec.DecoderPictureBuffer.Current);  });
                    var forwardWorker  = this.ThreadWorkerPool.EnqueueJob(delegate { _vesdec.CollectPictureBuffer(_forwardPictureBuffer,  VESDec.DecoderPictureBuffer.Forward);  });
                    var backwardWorker = this.ThreadWorkerPool.EnqueueJob(delegate { _vesdec.CollectPictureBuffer(_backwardPictureBuffer, VESDec.DecoderPictureBuffer.Backward); });
                    backwardWorker.WaitForCompletion();
                    forwardWorker.WaitForCompletion();
                    currentWorker.WaitForCompletion();
                }
                else
                {
                    _vesdec.CollectPictureBuffer(_currentPictureBuffer, VESDec.DecoderPictureBuffer.Current);
                    _vesdec.CollectPictureBuffer(_forwardPictureBuffer, VESDec.DecoderPictureBuffer.Forward);
                    _vesdec.CollectPictureBuffer(_backwardPictureBuffer, VESDec.DecoderPictureBuffer.Backward);
                }
            }
        }
    }
}
