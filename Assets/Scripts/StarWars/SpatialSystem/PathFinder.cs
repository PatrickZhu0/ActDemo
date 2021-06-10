using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWarsSpatial
{
    public class PathNode
    {
        public PathNode(CellPos cellpos)
        {
            cell = cellpos;
            parent = null;
            cast = 0;
            from_start_cast = 0;
        }

        public CellPos cell;
        public PathNode parent;
        public float cast;
        public float from_start_cast;
    }
    public sealed class PathFinder
    {
        public static List<CellPos> FindPath(ICellMapView cellMapView, CellPos start, CellPos target)
        {
            JumpPointFinder finder = new JumpPointFinder(cellMapView);
            return finder.FindPath(start, target);
        }
        public static List<CellPos> FindPath(ICellMapView cellMapView, CellPos start, CellPos target, CellPos minCell, CellPos maxCell)
        {
            JumpPointFinder finder = new JumpPointFinder(cellMapView, minCell, maxCell);
            return finder.FindPath(start, target);
        }
    }
}
