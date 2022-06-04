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

namespace Voxam.MPEG1ToolKit.Streams
{
    public class PESSubstreamPositionMapper
    {
        private readonly Dictionary<long, MPEG1PESPacket> _substreamPositionLookupMap = new Dictionary<long, MPEG1PESPacket>();
        private readonly List<SortedSet<long>> _substreamChunkMap = new List<SortedSet<long>>();

        private const int SUBSTREM_CHUNK_BITS = 14; //16k chunk lookups

        public MPEG1PESPacket LookupPacketFromSubstreamPosition(long substreamPosition, out int packetOffset)
        {
            packetOffset = 0;

            if (substreamPosition < 0) return null;

            int chunkMapIndex = (int)(substreamPosition >> SUBSTREM_CHUNK_BITS);
            if (chunkMapIndex >= _substreamChunkMap.Count) return null;
            var packetStartPositions = _substreamChunkMap[chunkMapIndex];

            long substreamPacketStartPosition = -1;
            foreach (var pos in packetStartPositions)
            {
                if (pos > substreamPosition) break;
                substreamPacketStartPosition = pos;
            }
            if (substreamPacketStartPosition == -1) return null;

            packetOffset = (int)(substreamPosition - substreamPacketStartPosition);
            var rv = _substreamPositionLookupMap[substreamPacketStartPosition];
            if (packetOffset >= rv.PayloadLength) return null;
            return rv;
        }


        internal void Push(long substreamPosition, MPEG1PESPacket packet)
        {
            if (_substreamPositionLookupMap.ContainsKey(substreamPosition)) return;
            _substreamPositionLookupMap.Add(substreamPosition, packet);

            int chunkMapIndex = (int)(substreamPosition >> SUBSTREM_CHUNK_BITS);
            long chunkMapIndexStreamPosition;
            do
            {
                while (_substreamChunkMap.Count <= chunkMapIndex)
                    _substreamChunkMap.Add(new SortedSet<long>());

                _substreamChunkMap[chunkMapIndex].Add(substreamPosition);

                ++chunkMapIndex;
                chunkMapIndexStreamPosition = chunkMapIndex;
                chunkMapIndexStreamPosition <<= SUBSTREM_CHUNK_BITS;
            } while (chunkMapIndexStreamPosition < (substreamPosition + packet.PayloadLength));
        }
    }
}
