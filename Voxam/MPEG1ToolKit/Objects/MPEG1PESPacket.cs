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

using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit.Objects
{
    public class MPEG1PESPacket : IMPEG1Object
    {
        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;

        private readonly byte _streamIdType;

        public const uint INVALID_PSTD = 0;
        public const UInt64 INVALID_PTSDTS = 0xFFFFFFFFFFFFFFFF;

        public readonly int PayloadLength; //after headers like padding/pstd/pts/dts
        public readonly int PayloadStartOffset; //zero-based; after headers like padding/pstd/pts/dts
        public readonly uint PSTDBufferScale;
        public readonly uint PSTDBufferSizeValue;
        public readonly UInt64 PTS;
        public readonly UInt64 DTS;

        public bool HavePSTDBuffer { get { return PSTDBufferScale != INVALID_PSTD; } }
        public uint PSTDBufferSize { get { return PSTDBufferScale * PSTDBufferSizeValue; } }
        public bool HavePTS { get { return PTS != INVALID_PTSDTS; } }
        public bool HaveDTS { get { return DTS != INVALID_PTSDTS; } }


        public string Name => "MPEG-1 PES Packet";
        public IMPEG1Object Parent => _parent;
        public MPEG1ObjectSource Source => _source;
        public byte StreamIdType => _streamIdType;

        public static bool IsPESType(byte streamIdType)
        {
            return (streamIdType >= 0xBD) && (streamIdType <= 0xEF);
        }

        public bool IsVideoType { get => (StreamIdType & 0xE0) == 0xE0; }
        public bool IsAudioType { get => (StreamIdType & 0xE0) == 0xC0; }
        public bool IsPrivateType { get => IsPrivate1Type || IsPrivate2Type; }
        public bool IsPrivate1Type { get => StreamIdType == 0xBD; }
        public bool IsPrivate2Type { get => StreamIdType == 0xBF; }
        public bool IsPaddingType { get => StreamIdType == 0xBE; }

        public MPEG1PESPacket(IMPEG1Object parent, MPEG1ObjectSource source, byte streamIdType,
            int payloadLength,
            int payloadStartOffset,
            uint pstdBufferScale = INVALID_PSTD,
            uint pstdBufferSizeValue = 0,
            UInt64 pts = INVALID_PTSDTS,
            UInt64 dts = INVALID_PTSDTS)
        {
            _parent = parent;
            _source = source;
            _streamIdType = streamIdType;
            PayloadLength = payloadLength;
            PayloadStartOffset = payloadStartOffset;
            PSTDBufferScale = pstdBufferScale;
            PSTDBufferSizeValue = pstdBufferSizeValue;
            PTS = pts;
            DTS = dts;
        }

        public static MPEG1PESPacket Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            var streamIdType = iter.MPEGObjectType;
            if (!IsPESType(streamIdType)) return null;

            int payloadLength = iter.MPEGObjectContentsBufferLength;
            int payloadStartOffset = 0;

            if ((streamIdType & 0xFE) == 0xBE) //these two substream types dont have any header extensions
                return new MPEG1PESPacket(parent, new MPEG1ObjectSource(iter), streamIdType, payloadLength, payloadStartOffset);

            uint pstdBufferScale = INVALID_PSTD;
            uint pstdBufferSizeValue = 0;
            UInt64 pts = INVALID_PTSDTS;
            UInt64 dts = INVALID_PTSDTS;


            byte[] buf = iter.MPEGObjectBuffer;
            int bufoff = iter.MPEGObjectContentsBufferOffset;

            //dispose of any padding
            while ((payloadLength > 0) && (buf[bufoff + payloadStartOffset] == 0xFF))
            {
                --payloadLength;
                ++payloadStartOffset;
            }

            //parse any header extensions...
            while ((payloadLength > 0) && ((pstdBufferScale == INVALID_PSTD) || (pts == INVALID_PTSDTS)))
            {
                if ((buf[bufoff + payloadStartOffset] & 0xC0) == 0x40)
                {
                    //first two bits = '01' which means we have a P-STD buffer size field present
                    //need two total bytes for this...
                    if (payloadLength < 2) { payloadLength = 0; break; }
                    pstdBufferScale = ((buf[bufoff + payloadStartOffset] & 0x20) != 0) ? (uint)1024 : (uint)128;
                    pstdBufferSizeValue = (uint)(buf[bufoff + payloadStartOffset] & 0x1F);
                    pstdBufferSizeValue <<= 8;
                    pstdBufferSizeValue |= (uint)buf[bufoff + payloadStartOffset + 1];
                    payloadLength -= 2;
                    payloadStartOffset += 2;
                }
                else if ((buf[bufoff + payloadStartOffset] & 0xF0) == 0x20)
                {
                    //only PTS field present
                    //need five total bytes for this...
                    if (payloadLength < 5) { payloadLength = 0; break; }
                    pts = parsePtsDtsField(buf, bufoff + payloadStartOffset);
                    payloadLength -= 5;
                    payloadStartOffset += 5;
                }
                else if ((buf[bufoff + payloadStartOffset] & 0xF0) == 0x30)
                {
                    //PTS and DTS field present
                    //need nine total bytes for this...
                    if (payloadLength < 9) { payloadLength = 0; break; }
                    pts = parsePtsDtsField(buf, bufoff + payloadStartOffset);
                    dts = parsePtsDtsField(buf, bufoff + payloadStartOffset + 5);
                    payloadLength -= 10;
                    payloadStartOffset += 10;
                }
                else if (buf[bufoff + payloadStartOffset] == 0x0f)
                {
                    //done with header
                    payloadLength -= 1;
                    payloadStartOffset += 1;
                    break;
                }
                else
                {
                    //unknown???
                    //assume we are done and this byte is part of the payload...
                    break;
                }
            }

            return new MPEG1PESPacket(parent, new MPEG1ObjectSource(iter), streamIdType,
                payloadLength, payloadStartOffset,
                pstdBufferScale, pstdBufferSizeValue,
                pts, dts);
        }



        private static UInt64 parsePtsDtsField(byte[] buf, int offset)
        {
            //check for the marker bits...
            if ((buf[offset + 4] & 0x01) != 0x01) return 0;
            if ((buf[offset + 2] & 0x01) != 0x01) return 0;
            if ((buf[offset + 0] & 0x01) != 0x01) return 0;

            //marshal the field...
            UInt64 rv = ((UInt64)buf[offset + 0] & 0x0E) >> 1; //3 bits
            rv <<= 8; rv |= (UInt64)buf[offset + 1];              //8 bits
            rv <<= 7; rv |= (UInt64)buf[offset + 2] >> 1;         //7 bits
            rv <<= 8; rv |= (UInt64)buf[offset + 3];              //8 bits
            rv <<= 7; rv |= (UInt64)buf[offset + 4] >> 1;         //7 bits
            return rv;
        }
    }
}
