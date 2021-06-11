using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    internal class MoveDirController : AbstractController<MoveDirController>
    {
        public override void Adjust()
        {
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(m_ObjId);
            if (null != info)
            {
                float curTime = TimeUtility.GetLocalMilliseconds();
                float delta = curTime - m_LastTime;
                m_LastTime = curTime;
                m_CurTotalTime += delta;
                float moveDir = info.GetMovementStateInfo().GetMoveDir();
                if (m_CurTotalTime >= m_TotalTime || Math.Abs(moveDir - m_MoveDir) <= 0.1f)
                {
                    info.GetMovementStateInfo().SetMoveDir(m_MoveDir);

                    //GfxSystem.GfxLog("MoveDir adjust {0}", m_MoveDir);

                    m_IsTerminated = true;
                }
                else
                {
                    float offset = c_PI - (m_MoveDir + c_2PI - moveDir) % c_2PI;
                    if (offset * m_DeltaDir <= 0)
                    {
                        info.GetMovementStateInfo().SetMoveDir(m_MoveDir);

                        //GfxSystem.GfxLog("MoveDir adjust {0}", m_MoveDir);

                        m_IsTerminated = true;
                    }
                    else
                    {
                        float newMoveDir = (moveDir + c_2PI + delta * m_DeltaDir / m_TotalTime) % c_2PI;
                        info.GetMovementStateInfo().SetMoveDir(newMoveDir);

                        //GfxSystem.GfxLog("MoveDirController {0}, obj:{1}, moveDir:{2}->{3}, delta:{4} totalTime:{5} deltaDir:{6} targetDir:{7}", m_Id, m_ObjId, moveDir, newMoveDir, delta, m_TotalTime, m_DeltaDir, m_MoveDir);
                    }
                }
            }
            else
            {
                m_IsTerminated = true;
            }
        }

        public void Init(int id, int objId, float moveDir)
        {
            m_CurTotalTime = 0;
            m_Id = id;
            m_LastTime = TimeUtility.GetLocalMilliseconds();
            m_MoveDir = moveDir;
            m_ObjId = objId;
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(m_ObjId);
            if (null != info)
            {
                float curMoveDir = info.GetMovementStateInfo().GetMoveDir();
                m_DeltaDir = ((moveDir + c_2PI) - curMoveDir) % c_2PI;
                if (m_DeltaDir > c_PI)
                {
                    m_DeltaDir -= c_2PI;
                    m_TotalTime = -m_DeltaDir * c_TimePerRadian;
                }
                else
                {
                    m_TotalTime = m_DeltaDir * c_TimePerRadian;
                }
            }
            else
            {
                m_TotalTime = 0;
                m_DeltaDir = 0;
            }

            //GfxSystem.GfxLog("MoveDirController {0}, obj:{1}, moveDir:{2} deltaDir:{3} totalTime:{4} isTerminated:{5}", id, objId, moveDir, m_DeltaDir, m_TotalTime, m_IsTerminated);
        }

        private float m_CurTotalTime = 0;
        private float m_LastTime = 0;
        private float m_TotalTime = 0;
        private int m_ObjId = 0;
        private float m_MoveDir = 0;
        private float m_DeltaDir = 0;

        private const float c_TimePerRadian = 1000 / ((float)Math.PI * 8.0f);
        private const float c_PI = (float)Math.PI;
        private const float c_2PI = (float)Math.PI * 2;
    }
}
