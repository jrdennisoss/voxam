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

using System.Collections;
using System.Collections.Generic;
using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit.Objects
{
    public class MPEG1ObjectCollection : IMPEG1ObjectCollection
    {
        private readonly List<IMPEG1Object> _list;
        private MPEG1ObjectCollection(List<IMPEG1Object> list)
        {
            _list = list;
        }

        public static MPEG1ObjectCollection Marshal(MPEG1StreamObjectIterator iter)
        {
            var objects = new List<IMPEG1Object>();

            MPEG1SystemHeader currentSystemHeader = null;

            for (; !iter.EndOfStream; iter.Next())
            {
                if (iter.MPEGObjectValid && (iter.MPEGObjectType == MPEG1SystemHeader.STREAM_ID_TYPE)) currentSystemHeader = null;
                var obj = MPEG1Object.Marshal(iter, currentSystemHeader);
                if (obj == null) continue;
                objects.Add(obj);
            }
            return new MPEG1ObjectCollection(objects);
        }

        public IMPEG1Object this[int index] => _list[index];
        public int Count => _list.Count;


        public IEnumerator<IMPEG1Object> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
        private class Enumerator : IEnumerator<IMPEG1Object>
        {
            private int _idx = -1;
            private readonly MPEG1ObjectCollection _collection;
            public Enumerator(MPEG1ObjectCollection collection) => _collection = collection;
            public IMPEG1Object Current => _collection[_idx];
            object IEnumerator.Current => _collection[_idx];
            public void Dispose() { }
            public bool MoveNext() => ++_idx < _collection.Count;
            public void Reset() => _idx = -1;
        }


    }
}
