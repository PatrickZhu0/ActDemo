using System;
using System.Collections.Generic;

namespace StarWars
{
    public enum SceneLogicId
    {
        USER_ENTER_AREA = 10001,
        REVIVE_POINT = 10002,
        TIME_OUT = 10003,
        AREA_DETECT = 10004,
    }

    public class SceneLogicInfo
    {
        public int DataId
        {
            get { return m_DataId; }
            set { m_DataId = value; }
        }
        public int GetId()
        {
            return m_Id;
        }
        public int ConfigId
        {
            get
            {
                int id = 0;
                if (null != m_SceneLogicConfig)
                {
                    id = m_SceneLogicConfig.GetId();
                }
                return id;
            }
        }
        public int LogicId
        {
            get
            {
                int id = 0;
                if (null != m_SceneLogicConfig)
                {
                    id = m_SceneLogicConfig.m_LogicId;
                }
                return id;
            }
        }
        public SceneLogicConfig SceneLogicConfig
        {
            get { return m_SceneLogicConfig; }
            set { m_SceneLogicConfig = value; }
        }
        public bool IsLogicFinished
        {
            get { return m_IsLogicFinished; }
            set { m_IsLogicFinished = value; }
        }
        public SceneContextInfo SceneContext
        {
            get { return m_SceneContext; }
            set { m_SceneContext = value; }
        }
        public StarWarsSpatial.ISpatialSystem SpatialSystem
        {
            get
            {
                StarWarsSpatial.ISpatialSystem sys = null;
                if (null != m_SceneContext)
                {
                    sys = m_SceneContext.SpatialSystem;
                }
                return sys;
            }
        }
        public SceneLogicInfoManager SceneLogicInfoManager
        {
            get
            {
                SceneLogicInfoManager mgr = null;
                if (null != m_SceneContext)
                {
                    mgr = m_SceneContext.SceneLogicInfoManager;
                }
                return mgr;
            }
        }

        public UserManager UserManager
        {
            get
            {
                UserManager mgr = null;
                if (null != m_SceneContext)
                {
                    mgr = m_SceneContext.UserManager;
                }
                return mgr;
            }
        }
        public BlackBoard BlackBoard
        {
            get
            {
                BlackBoard blackBoard = null;
                if (null != m_SceneContext)
                {
                    blackBoard = m_SceneContext.BlackBoard;
                }
                return blackBoard;
            }
        }
        public long Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        public TypedDataCollection LogicDatas
        {
            get { return m_LogicDatas; }
        }
        public SceneLogicInfo(int id)
        {
            m_Id = id;
            m_IsLogicFinished = false;
        }
        public void InitId(int id)
        {
            m_Id = id;
        }
        public void Reset()
        {
            m_Time = 0;
            m_IsLogicFinished = false;
            m_LogicDatas.Clear();
        }
        private int m_Id = 0;
        private int m_DataId = 0;
        private SceneLogicConfig m_SceneLogicConfig = null;
        private bool m_IsLogicFinished = false;
        private SceneContextInfo m_SceneContext = null;
        private long m_Time = 0;//由于场景逻辑主要在Tick里工作，通常需要限制工作的频率，这一数据用于此目的（由于LogicDatas的读取比较费，所以抽出来放公共里）
        private TypedDataCollection m_LogicDatas = new TypedDataCollection();
    }
}
