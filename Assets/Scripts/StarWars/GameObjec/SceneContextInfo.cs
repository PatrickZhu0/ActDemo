using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public delegate void HighlightPromptDelegation(int userId, int dict, object[] args);
    public sealed class SceneContextInfo
    {
        public HighlightPromptDelegation OnHighlightPrompt;

        public StarWarsSpatial.ISpatialSystem SpatialSystem
        {
            get { return m_SpatialSystem; }
            set { m_SpatialSystem = value; }
        }

        //public SightManager SightManager
        //{
        //    get { return m_SightManager; }
        //    set { m_SightManager = value; }
        //}
        public SceneLogicInfoManager SceneLogicInfoManager
        {
            get { return m_SceneLogicInfoManager; }
            set { m_SceneLogicInfoManager = value; }
        }
        //public NpcManager NpcManager
        //{
        //    get { return m_NpcMgr; }
        //    set { m_NpcMgr = value; }
        //}
        public UserManager UserManager
        {
            get { return m_UserMgr; }
            set { m_UserMgr = value; }
        }
        public BlackBoard BlackBoard
        {
            get { return m_BlackBoard; }
            set { m_BlackBoard = value; }
        }
        public int SceneResId
        {
            get { return m_SceneResId; }
            set { m_SceneResId = value; }
        }
        public bool IsRunWithRoomServer
        {
            get { return m_IsRunWithRoomServer; }
            set { m_IsRunWithRoomServer = value; }
        }
        public long StartTime
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }
        public object CustomData
        {
            get { return m_CustomData; }
            set { m_CustomData = value; }
        }
        public CharacterInfo GetCharacterInfoById(int id)
        {
            CharacterInfo info = null;
            //if (null != m_NpcMgr)
            //{
            //    info = m_NpcMgr.GetNpcInfo(id);
            //}
            if (null == info && null != m_UserMgr)
            {
                info = m_UserMgr.GetUserInfo(id);
            }
            return info;
        }
        public CharacterInfo GetCharacterInfoByUnitId(int unitId)
        {
            CharacterInfo info = null;
            //if (null != m_NpcMgr)
            //{
            //    info = m_NpcMgr.GetNpcInfoByUnitId(unitId);
            //}
            return info;
        }
        public void HighlightPromptAll(int dict, params object[] args)
        {
            HighlightPrompt(0, dict, args);
        }
        public void HighlightPrompt(int userId, int dict, params object[] args)
        {
            if (null != OnHighlightPrompt)
            {
                OnHighlightPrompt(userId, dict, args);
            }
        }

        private StarWarsSpatial.ISpatialSystem m_SpatialSystem = null;
        //private SightManager m_SightManager = null;
        private SceneLogicInfoManager m_SceneLogicInfoManager = null;
        //private NpcManager m_NpcMgr = null;
        private UserManager m_UserMgr = null;
        private BlackBoard m_BlackBoard = null;
        private int m_SceneResId = 0;
        private bool m_IsRunWithRoomServer = true;
        private long m_StartTime = 0;
        private object m_CustomData = null;
    }
}
