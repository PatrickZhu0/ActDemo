using System;
using System.Collections.Generic;

namespace StarWars
{
    public sealed class SceneLogicSystem
    {
        public void SetSceneLogicInfoManager(SceneLogicInfoManager mgr)
        {
            m_SceneLogicInfoMgr = mgr;
        }
        public SceneLogicInfoManager GetSceneLogicInfoManager()
        {
            return m_SceneLogicInfoMgr;
        }
        public void Reset()
        {
            m_LastTickTime = 0;
        }
        public void Tick()
        {
            if (0 == m_LastTickTime)
            {
                m_LastTickTime = TimeUtility.GetServerMilliseconds();
            }
            else
            {
                long delta = TimeUtility.GetServerMilliseconds() - m_LastTickTime;
                m_LastTickTime = TimeUtility.GetServerMilliseconds();
                for (LinkedListNode<SceneLogicInfo> node = m_SceneLogicInfoMgr.SceneLogicInfos.FirstValue; null != node; node = node.Next)
                {
                    SceneLogicInfo info = node.Value;
                    if (null != info)
                    {
                        ISceneLogic logic = SceneLogicManager.Instance.GetSceneLogic(info.LogicId);
                        if (null != logic)
                        {
                            logic.Execute(info, delta);
                        }
                        if (info.IsLogicFinished)
                        {
                            m_SceneLogicInfos.Add(info);
                        }
                    }
                }
                foreach (SceneLogicInfo info in m_SceneLogicInfos)
                {
                    m_SceneLogicInfoMgr.RemoveSceneLogicInfo(info.GetId());
                }
                m_SceneLogicInfos.Clear();
                m_SceneLogicInfoMgr.ExecuteDelayAdd();
            }
        }

        private long m_LastTickTime = 0;
        private SceneLogicInfoManager m_SceneLogicInfoMgr = null;
        private List<SceneLogicInfo> m_SceneLogicInfos = new List<SceneLogicInfo>();
    }
}
