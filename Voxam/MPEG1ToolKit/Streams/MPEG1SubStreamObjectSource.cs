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
using System.IO;

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam.MPEG1ToolKit.Streams
{
    public class MPEG1SubStreamObjectSource : IMPEG1StreamObjectSource
    {
        public readonly MPEG1StreamObjectIterator Parent;
        public readonly long ParentSourceStartPosition;
        public readonly byte StreamId;

        private int _currentMPEGObjectBufferOffset = 0;
        private int _currentMPEGObjectBufferLength = 0;
        private long _currentSubstreamPosition = 0;
        private MPEG1PESPacket _currentPacket = null;


        public MPEG1SubStreamObjectSource(MPEG1StreamObjectIterator duplicateFrom)
        {
            if (!duplicateFrom.MPEGObjectValid) throw new Exception("Can not create substream iterator as parent stream is not position on a valid substream / PES object");
            if (!duplicateFrom.MPEGObjectTypeSubstream) throw new Exception("Can not create substream iterator as parent stream is not position on a valid substream / PES object");
            Parent = duplicateFrom.Duplicate();
            ParentSourceStartPosition = Parent.MPEGObjectSourceStreamPosition;
            StreamId = Parent.MPEGObjectType;

            startCurrentPacket();
        }

        private MPEG1SubStreamObjectSource(MPEG1SubStreamObjectSource duplicateFrom)
        {
            Parent = duplicateFrom.Parent.Duplicate(duplicateFrom.ParentSourceStartPosition);
            ParentSourceStartPosition = duplicateFrom.ParentSourceStartPosition;
            StreamId = duplicateFrom.StreamId;

            startCurrentPacket();

            this.Seek(duplicateFrom._currentSubstreamPosition, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            Parent.Dispose();
        }

        public IMPEG1StreamObjectSource Duplicate()
        {
            return new MPEG1SubStreamObjectSource(this);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (count > 0)
            {
                while (_currentMPEGObjectBufferLength < 1)
                {
                    if (!advanceToNextPacket()) return totalBytesRead;
                }
                int copySize = count;
                if (copySize > _currentMPEGObjectBufferLength)
                    copySize = _currentMPEGObjectBufferLength;
                Array.Copy(Parent.MPEGObjectBuffer, _currentMPEGObjectBufferOffset, buffer, offset, copySize);
                _currentMPEGObjectBufferOffset += copySize;
                _currentMPEGObjectBufferLength -= copySize;
                _currentSubstreamPosition += copySize;
                offset += copySize;
                count -= copySize;
                totalBytesRead += copySize;
            }
            return totalBytesRead;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Current) return this.Seek(_currentSubstreamPosition + offset, SeekOrigin.Begin);
            if (origin != SeekOrigin.Begin) throw new NotImplementedException();

            if (offset < 0) throw new ArgumentOutOfRangeException();


            if (offset < _currentSubstreamPosition)
            {
                Console.Error.WriteLine("WARNING: OPTIMIZE THIS! SEEKING PARENT BACK TO PES START!!");
                Parent.SeekSourceTo(ParentSourceStartPosition, SeekOrigin.Begin);
                _currentSubstreamPosition = 0;
                startCurrentPacket();
            }
            offset -= _currentSubstreamPosition;

            while (offset > _currentMPEGObjectBufferLength)
            {
                Console.Error.WriteLine("WARNING: OPTIMIZE THIS! JUMPING THROUGH PES PACKETS...");
                offset -= _currentMPEGObjectBufferLength;
                _currentSubstreamPosition += _currentMPEGObjectBufferLength;
                if (!advanceToNextPacket()) return -1;
            }

            _currentMPEGObjectBufferOffset += (int)offset;
            _currentMPEGObjectBufferLength -= (int)offset;
            _currentSubstreamPosition += offset;

            return 0; //???
        }

        private void startCurrentPacket()
        {
            _currentPacket = MPEG1PESPacket.Marshal(Parent);
            _currentMPEGObjectBufferOffset = Parent.MPEGObjectContentsBufferOffset;
            _currentMPEGObjectBufferLength = 0;
            if (_currentPacket == null) return;
            _currentMPEGObjectBufferOffset += _currentPacket.PayloadStartOffset;
            _currentMPEGObjectBufferLength = _currentPacket.PayloadLength;
        }

        private bool advanceToNextPacket()
        {
            clearCurrentPacket();
            while (Parent.Next())
            {
                if (Parent.MPEGObjectValid && (Parent.MPEGObjectType == this.StreamId))
                {
                    //found the next packet...
                    startCurrentPacket();
                    return true;
                }
            }
            return false;
        }

        private void clearCurrentPacket()
        {
            _currentPacket = null;
            _currentMPEGObjectBufferOffset = 0;
            _currentMPEGObjectBufferLength = 0;
        }
    }
}
