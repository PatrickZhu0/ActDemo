using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    internal class FaceDirController : AbstractController<FaceDirController>
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
                float faceDir = info.GetMovementStateInfo().GetFaceDir();
                if (m_CurTotalTime >= m_TotalTime || Math.Abs(faceDir - m_FaceDir) <= 0.1f)
                {
                    info.GetMovementStateInfo().SetFaceDir(m_FaceDir);
                    m_IsTerminated = true;
                }
                else
                {
                    float offset = c_PI - (m_FaceDir + c_2PI - faceDir) % c_2PI;
                    if (offset * m_DeltaDir <= 0)
                    {
                        info.GetMovementStateInfo().SetFaceDir(m_FaceDir);
                        m_IsTerminated = true;
                    }
                    else
                    {
                        float newFaceDir = (faceDir + c_2PI + delta * m_DeltaDir / m_TotalTime) % c_2PI;
                        info.GetMovementStateInfo().SetFaceDir(newFaceDir);
                    }
                }
            }
            else
            {
                m_IsTerminated = true;
            }
        }

        public void Init(int id, int objId, float faceDir)
        {
            m_CurTotalTime = 0;
            m_Id = id;
            m_LastTime = TimeUtility.GetLocalMilliseconds();
            m_FaceDir = faceDir;
            m_ObjId = objId;
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(m_ObjId);
            if (null != info)
            {
                float curFaceDir = info.GetMovementStateInfo().GetFaceDir();
                m_DeltaDir = ((faceDir + c_2PI) - curFaceDir) % c_2PI;
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
        }

        private float m_CurTotalTime = 0;
        private float m_LastTime = 0;
        private float m_TotalTime = 0;
        private int m_ObjId = 0;
        private float m_FaceDir = 0;
        private float m_DeltaDir = 0;

        private const float c_TimePerRadian = 1000 / ((float)Math.PI * 4.0f);
        private const float c_PI = (float)Math.PI;
        private const float c_2PI = (float)Math.PI * 2;
    }
}
