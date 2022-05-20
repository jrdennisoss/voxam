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

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam.MPEG1ToolKit
{
    public class MPEG1PredictionTracker
    {

        private int _trackIndex;
        private PicturePredictionNode _currentForward;
        private PicturePredictionNode _currentBackward;
        private List<PicturePredictionNode> _trackedNodes = new List<PicturePredictionNode>();


        public class PicturePredictionNode
        {
            public PicturePredictionNode ForwardDependency = null;
            public PicturePredictionNode BackwardDependency = null;
            public readonly List<PicturePredictionNode> Dependents = new List<PicturePredictionNode>();
            public readonly MPEG1Picture Picture;
            public readonly int TrackIndex;
            public PicturePredictionNode(int trackIndex, MPEG1Picture picture)
            {
                Picture = picture;
                TrackIndex = trackIndex;
            }
        }

        public MPEG1PredictionTracker()
        {
            Clear();
        }

        public void Clear(int startingTrackIndex = 0)
        {
            _trackIndex = startingTrackIndex;
            _currentForward = _currentBackward = null;
            _trackedNodes.Clear();
        }

        public PicturePredictionNode LookupPicturePredictionNode(MPEG1Picture picture)
        {
            if (picture == null) return null;
            foreach (var node in _trackedNodes)
            {
                if (picture == node.Picture) return node;
            }
            return null;
        }

        public void TrackNewPicture(MPEG1Picture picture)
        {
            var node = new PicturePredictionNode(_trackIndex++, picture);
            _trackedNodes.Add(node);

            switch (picture.Type)
            {
                case MPEG1Picture.PictureType.IntraCoded:
                    trackNewAnchor(node);
                    break;
                case MPEG1Picture.PictureType.Predictive:
                    trackNewAnchor(node);
                    if ((node.ForwardDependency = _currentForward) != null)
                        _currentForward.Dependents.Add(node);
                    break;
                case MPEG1Picture.PictureType.Bipredictive:
                    if ((node.ForwardDependency = _currentForward) != null)
                        _currentForward.Dependents.Add(node);
                    if ((node.BackwardDependency = _currentBackward) != null)
                        _currentBackward.Dependents.Add(node);
                    break;
            }
        }
        private void trackNewAnchor(PicturePredictionNode newAnchor)
        {
            _currentForward = _currentBackward;
            _currentBackward = newAnchor;
        }


        /*
        private MPEG1GOP _currentGOP;
        private int _trackIndex;
        private PicturePredictionNode _currentAnchor;
        private List<PicturePredictionNode> _trackedNodes = new List<PicturePredictionNode>();

        public MPEG1GOP GOP { get { return _currentGOP; } }



        public class PicturePredictionNode
        {
            public PicturePredictionNode ForwardDependency = null;
            public PicturePredictionNode BackwardDependency = null;
            public readonly List<PicturePredictionNode> Dependents = new List<PicturePredictionNode>();
            public readonly MPEG1Picture Picture;
            public readonly int TrackIndex;
            public PicturePredictionNode(int trackIndex, MPEG1Picture picture)
            {
                Picture = picture;
                TrackIndex = trackIndex;
            }
        }

        public MPEG1PredictionTracker()
        {
            Clear();
        }

        public void Clear()
        {
            _currentGOP = null;
            _trackIndex = 0;
            _currentAnchor = new PicturePredictionNode(-1, null);
            _trackedNodes.Clear();
        }

        public PicturePredictionNode LookupPicturePredictionNode(MPEG1Picture picture)
        {
            if (picture == null) return null;
            foreach (var node in _trackedNodes)
            {
                if (picture == node.Picture) return node;
            }
            return null;
        }

        public void TrackNewPicture(MPEG1Picture picture)
        {
            MPEG1GOP gop = getPictureGop(picture);
            if (gop != _currentGOP)
            {
                Clear();
                _currentGOP = gop;
            }

            var node = new PicturePredictionNode(_trackIndex++, picture);
            _trackedNodes.Add(node);

            switch (picture.Type)
            {
                case MPEG1Picture.PictureType.IntraCoded:
                    trackNewAnchor(node);
                    break;
                case MPEG1Picture.PictureType.Predictive:
                    node.ForwardDependency = _currentAnchor;
                    _currentAnchor.Dependents.Add(node);
                    trackNewAnchor(node);
                    break;
                case MPEG1Picture.PictureType.Bipredictive:
                    node.ForwardDependency = _currentAnchor;
                    _currentAnchor.Dependents.Add(node);
                    break;
            }
        }

        private void trackNewAnchor(PicturePredictionNode newAnchor)
        {
            foreach (var node in _currentAnchor.Dependents)
            {
                if (node.TrackIndex < _currentAnchor.TrackIndex) continue;
                if (node.Picture.Type != MPEG1Picture.PictureType.Bipredictive) continue;
                node.BackwardDependency = newAnchor;
                newAnchor.Dependents.Add(node);
            }
            _currentAnchor = newAnchor;
        }

        private static MPEG1GOP getPictureGop(MPEG1Picture picture)
        {
            if (picture.Parent is MPEG1GOP) return (MPEG1GOP) picture.Parent;
            return null;
        }

        */
    }
}
