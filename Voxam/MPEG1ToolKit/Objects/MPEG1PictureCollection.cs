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
using System.Collections.Generic;

using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit.Objects
{
    public class MPEG1PictureCollection : IMPEG1PictureCollection
    {
        private readonly List<MPEG1Picture> _list;
        private MPEG1PictureCollection(List<MPEG1Picture> list)
        {
            _list = list;
        }

        public static MPEG1PictureCollection Marshal(MPEG1StreamObjectIterator iter)
        {
            var pictures = new List<MPEG1Picture>();

            MPEG1Sequence sequence = null;
            MPEG1GOP gop = null;
            IMPEG1Object pictureParent = null;


            for(; !iter.EndOfStream; iter.Next())
            {
                if (!iter.MPEGObjectValid) continue;
                switch(iter.MPEGObjectType)
                {
                    case MPEG1Sequence.STREAM_ID_TYPE:
                        sequence = MPEG1Sequence.Marshal(iter);
                        gop = null;
                        pictureParent = sequence;
                        break;
                    case MPEG1SequenceEnd.STREAM_ID_TYPE:
                        sequence = null;
                        gop = null;
                        pictureParent = null;
                        break;
                    case MPEG1GOP.STREAM_ID_TYPE:
                        gop = MPEG1GOP.Marshal(iter, sequence);
                        if (gop != null) pictureParent = gop;
                        break;
                    case MPEG1Picture.STREAM_ID_TYPE:
                        MPEG1Picture picture = MPEG1Picture.Marshal(iter, pictureParent);
                        if (picture != null) pictures.Add(picture);
                        break;
                }
            }

            return new MPEG1PictureCollection(pictures);
        }

        public int Count { get { return _list.Count; } }
        public MPEG1Picture this[int index] { get { return _list[index]; } }
    }
}
