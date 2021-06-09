using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using StarWars;

namespace StarWarsSpatial
{
    public interface ISpatialSystem
    {
        float Width { get; }
        float Height { get; }
        /// <summary>
        /// 将物体obj加入到空间系统新增列表，物体将在下一帧加入到管理队列中，
        /// 通过区域等查询接口也只能在下一帧后才能取到该物体
        /// </summary>
        /// <param name="obj">待新增的物体</param>
        /// <returns></returns>
        RetCode AddObj(ISpaceObject obj);

        /// <summary>
        /// 将物体obj加入到空间系统的删除缓冲列表, 物体将在当前帧之后被删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool RemoveObj(ISpaceObject obj);

        /// <summary>
        /// 查询物休obj是否在area区域内
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        bool IsObjectInArea(ISpaceObject obj, Shape area);

        /// <summary>
        /// 取得位于pos，朝向为direction与某个形状态相交的物体
        /// </summary>
        /// <param name="pos">area所在的位置</param>
        /// <param name="direction">area的朝向</param>
        /// <param name="area">查询的区域</param>
        /// <returns>返回在区域内的所有物体</returns>
        List<ISpaceObject> GetObjectInShape(Vector3 pos, float direction, Shape area);
        List<ISpaceObject> GetObjectInShape(Vector3 pos, float direction, Shape area, MyFunc<float, ISpaceObject, bool> pred);
        void VisitObjectInShape(Vector3 pos, float direction, Shape area, MyAction<float, ISpaceObject> visitor);
        void VisitObjectInShape(Vector3 pos, float direction, Shape area, MyFunc<float, ISpaceObject, bool> visitor);
        /// <summary>
        /// 取得与某个区域相交的物品（区域必须经过坐标变换，已经是世界坐标）
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        List<ISpaceObject> GetObjectInArea(Shape area);
        List<ISpaceObject> GetObjectInArea(Shape area, MyFunc<float, ISpaceObject, bool> pred);
        void VisitObjectInArea(Shape area, MyAction<float, ISpaceObject> visitor);
        void VisitObjectInArea(Shape area, MyFunc<float, ISpaceObject, bool> visitor);
        /// <summary>
        /// 取得圆心在center位置，半径为radius的圆内的物体
        /// </summary>
        /// <param name="center">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <returns>圆形区域内的物体</returns>
        List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius);
        List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> pred);
        void VisitObjectInCircle(Vector3 center, float radius, MyAction<float, ISpaceObject> visitor);
        void VisitObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> visitor);

        /// <summary>
        /// 取得从startpos开始到endpos的线段穿过的所有物体，该线段会受到不同api相应的阻挡影响(GetObjectCrossByLine不考虑阻挡)。
        /// </summary>
        /// <param name="startpos">线段开始位置</param>
        /// <param name="endpos">线段结束位置</param>
        /// <returns>返回线段穿过的物体列表，该列表以离starpos的距离升序排列</returns>
        List<ISpaceObject> GetObjectCrossByLine(Vector3 startpos, Vector3 endpos);
        List<ISpaceObject> GetObjectCrossByLineForPass(Vector3 startpos, Vector3 endpos);
        List<ISpaceObject> GetObjectCrossByLineForShoot(Vector3 startpos, Vector3 endpos);
        List<ISpaceObject> GetObjectCrossByLineForLeap(Vector3 startpos, Vector3 endpos);
        List<ISpaceObject> GetObjectCrossByLine(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred);
        List<ISpaceObject> GetObjectCrossByLineForPass(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred);
        List<ISpaceObject> GetObjectCrossByLineForShoot(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred);
        List<ISpaceObject> GetObjectCrossByLineForLeap(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred);

        /// <summary>
        /// 查询位置在指定多边形内的对象，不考虑对象半径。
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon);
        List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> pred);
        void VisitObjectInPolygon(IList<Vector3> polygon, MyAction<float, ISpaceObject> visitor);
        void VisitObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> visitor);

        /// <summary>
        /// 查询与线段相交的格子是否存在位于指定多边形内的路障（即掩体）
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="blindage"></param>
        /// <returns></returns>
        bool CheckRoadBlock(Vector3 start, Vector3 end, Vector3[] polygon, out Vector3 roadBlock);

        /// <summary>
        /// 是否能够站在某点
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="depth">网格深度</param>
        /// <returns>可以返回true, 反之返回false</returns>
        bool CanPass(Vector3 pos, int depth);
        bool CanShoot(Vector3 pos, int depth);
        bool CanLeap(Vector3 pos, int depth);
        bool CanSee(Vector3 pos, int depth);

        /// <summary>
        /// 查找从from到to的路径
        /// </summary>
        /// <param name="from">出发位置</param>
        /// <param name="to">结果位置</param>
        /// <param name="bodysize">物体所在网格的深度，如1时为1格，2时为二层9格</param>
        /// <returns>从from到to的路点，没有找到路径时为空</returns>
        List<Vector3> FindPath(Vector3 from, Vector3 to, int bodysize);

        /// <summary>
        /// 查找从from到to的路径
        /// </summary>
        /// <param name="from">出发位置</param>
        /// <param name="to">结果位置</param>
        /// <param name="scope">搜索区域（比from-to的矩形区大多少圈格子）</param>
        /// <param name="bodysize">物体所在网格的深度，如1时为1格，2时为二层9格</param>
        /// <returns>从from到to的路点，没有找到路径时为空</returns>
        /// <remarks>
        /// 注间：通常范围应该比起点到终点的矩形区域大至少一圈格子。
        /// </remarks>
        List<Vector3> FindPath(Vector3 from, Vector3 to, int scope, int bodysize);

        /// <summary>
        /// 计算对象避让其它对象后的速度矢量
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prefDir">优先的移动方向，需要是单位向量</param>
        /// <param name="stepTime"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="neighborDist"></param>
        /// <param name="isUsingAvoidanceVelocity"></param>
        /// <returns></returns>
        Vector3 ComputeVelocity(ISpaceObject obj, Vector3 prefDir, float stepTime, float maxSpeed, float neighborDist, bool isUsingAvoidanceVelocity);

        /// <summary>
        /// 判断物体obj是否可以走到to的位置，检测物体是否被阻挡挡住
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="to">所要去的位置</param>
        /// <returns>过以走到to时返回true, 反之返回false</returns>
        bool CanPass(ISpaceObject obj, Vector3 to);
        bool CanPass(Vector3 from, Vector3 to);

        /// <summary>
        /// 判断从某对象位置是否能对目标点进行射击
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool CanShoot(ISpaceObject obj, Vector3 to);
        bool CanShoot(Vector3 from, Vector3 to);

        /// <summary>
        /// 判断从某对象位置是否能飞跃到目标点
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool CanLeap(ISpaceObject obj, Vector3 to);
        bool CanLeap(Vector3 from, Vector3 to);

        /// <summary>
        /// 判断从某对象位置是否能看到目标点（视野阻挡）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool CanSee(ISpaceObject obj, Vector3 to);
        bool CanSee(Vector3 from, Vector3 to);

        /// <summary>
        /// 检测从from到to的线段触到的离开始点最近的静态阻挡点，该点为第一个阻挡格子中心点
        /// </summary>
        /// <param name="from">线段开始点</param>
        /// <param name="to">线段终点</param>
        /// <param name="hitpoint">碰撞点</param>
        /// <returns>当线段内有阻挡时返回true, hitpoint将被赋值，反之返回false</returns>
        bool RayCastForPass(Vector3 from, Vector3 to, out Vector3 hitpoint);
        bool RayCastForShoot(Vector3 from, Vector3 to, out Vector3 hitpoint);
        bool RayCastForLeap(Vector3 from, Vector3 to, out Vector3 hitpoint);

        ICellMapView GetCellMapView(int radius);
        /// <summary>
        /// 得到指定格子的阻挡信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        byte GetCellStatus(int row, int col);
        /// <summary>
        /// 设定指定格子的阻挡信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="status"></param>
        void SetCellStatus(int row, int col, byte status);
        /// <summary>
        /// 得到指定多边形包含的格子（以格子中心点计算）
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        List<CellPos> GetCellsInPolygon(IList<Vector3> pts);
        void VisitCellsInPolygon(IList<Vector3> pts, MyAction<int, int> visitor);
        /// <summary>
        /// 得到与指定线段相交的格子
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByLine(Vector3 start, Vector3 end);
        void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyAction<int, int> visitor);
        void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyFunc<int, int, bool> visitor);
        /// <summary>
        /// 得到与指定折线的边相交的格子
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByPolyline(IList<Vector3> pts);
        void VisitCellsCrossByPolyline(IList<Vector3> pts, MyAction<int, int> visitor);
        /// <summary>
        /// 得到与指定多边形的边相交的格子
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByPolygon(IList<Vector3> pts);
        void VisitCellsCrossByPolygon(IList<Vector3> pts, MyAction<int, int> visitor);
        /// <summary>
        /// 得到包含在圆内的格子（以格子中心计）
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<CellPos> GetCellsInCircle(Vector3 center, float radius);
        void VisitCellsInCircle(Vector3 center, float radius, MyAction<int, int> visitor);
        /// <summary>
        /// 得到与指定圆的边相交的格子（不保证精确相交，用圆心距粗略估算）
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByCircle(Vector3 center, float radius);
        void VisitCellsCrossByCircle(Vector3 center, float radius, MyAction<int, int> visitor);
        /// <summary>
        /// 计算离指定点在一定范围内最近的一个可到达点。
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        Vector3 CalcNearstReachablePoint(Vector3 src, float radius);
        /// <summary>
        /// 计算离指定点在一定范围内且在指定点与目标点之间的一个可到达点。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        Vector3 CalcNearstReachablePoint(Vector3 src, Vector3 target, float radius);
    }
}
