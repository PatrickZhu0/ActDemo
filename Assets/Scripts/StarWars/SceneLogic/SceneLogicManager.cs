using System;
using System.Collections.Generic;

namespace StarWars
{
    public sealed class SceneLogicManager
    {
        public ISceneLogic GetSceneLogic(int id)
        {
            ISceneLogic logic = null;
            if (m_SceneLogics.ContainsKey(id))
            {
                logic = m_SceneLogics[id];
            }
            return logic;
        }

        private SceneLogicManager()
        {
            // 在这里初始化所有场景逻辑，并注册到全局字典里。
            //m_SceneLogics.Add((int)SceneLogicId.USER_ENTER_AREA, new SceneLogic_UserEnterArea());
            //m_SceneLogics.Add((int)SceneLogicId.REVIVE_POINT, new SceneLogic_RevivePoint());
            //m_SceneLogics.Add((int)SceneLogicId.TIME_OUT, new SceneLogic_Timeout());
            //m_SceneLogics.Add((int)SceneLogicId.AREA_DETECT, new SceneLogic_AreaDetect());
        }

        private Dictionary<int, ISceneLogic> m_SceneLogics = new Dictionary<int, ISceneLogic>(); // 场景逻辑容器

        #region Singleton
        private static SceneLogicManager s_Instance = new SceneLogicManager();
        public static SceneLogicManager Instance
        {
            get { return s_Instance; }
        }
        #endregion
    }
}
