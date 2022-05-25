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

using Voxam.MPEG1ToolKit.ReelMagic;
using Voxam.MPEG1ToolKit.Objects;
using Voxam.MPEG1ToolKit.Streams;

//
// This class acts as a single place to cotnrol access to the source stream
// that anything within the UI works with. This is because:
// . Caching can be managed in a single place
// . Filtering can be managed in a single place
// . Data fetching, notification events, and multi-threading can be managed in a single place
//


namespace Voxam
{
    public class MasterSourceProvider : IDisposable
    {
        //
        // system/main-level stuff is here...
        //
        public readonly MPEG1StreamObjectIterator PrimaryIterator;
        private IMPEG1ObjectCollection _objectCollection = null;
        public IMPEG1ObjectCollection Objects
        {
            get
            {
                if (_objectCollection == null)
                {
                    var iter = PrimaryIterator;
                    if (iter == null) return null;
                    iter.SeekSourceTo(0);
                    _objectCollection = MPEG1ObjectCollection.Marshal(iter);
                }
                return _objectCollection;
            }
        }



        //
        // video stream stuff is here...
        //
        private MPEG1StreamObjectIterator _videoIterator = null;
        public MPEG1StreamObjectIterator VideoIterator
        {
            get
            {
                if (_videoIterator == null) AutoSetVideoIterator();
                return _videoIterator;
            }
        }
        private IMPEG1PictureCollection _pictureCollection = null;
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


        //
        // ReelMagic video converter stuff...
        //
        public readonly VideoConverterSettings VideoConverterSettings = new VideoConverterSettings();
        private VideoConverterPictureCollection _videoConverterPictureCollection = null;
        public VideoConverterPictureCollection VideoConverterPictureCollection
        {
            get
            {
                if (_videoConverterPictureCollection == null)
                {
                    var pictures = Pictures;
                    if (pictures == null) return null;
                    _videoConverterPictureCollection = new VideoConverterPictureCollection(VideoConverterSettings, pictures);
                }
                return _videoConverterPictureCollection;
            }
        }






        public MasterSourceProvider(string filename)
        {
            PrimaryIterator = new MPEG1StreamObjectIterator(new MPEG1FileStreamObjectSource(filename));
        }

        public MPEG1StreamObjectIterator AllocateSubstream(byte streamid)
        {
            var iter = PrimaryIterator;
            iter.SeekSourceTo(0);
            for (; !iter.EndOfStream; iter.Next())
            {
                if (iter.MPEGObjectValid &&
                    iter.MPEGObjectTypeSubstream &&
                    (iter.MPEGObjectType == streamid))
                {
                    return new MPEG1StreamObjectIterator(new MPEG1SubStreamObjectSource(iter));
                }
            }
            return null;
        }

        public void AutoSetVideoIterator()
        {
            if (_videoIterator != null)
            {
                if (_videoIterator != PrimaryIterator)
                    _videoIterator.Dispose();
            }
            _videoIterator = null;

            //first see if the master iterator's first MPEG object is a video elemenatry stream only object
            var iter = PrimaryIterator;
            iter.SeekSourceTo(0);
            for (; !iter.EndOfStream; iter.Next())
            {
                if (!iter.MPEGObjectValid) continue;
                if (iter.MPEGObjectType <= 0xB8)
                {
                    //first valid MPEG object type in the stream is <= 0xB8...
                    //this is a video elementary stream (non-system) object...
                    //
                    //here we can assume that the primary iterator IS the video
                    //iterator... simply just reference the primary...
                    _videoIterator = PrimaryIterator;
                    return;
                }
                break;
            }

            //find the first video stream id in the stream...
            for (; !iter.EndOfStream; iter.Next())
            {
                if (!iter.MPEGObjectValid) continue;
                if (!iter.MPEGObjectTypeSubstream) continue;
                if ((iter.MPEGObjectType & 0xE0) != 0xE0) continue;
                _videoIterator = new MPEG1StreamObjectIterator(new MPEG1SubStreamObjectSource(iter));
                break;
            }
        }

        public void Dispose()
        {
            PrimaryIterator.Dispose();
            _videoIterator?.Dispose();
        }
    }
}
