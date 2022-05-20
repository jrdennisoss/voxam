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

using Voxam.MPEG1ToolKit.Objects;
using Voxam.MPEG1ToolKit.Streams;


namespace Voxam
{
    public class MasterSourceProvider : IDisposable
    {
        public readonly MPEG1StreamObjectIterator PrimaryIterator;

        private MPEG1StreamObjectIterator _videoIterator = null;
        public MPEG1StreamObjectIterator VideoIterator
        {
            get
            {
                return _videoIterator;
            }
        }

        IMPEG1PictureCollection _pictureCollection = null;
        public IMPEG1PictureCollection Pictures
        {
            get
            {
                if (_pictureCollection == null)
                {
                    var iter = VideoIterator;
                    if (iter == null) return null;
                    iter.SeekSourceTo(0);
                    _pictureCollection = MPEG1PictureCollection.Marshal(iter);
                }
                return _pictureCollection;
            }
        }

        public MasterSourceProvider(string filename)
        {
            PrimaryIterator = new MPEG1StreamObjectIterator(new MPEG1FileStreamObjectSource(filename));
            _videoIterator = AllocateSubstream(0xE0);
        }

        public MPEG1StreamObjectIterator AllocateSubstream(byte streamid)
        {
            if (!PrimaryIterator.StartOfStream) PrimaryIterator.SeekSourceTo(0);
            for (; !PrimaryIterator.EndOfStream; PrimaryIterator.Next())
            {
                if (PrimaryIterator.MPEGObjectValid &&
                    PrimaryIterator.MPEGObjectTypeSubstream &&
                    (PrimaryIterator.MPEGObjectType == streamid))
                {
                    return new MPEG1StreamObjectIterator(new MPEG1SubStreamObjectSource(PrimaryIterator));
                }
            }
            return null;
        }

        public void Dispose()
        {
            PrimaryIterator.Dispose();
            _videoIterator?.Dispose();
        }
    }
}
