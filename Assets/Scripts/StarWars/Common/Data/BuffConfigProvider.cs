using System;
using System.Collections.Generic;

namespace StarWars
{
    public class BuffConfig : IData
    {
        public int m_Id;
        public AttrDataConfig m_AttrData = new AttrDataConfig();

        /**
         * @brief 提取数据
         *
         * @param node
         *
         * @return 
         */
        public bool CollectDataFromDBC(DBC_Row node)
        {
            m_Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
            m_AttrData.CollectDataFromDBC(node);

            return true;
        }

        /**
         * @brief 获取数据ID
         *
         * @return 
         */
        public int GetId()
        {
            return m_Id;
        }

    }

    public class BuffConfigProvider
    {

        public DataDictionaryMgr<BuffConfig> BuffConfigMgr
        {
            get { return m_BuffConfigMgr; }
        }
        public BuffConfig GetDataById(int id)
        {
            return m_BuffConfigMgr.GetDataById(id);
        }
        public void Load(string file, string root)
        {
            m_BuffConfigMgr.CollectDataFromDBC(file, root);
        }

        private DataDictionaryMgr<BuffConfig> m_BuffConfigMgr = new DataDictionaryMgr<BuffConfig>();

        public static BuffConfigProvider Instance
        {
            get { return s_Instance; }
        }
        private static BuffConfigProvider s_Instance = new BuffConfigProvider();
    }
}
