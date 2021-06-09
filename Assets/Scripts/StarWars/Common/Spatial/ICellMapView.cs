using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWarsSpatial
{
    public interface ICellMapView
    {
        int Radius { get; }
        float RadiusLength { get; }
        int MaxRowCount { get; }
        int MaxColCount { get; }
        bool IsCellValid(int row, int col);
        bool GetCell(Vector3 pos, out int row, out int col);
        Vector3 GetCellCenter(int row, int col);
        List<CellPos> GetCellAdjacent(CellPos center);
        bool CanPass(int row, int col);
        bool CanPass(Vector3 targetPos);
        bool CanShoot(int row, int col);
        bool CanShoot(Vector3 targetPos);
        bool CanLeap(int row, int col);
        bool CanLeap(Vector3 targetPos);
        bool CanSee(int row, int col);
        bool CanSee(Vector3 targetPos);
        byte GetCellLevel(int row, int col);
        byte GetCellLevel(Vector3 targetPos);
        CellPos GetFirstWalkableCell(Vector3 from, Vector3 to);
    }
}
