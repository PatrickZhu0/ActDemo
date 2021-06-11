using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public sealed class ControlSystemOperation
    {
        public static void AdjustCharacterPosition(int id, float x, float z, double time, float dir)
        {
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(id);
            if (null != info)
            {
                MovementStateInfo msi = info.GetMovementStateInfo();
                if (time < 1000)
                {
                    Vector3 pos = msi.GetPosition3D();
                    double speed = info.GetActualProperty().MoveSpeed;
                    double distance = (speed * time) / 1000;
                    double len = pos.magnitude;
                    float nz = (float)(z + distance * Math.Cos(dir));
                    float nx = (float)(x + distance * Math.Sin(dir));
                    float dx = nx - pos.x;
                    float dz = nz - pos.z;
                    float distSqr = dx * dx + dz * dz;
                    if (distSqr > 0)
                    {
                        msi.SetPosition2D(x, z);

                        CharacterView view = EntityManager.Instance.GetCharacterViewById(info.GetId());
                        if (null != view)
                        {
                            GfxSystem.UpdateGameObjectLocalPosition2D(view.Actor, x, z);
                        }

                        //LogSystem.Debug("PositionController start, id:{0} dx:{1} dz:{2} time:{3}, just move to pos", id, dx, dz, time);
                    }
                    else
                    {
                        CharacterView view = EntityManager.Instance.GetCharacterViewById(info.GetId());
                        if (null != view)
                        {
                            lock (GfxSystem.SyncLock)
                            {
                                view.ObjectInfo.AdjustDx = dx;
                                view.ObjectInfo.AdjustDz = dz;
                                view.ObjectInfo.CurTime = 0.0f;
                                view.ObjectInfo.TotalTime = 0.5f;
                            }
                        }
                        //LogSystem.Debug("PositionController start, id:{0} dx:{1} dz:{2} time:{3}", id, dx, dz, time);
                    }
                }
                else
                {
                    msi.SetPosition2D(x, z);

                    CharacterView view = EntityManager.Instance.GetCharacterViewById(info.GetId());
                    if (null != view)
                    {
                        GfxSystem.UpdateGameObjectLocalPosition2D(view.Actor, x, z);
                    }

                    //LogSystem.Debug("PositionController just move to pos, id:{0} x:{1} z:{2}", id, x, z);
                }
            }
        }
        public static void AdjustCharacterFaceDir(int id, float faceDir)
        {
            const float c_PI = (float)Math.PI;
            const float c_2PI = (float)Math.PI * 2;
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(id);
            if (null != info)
            {
                float curFaceDir = info.GetMovementStateInfo().GetFaceDir();
                float deltaDir = ((faceDir + c_2PI) - curFaceDir) % c_2PI;
                if (deltaDir > c_PI)
                {
                    deltaDir = c_2PI - deltaDir;
                }
                if (deltaDir > 0.1f)
                {
                    int ctrlId = ControllerIdCalculator.Calc(ControllerType.FaceDir, id);
                    FaceDirController ctrl = s_Helper.FaceControllerPool.Alloc();
                    if (null != ctrl)
                    {
                        ctrl.Init(ctrlId, id, faceDir);
                        s_Helper.System.AddController(ctrl);
                    }
                }
                else
                {
                    info.GetMovementStateInfo().SetFaceDir(faceDir);
                }
            }
        }
        public static void AdjustCharacterMoveDir(int id, float moveDir)
        {
            const float c_PI = (float)Math.PI;
            const float c_2PI = (float)Math.PI * 2;
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(id);
            if (null != info)
            {
                float curMoveDir = info.GetMovementStateInfo().GetMoveDir();
                float deltaDir = ((moveDir + c_2PI) - curMoveDir) % c_2PI;
                if (deltaDir > c_PI)
                {
                    deltaDir = c_2PI - deltaDir;
                }
                if (deltaDir > 0.1f && deltaDir < c_2PI / 8)
                {
                    int ctrlId = ControllerIdCalculator.Calc(ControllerType.MoveDir, id);
                    MoveDirController ctrl = s_Helper.MoveDirControllerPool.Alloc();
                    if (null != ctrl)
                    {
                        ctrl.Init(ctrlId, id, moveDir);
                        s_Helper.System.AddController(ctrl);
                    }
                }
                else
                {
                    info.GetMovementStateInfo().SetMoveDir(moveDir);
                }
            }
        }
        public static void Reset()
        {
            s_Helper.System.Reset();
        }
        public static void Tick()
        {
            s_Helper.System.Tick();
        }

        private sealed class ControlSystemHelper
        {
            internal ObjectPool<FaceDirController> FaceControllerPool
            {
                get { return m_FaceControllerPool; }
            }
            internal ObjectPool<MoveDirController> MoveDirControllerPool
            {
                get { return m_MoveDirControllerPool; }
            }
            internal ControlSystem System
            {
                get { return m_ControlSystem; }
            }

            internal ControlSystemHelper()
            {
                m_FaceControllerPool.Init(128);
                m_MoveDirControllerPool.Init(128);
            }

            private ObjectPool<FaceDirController> m_FaceControllerPool = new ObjectPool<FaceDirController>();
            private ObjectPool<MoveDirController> m_MoveDirControllerPool = new ObjectPool<MoveDirController>();
            private ControlSystem m_ControlSystem = new ControlSystem();
        }

        private static ControlSystemHelper s_Helper = new ControlSystemHelper();
    }
}
