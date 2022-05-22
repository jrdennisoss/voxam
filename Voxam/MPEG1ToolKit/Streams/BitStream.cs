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

namespace Voxam.MPEG1ToolKit.Streams
{
    public class BitStream
    {
        private readonly byte[] _buf;
        private readonly int _off;
        private readonly int _len;
        private readonly int _offMax;


        public BitStream(byte[] buf, int off, int len)
        {
            _buf = buf;
            _off = (off < 1) ? 0 : off;
            _len = (len < 1) ? 0 : len;
            _offMax = _off + _len;

            Rewind();
        }

        public UInt64 DecodeUInt64(int bitIndex, int bitCount = 64)
        {
            UInt64 rv = 0;
            UInt64 byteval;

            int byteIndex = (bitIndex >> 3) + _off;
            bitIndex &= 0x07;

            int bitsPerCurrentByteAvailable;

            for (; (bitCount > 0) && (byteIndex < _offMax); ++byteIndex, bitIndex = 0)
            {
                bitsPerCurrentByteAvailable = 8 - bitIndex;
                byteval = _buf[byteIndex];
                byteval &= 0xFFul >> bitIndex;
                if (bitCount < bitsPerCurrentByteAvailable)
                {
                    byteval >>= bitsPerCurrentByteAvailable - bitCount;
                    bitsPerCurrentByteAvailable = bitCount;
                }

                rv <<= bitsPerCurrentByteAvailable;
                rv |= byteval;
                bitCount -= bitsPerCurrentByteAvailable;
            }

            rv <<= bitCount;
            return rv;
        }




        public int BitsRemaining
        {
            get
            {
                int rv = (_len << 3) - _bitIndex;
                if (rv < 0) return 0;
                return rv;
            }
        }
        private int _bitIndex;
        public void Rewind()
        {
            _bitIndex = 0;
        }


        public void Skip(int bitCount)
        {
            _bitIndex += bitCount;
        }
        public UInt64 ReadUInt64(int bitCount = 64)
        {
            UInt64 rv = DecodeUInt64(_bitIndex, bitCount);
            _bitIndex += bitCount;
            return rv;
        }

        public uint ReadUInt(int bitCount = 32)
        {
            uint rv = (uint)DecodeUInt64(_bitIndex, bitCount);
            _bitIndex += bitCount;
            return rv;
        }

        public int ReadInt(int bitCount = 32)
        {
            int rv = (int)DecodeUInt64(_bitIndex, bitCount);
            _bitIndex += bitCount;
            return rv;
        }

        public byte ReadByte(int bitCount = 8)
        {
            byte rv = (byte)DecodeUInt64(_bitIndex, bitCount);
            _bitIndex += bitCount;
            return rv;
        }

        public bool ReadBool(int bitCount = 1)
        {
            return ReadInt(bitCount) != 0;
        }
    }
}
