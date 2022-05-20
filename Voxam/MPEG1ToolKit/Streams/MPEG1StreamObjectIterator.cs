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

namespace Voxam.MPEG1ToolKit.Streams
{
    public class MPEG1StreamObjectIterator : IDisposable
    {
        public readonly IMPEG1StreamObjectSource Source;
        public readonly byte[] MPEGObjectBuffer = new byte[256 * 1024];
        private int _mpegObjectBufferLength = 0; //how much data from start of buffer
        private int _mpegObjectBufferCurrentStartOffset = 0; //where the start code (starting with the 00 00 01 XX) of the current object starts
        private int _mpegObjectBufferNextStartOffset = 0; //where the start code (starting with the 00 00 01 XX) of the next object starts
        private long _mpegObjectBufferSourceStreamPosition = 0; //the position in the source stream that corolates to byte offset 0 in the MPEGObjectBuffer
        private bool _sourceStreamEnded = false;

        private const int START_CODE_LENGTH = 4;
        private const int SUBSTREAM_HEADER_LENGTH = START_CODE_LENGTH + 2;

        public MPEG1StreamObjectIterator(IMPEG1StreamObjectSource source) : this(source, 0) { }
        public MPEG1StreamObjectIterator(IMPEG1StreamObjectSource source, long offset)
        {
            Source = source;
            SeekSourceTo(offset);
        }

        /// <summary>
        /// Function <c>Duplicate()</c> creates a copy of this iterator as well as the source stream. If a new stream position/offset is
        /// not provided, then the duplicated iterator will be placed at the same position as the existing iterator.
        /// </summary>
        public MPEG1StreamObjectIterator Duplicate() {return Duplicate(this.MPEGObjectSourceStreamPosition);}
        public MPEG1StreamObjectIterator Duplicate(long offset)
        {
            return new MPEG1StreamObjectIterator(this.Source.Duplicate(), offset);
        }

        /// <summary>
        /// Property <c>StartOfStream</c> indicates if the iterator is position at the start of the stream; very first position.
        /// </summary>
        public bool StartOfStream
        {
            get
            {
                return (_mpegObjectBufferSourceStreamPosition == 0) && (_mpegObjectBufferCurrentStartOffset == 0);
            }
        }

        /// <summary>
        /// Property <c>EndOfStream</c> indicates if the iterator is at the end, which is not on a valid position.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return _sourceStreamEnded && (_mpegObjectBufferCurrentStartOffset >= _mpegObjectBufferLength);
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectValid</c> indicates if the iterator is current positioned and aligned on to a valid MPEG-1 object
        /// </summary>
        public bool MPEGObjectValid 
        {
            get
            {
                if ((_mpegObjectBufferLength - _mpegObjectBufferCurrentStartOffset) < START_CODE_LENGTH) return false;
                return (MPEGObjectBuffer[_mpegObjectBufferCurrentStartOffset + 0] == 0x00)
                    && (MPEGObjectBuffer[_mpegObjectBufferCurrentStartOffset + 1] == 0x00)
                    && (MPEGObjectBuffer[_mpegObjectBufferCurrentStartOffset + 2] == 0x01);
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectType</c> returns the MPEG-1 object type (aka stream ID)
        /// Note: <c>MPEGObjectValid</c> must first be checked before using this!
        /// </summary>
        public byte MPEGObjectType
        {
            get
            {
                return MPEGObjectBuffer[_mpegObjectBufferCurrentStartOffset + 3];
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectTypeSubstream</c> indicates whether the current MPEG-1 object type is a substream / packetized PES data
        /// Note: <c>MPEGObjectValid</c> must first be checked before using this!
        /// </summary>
        public bool MPEGObjectTypeSubstream
        {
            get
            {
                return (MPEGObjectType >= 0xBD) && (MPEGObjectType <= 0xEF);
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectComplete</c> indicates whether the current MPEG-1 object type is completely loaded in the buffer
        /// This will return false there is not a next MPEG-1 object found in the buffer.
        /// </summary>
        public bool MPEGObjectComplete
        {
            get
            {
                return _mpegObjectBufferNextStartOffset > 0;
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectContentsBufferOffset</c> returns the current iterator position in the buffer
        /// The conditions for the position value are as follows:
        /// <list type="bullet">
        ///     <item>
        ///         If the iterator is currently positioned on an invalid non-MPEG object, then the position returned is at the
        ///         current byte-position of the iterator.
        ///     </item>
        ///     <item>
        ///         If the iterator is currently positioned on a substream object, then the position returned is at the start
        ///         of the PES packet data.
        ///     </item>
        ///     <item>
        ///         If the iterator is currently positioned on a non-substream object, then the position returned is at the start
        ///         of the object data, past the start-code.
        ///     </item>
        /// </list>
        /// </summary>
        public int MPEGObjectContentsBufferOffset
        {
            get
            {
                int offset = _mpegObjectBufferCurrentStartOffset;
                if (this.MPEGObjectValid)
                {
                    if (this.MPEGObjectTypeSubstream)
                        offset += SUBSTREAM_HEADER_LENGTH;
                    else
                        offset += START_CODE_LENGTH;
                }
                return offset;
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectContentsBufferLength</c> returns the best known length of the current MPEG object's payload/content.
        /// The conditions for the length value are as follows:
        /// <list type="bullet">
        ///     <item>The length returned will never run beyond the boundary of the <c>MPEGObjectBuffer array</c>.</item>
        ///     <item>
        ///         If the iterator is currently positioned on an invalid non-MPEG object, then the length returned is the count
        ///         of bytes in the buffer up to the next known valid MPEG object. If there is not a next known valid MPEG object,
        ///         then the count of bytes returned is whatever is available in the buffer.
        ///     </item>
        ///     <item>
        ///         If the iterator is currently positioned on a substream object, then the length returned will be the substream
        ///         packet contents.
        ///     </item>
        ///     <item>
        ///         If the iterator is currently positioned on a non-substream object, then the length returned will be the
        ///         contents of the object up to the point of the next object in the stream, or the length of the remainder
        ///         of the buffer in the case where <c>MPEGObjectComplete</c> is false. 
        ///     </item>
        /// </list>
        /// </summary>
        public int MPEGObjectContentsBufferLength
        {
            get
            {
                //first, determing the maximum possible length of the object at the current position 
                //the iterator is on... if we have a known next object start in the buffer, use that
                //as the point to box in the size of our current object... however, if we do not know
                //the position of the next object, then simply use the 
                int length;
                if (_mpegObjectBufferNextStartOffset > _mpegObjectBufferCurrentStartOffset)
                    length = _mpegObjectBufferNextStartOffset - _mpegObjectBufferCurrentStartOffset;
                else
                    length = _mpegObjectBufferLength - _mpegObjectBufferCurrentStartOffset;

                if (this.MPEGObjectValid)
                {
                    if (this.MPEGObjectTypeSubstream)
                    {
                        //for substream type objects, this returns the payload size specified in the substream object's header
                        int substreamPayloadLength = decodeSubstreamPayloadLength();
                        if (substreamPayloadLength < 0)
                            length = 0; //something is bad... return a zero-length substream PES packet payload...
                        else if (substreamPayloadLength > length) //is this even a real use case? defensive i guess...
                            throw new Exception("Check the math between MPEGObjectContentsBufferLength and findNextObjectStart() !!");
                        else
                            length = substreamPayloadLength;
                    }
                    else
                    {
                        length -= START_CODE_LENGTH;
                    }
                }

                if (length < 0) length = 0; //ensure we NEVER return a negative length
                return length;
            }
        }

        /// <summary>
        /// Property <c>MPEGObjectSourceStreamPosition</c> returns the stream byte offset of the current iterator's position
        /// </summary>
        public long MPEGObjectSourceStreamPosition
        {
            get
            {
                long rv = _mpegObjectBufferSourceStreamPosition;
                rv += _mpegObjectBufferCurrentStartOffset;
                return rv;
            }
        }

        /// <summary>
        /// Property <c>AbsoluteBufferOffset</c> returns the current iterator position in the buffer
        /// </summary>
        public int AbsoluteBufferOffset { get { return _mpegObjectBufferCurrentStartOffset; } }

        /// <summary>
        /// Property <c>AbsoluteBufferOffset</c> returns the length of the current iteration's payload/content available in the buffer.
        /// </summary>
        public int AbsoluteBufferLength
        {
            get
            {
                if (_mpegObjectBufferNextStartOffset > _mpegObjectBufferCurrentStartOffset)
                    return _mpegObjectBufferNextStartOffset - _mpegObjectBufferCurrentStartOffset;
                return _mpegObjectBufferLength - _mpegObjectBufferCurrentStartOffset;
            }
        }

        /// <summary>
        /// Function <c>SeekSourceTo()</c> sets the position of this iterator directly at the specified byte-offset in the source stream
        /// </summary>
        public void SeekSourceTo(long source_offset) { SeekSourceTo(source_offset, SeekOrigin.Begin);}
        public void SeekSourceTo(long source_offset, SeekOrigin origin)
        {
            //translate everything to work in absolute offsets...
            if (origin == SeekOrigin.Current)
            {
                this.SeekSourceTo(this.MPEGObjectSourceStreamPosition + source_offset, SeekOrigin.Begin);
                return;
            }   
            if (origin != SeekOrigin.Begin) throw new NotImplementedException();

            //is the requested offset currently in our buffer/cache?
            if ((source_offset >= _mpegObjectBufferSourceStreamPosition) &&
                (source_offset < (_mpegObjectBufferSourceStreamPosition + _mpegObjectBufferLength)))
            {
                //requested position is indeed in the current buffer/cache...

                if (source_offset == this.MPEGObjectSourceStreamPosition)
                    return; //nothing to do... we are already positioned at the requested offset...

                //re-align the pointers, and only fetch if necessary...
                _mpegObjectBufferCurrentStartOffset = (int)(source_offset - _mpegObjectBufferSourceStreamPosition);
                _mpegObjectBufferNextStartOffset = 0;
                if (!findNextObjectStart())
                {
                    //specified offset does not contain a complete MPEG-1 object... need to shift and re-fill buffer...
                    shiftBuffer(_mpegObjectBufferCurrentStartOffset);
                    fillMPEGObjectBuffer();
                }
                return;
            }

            //not cached... need to clear and complete fetch data...
            clearState();
            //TODO: Warning: can seek fail?
            this.Source.Seek(source_offset, origin);
            _mpegObjectBufferSourceStreamPosition = source_offset;
            fillMPEGObjectBuffer();
            findNextObjectStart();
        }

        /// <summary>
        /// Function <c>Next()</c> advances the position of this iterator to the next object
        /// </summary>
        /// <returns>True indicating if the iterator is valid. False if the iterator beyond the end of the stream.</returns>
        public bool Next()
        {
            if (this.EndOfStream) return false;

            if (_mpegObjectBufferNextStartOffset > _mpegObjectBufferCurrentStartOffset)
            {
                //already have the start position of the next object... update things...
                _mpegObjectBufferCurrentStartOffset = _mpegObjectBufferNextStartOffset;
            }

            //ensure we have the start of the next object in the buffer so that we can determine the length of the current object...
            if (!findNextObjectStart())
            {
                shiftBuffer(_mpegObjectBufferCurrentStartOffset);
                fillMPEGObjectBuffer();
                return findNextObjectStart();
            }
            return true;
        }


        public void Dispose()
        {
            this.Source.Dispose();
        }





        private void clearState()
        {
            _mpegObjectBufferLength = 0;
            _mpegObjectBufferCurrentStartOffset = 0;
            _mpegObjectBufferNextStartOffset = 0;
            _mpegObjectBufferSourceStreamPosition = 0;
            _sourceStreamEnded = false;
        }
        private void fillMPEGObjectBuffer()
        {
            if (_sourceStreamEnded) return;
            while (_mpegObjectBufferLength < MPEGObjectBuffer.Length)
            {
                var bytesRead = this.Source.Read(this.MPEGObjectBuffer, _mpegObjectBufferLength, this.MPEGObjectBuffer.Length - _mpegObjectBufferLength);
                if (bytesRead < 1)
                {
                    _sourceStreamEnded = true;
                    break;
                }
                _mpegObjectBufferLength += bytesRead;
            }
        }

        private bool findNextObjectStart()
        {
            //this is an unnecessarily complicated routine; primarily because of the way MPEG-1 deliminates
            //objects in its stream... in a nutshell, MPEG-1 uses a pattern of 00 00 01 (aka "start code") to
            //deliminate the boundary of objects with a byte stream / file. unfortunately, there are a handful
            //of certain object types (aka substreams) that are allowed to contain the start code sequence.
            //therfore, we must be mindful of all this bullshit when determining where the next object 
            //actually starts at in the stream.

            int position = _mpegObjectBufferCurrentStartOffset;
            if (this.MPEGObjectValid && this.MPEGObjectTypeSubstream)
            {
                //our current iterator is positioned on a substream object... here we need to make sure that we
                //completely jump over its payload before finding the next object because a substream object's
                //payload can (and likely will in the case of a video elementary stream) contain a valid start
                //code (00 00 01) pattern... thanks MPEG-1 for such a shitty design...
                int substreamPayloadLength = decodeSubstreamPayloadLength();
                if (substreamPayloadLength < 0)
                {
                    //not enough data in the buffer to contain a complete substream MPEG-1 header
                    _mpegObjectBufferNextStartOffset = 0;
                    return false;
                }
                position += SUBSTREAM_HEADER_LENGTH + substreamPayloadLength;
            }
            else
            {
                //not a substream or even possible a valid/alined object...
                //start matching at one byte into the current iterator position... this way, if we are currently aligned, we don't
                //match the current iterator position
                position += 1;
            }

            //find the start code pattern (00 00 01) in the buffer starting at some position beyond the current object...
            for (; position <= (_mpegObjectBufferLength - START_CODE_LENGTH); ++position)
            {
                if ((MPEGObjectBuffer[position + 0] == 0x00) &&
                    (MPEGObjectBuffer[position + 1] == 0x00) &&
                    (MPEGObjectBuffer[position + 2] == 0x01))
                {
                    //found a start code offset in the buffer... return it...
                    _mpegObjectBufferNextStartOffset = position;
                    return true; //next object start position was found
                }
            }

            // if we get here, then a start code pattern was not found...
            if (_sourceStreamEnded)
            {
                //source stream has ended which means that we have the total remaining contents of the current
                //object in the buffer... set the next mpeg object start offset to the end of the buffer so that
                //way an "MPEGObjectComplete" query is still valid for the last object in the stream
                _mpegObjectBufferNextStartOffset = _mpegObjectBufferLength;
                return true; //next object start position was "found"
            }
            _mpegObjectBufferNextStartOffset = 0; //next start code not found in buffer... current buffered object is incomplete!
            return false; //next object start position was not found
        }

        private void shiftBuffer(int dispose_bytes)
        {
            if (dispose_bytes == 0) return; //nothing to do
            if (dispose_bytes > _mpegObjectBufferLength) throw new Exception("Attempting to shift and dispose more bytes than in the buffer!");
            if (dispose_bytes < _mpegObjectBufferLength)
            {
                Array.Copy(this.MPEGObjectBuffer, dispose_bytes, this.MPEGObjectBuffer, 0, _mpegObjectBufferLength - dispose_bytes);
            }

            _mpegObjectBufferLength -= dispose_bytes;
            _mpegObjectBufferCurrentStartOffset -= dispose_bytes;
            _mpegObjectBufferNextStartOffset -= dispose_bytes;
            _mpegObjectBufferSourceStreamPosition += dispose_bytes;

            if (_mpegObjectBufferCurrentStartOffset < 0) _mpegObjectBufferCurrentStartOffset = 0;
            if (_mpegObjectBufferNextStartOffset < 0) _mpegObjectBufferNextStartOffset = 0;
        }


        private int decodeSubstreamPayloadLength()
        {
            if ((_mpegObjectBufferLength - _mpegObjectBufferCurrentStartOffset) < SUBSTREAM_HEADER_LENGTH) return -1;
            int rv = this.MPEGObjectBuffer[_mpegObjectBufferCurrentStartOffset + START_CODE_LENGTH + 0];
            rv   <<= 8;
            rv    |= this.MPEGObjectBuffer[_mpegObjectBufferCurrentStartOffset + START_CODE_LENGTH + 1];
            return rv;
        }
    }
}
