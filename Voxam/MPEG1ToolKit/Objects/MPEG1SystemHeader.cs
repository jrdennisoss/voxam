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

using System.Collections.Generic;

using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit.Objects
{
    public class MPEG1SystemHeader : IMPEG1Object
    {
        public const byte STREAM_ID_TYPE = 0xBB;

        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;

        public readonly int MaximumProgramMuxRate;
        public readonly int MaximumAudioStreams;
        public readonly bool FixedBitrate;
        public readonly bool ConstrainedParameters;
        public readonly bool SystemAudioLock;
        public readonly bool SystemVideoLock;
        public readonly int MaximumVideoStreams;
        public readonly bool PacketRateRestriction;


        public class Stream
        {
            public const byte STREAMID_ALL_VIDEO = 0xB8;
            public const byte STREAMID_ALL_AUDIO = 0xB9;

            public readonly byte StreamId;
            public readonly bool PStdBufferBoundScale;
            public readonly int PStdBufferSizeBound;

            public Stream(byte streamId, bool pStdBufferBoundScale, int pStdBufferSizeBound)
            {
                StreamId = streamId;
                PStdBufferBoundScale = pStdBufferBoundScale;
                PStdBufferSizeBound = pStdBufferSizeBound;
            }

            public static Stream Marshal(BitStream bits)
            {
                if (bits.BitsRemaining < 24) return null;

                byte streamId = bits.ReadByte(8);
                if ((streamId & 0x80) == 0) return null;

                if (bits.ReadInt(2) != 0x3) return null;

                bool pStdBufferBoundScale = bits.ReadBool(1);
                int pStdBufferSizeBound = bits.ReadInt(13);

                return new Stream(streamId, pStdBufferBoundScale, pStdBufferSizeBound);
            }
        }
        private readonly Stream[] _streams;
        public int StreamCount { get { return _streams == null ? 0 : _streams.Length; } }
        public Stream GetStreamAt(int index)
        {
            if (_streams == null) return null;
            if (index >= _streams.Length) return null;
            if (index < 0) return null;
            return _streams[index];
        }


        public MPEG1SystemHeader(IMPEG1Object parent, MPEG1ObjectSource source,
            int maximumProgramMuxRate, int maximumAudioStreams, bool fixedBitrate, bool constrainedParameters, bool systemAudioLock, bool systemVideoLock, int maximumVideoStreams, bool packetRateRestriction,
            Stream[] streams)
        {
            _parent = parent;
            _source = source;
            MaximumProgramMuxRate = maximumProgramMuxRate;
            MaximumAudioStreams = maximumAudioStreams;
            FixedBitrate = fixedBitrate;
            ConstrainedParameters = constrainedParameters;
            SystemAudioLock = systemAudioLock;
            SystemVideoLock = systemVideoLock;
            MaximumVideoStreams = maximumVideoStreams;
            PacketRateRestriction = packetRateRestriction;
            _streams = streams;
        }

        public static MPEG1SystemHeader Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            if (iter.MPEGObjectType != STREAM_ID_TYPE) return null;

            int len = iter.MPEGObjectContentsBufferLength;
            if (len < 8) return null;
            BitStream bits = new BitStream(iter.MPEGObjectBuffer, iter.MPEGObjectContentsBufferOffset, len);

            //no point in parsing the provided header as it is not really respected according to this:
            //  http://stnsoft.com/DVD/sys_hdr.html
            //    "By definition the processing of the Program Stream System Header will continue so
            //     long as the most significant bit of the next available byte is set,
            //     regardless of the header length."
            //
            //int headerLength = bits.ReadInt(16);
            //if (headerLength > (len - 2)) return null;
            bits.Skip(16);

            if (!bits.ReadBool(1)) return null;
            int maximumProgramMuxRate = bits.ReadInt(22);
            if (!bits.ReadBool(1)) return null;
            int maximumAudioStreams = bits.ReadInt(6);
            bool fixedBitrate = bits.ReadBool(1);
            bool constrainedParameters = bits.ReadBool(1);
            bool systemAudioLock = bits.ReadBool(1);
            bool systemVideoLock = bits.ReadBool(1);
            if (!bits.ReadBool(1)) return null;
            int maximumVideoStreams = bits.ReadInt(5);
            bool PacketRateRestriction = bits.ReadBool(1);

            List<Stream> streams = new List<Stream>();
            Stream stream;
            while ((stream = Stream.Marshal(bits)) != null) streams.Add(stream);

            return new MPEG1SystemHeader(parent, new MPEG1ObjectSource(iter),
                maximumProgramMuxRate, maximumAudioStreams, fixedBitrate, constrainedParameters, systemAudioLock, systemVideoLock, maximumVideoStreams, PacketRateRestriction,
                streams.ToArray());
        }

        public string Name => "MPEG-1 System Header";

        public IMPEG1Object Parent => _parent;

        public MPEG1ObjectSource Source => _source;

        public byte StreamIdType => STREAM_ID_TYPE;
    }
}
