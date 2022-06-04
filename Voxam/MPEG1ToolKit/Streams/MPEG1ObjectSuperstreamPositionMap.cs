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
using System.Collections;
using System.Collections.Generic;

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam.MPEG1ToolKit.Streams
{
    public class MPEG1ObjectSuperstreamPositionMap : IEnumerable<MPEG1ObjectSuperstreamPositionMap.Entry>
    {
        public class Entry
        {
            public readonly long Offset;
            public readonly int Length;
            public Entry(long offset, int length) { Offset = offset; Length = length; }
        }
        private readonly List<Entry> _entries = new List<Entry>();

        public MPEG1ObjectSuperstreamPositionMap(MPEG1StreamObjectIterator super, IMPEG1Object obj)
        {
            //ok so this is some intense shit... the purpose of this is to create a set of 
            //offsets and lengths that represent the byte fragmentation of given object
            //within the given superstream...

            //first a few sanity checks...
            if (obj == null) return;
            if (super == null) return;
            if (obj.Source == null) return;
            if (obj.Source.Iterator == null) return;

            //then map out the segments...
            for (int totalMappedLength = 0; totalMappedLength < obj.Source.IteratorSourceStreamAbsoluteLength;)
            {
                var entry = mapSegment(super, obj, totalMappedLength);
                if ((entry.Length + totalMappedLength) > obj.Source.IteratorSourceStreamAbsoluteLength)
                    entry = new Entry(entry.Offset, obj.Source.IteratorSourceStreamAbsoluteLength - totalMappedLength);
                _entries.Add(entry);
                totalMappedLength += entry.Length;
            }
        }

        private static Entry mapSegment(MPEG1StreamObjectIterator super, IMPEG1Object obj, int objByteOffset)
        {
            //is the given object's direct source the given super iterator? if so, we have reached the 
            //end of this recursive operation and simply return the object's coordinates...
            if (obj.Source.Iterator == super)
                return new Entry(obj.Source.IteratorSourceStreamPosition + objByteOffset, obj.Source.IteratorSourceStreamAbsoluteLength - objByteOffset);


            //if we get here, then we need to perform a PES offset translation...

            var ss = obj.Source.Iterator.Source as MPEG1SubStreamObjectSource;
            if (ss == null) throw new Exception("Super iterator is not found within object source chain.");

            long substreamPosition = obj.Source.IteratorSourceStreamPosition + objByteOffset;
            var pes = ss.PESSubstreamPositionMapper.LookupPacketFromSubstreamPosition(substreamPosition, out int objBytePESPayloadOffset);
            if (pes == null) throw new Exception("PES Lookup Failed");

            //note: the pes 'PayloadStartOffset' attribute does not include the start code and length fields
            //      therfore, we must account for those here when translating to the superstream source position...
            //      the start code is 4 bytes and length is 2 bytes, totalling a +6 byte absolute offset correction...
            int pesByteOffset = 6 + pes.PayloadStartOffset + objBytePESPayloadOffset;
            return mapSegment(super, pes, pesByteOffset);
        }

        public int Count { get { return _entries.Count; } }
        public Entry this[int index] { get { return _entries[index]; } }

        public IEnumerator<Entry> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
        private class Enumerator : IEnumerator<Entry>
        {
            private int _idx = -1;
            private readonly MPEG1ObjectSuperstreamPositionMap _map;
            public Enumerator(MPEG1ObjectSuperstreamPositionMap map) => _map = map;
            public Entry Current => _map[_idx];
            object IEnumerator.Current => _map[_idx];
            public void Dispose() { }
            public bool MoveNext() => ++_idx < _map.Count;
            public void Reset() => _idx = -1;
        }
    }
}
