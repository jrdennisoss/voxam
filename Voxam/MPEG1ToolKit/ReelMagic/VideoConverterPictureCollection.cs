﻿/*
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

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam.MPEG1ToolKit.ReelMagic
{
    public class VideoConverterPictureCollection
    {
        private readonly VideoConverterSettings _settings;
        private readonly IMPEG1PictureCollection _pictures;
        private readonly Dictionary<MagicalSequence, VideoConverter> _converterCache = new Dictionary<MagicalSequence, VideoConverter>();
        public VideoConverterPictureCollection(VideoConverterSettings settings, IMPEG1PictureCollection pictures)
        {
            _settings = settings;
            _pictures = pictures;
        }
        public VideoConverter this[IMPEG1Object obj]
        {
            get
            {
                for (; obj != null; obj = obj.Parent)
                {
                    if (obj is MagicalSequence)
                    {
                        VideoConverter converter = null;
                        if (!_converterCache.TryGetValue((MagicalSequence)obj, out converter))
                        {
                            converter = new VideoConverter(_settings, _pictures, (MagicalSequence)obj);
                            _converterCache.Add((MagicalSequence)obj, converter);
                        }
                        return converter;
                    }
                }
                return null;
            }
        }
    }
}