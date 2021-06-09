using System;
using System.Collections.Generic;

namespace StarWars
{
    public sealed class DataListMgr<TData> where TData : IData, new()
    {
        public bool CollectDataFromDBC(string file, string rootLabel)
        {
            bool result = true;

            DBC document = new DBC();
            document.Load(HomePath.GetAbsolutePath(file));

            for (int index = 0; index < document.RowNum; index++)
            {
                DBC_Row node = document.GetRowByIndex(index);
                if (node != null)
                {
                    TData data = new TData();
                    bool ret = data.CollectDataFromDBC(node);
                    string info = string.Format("DataTableMgr.CollectDataFromDBC collectData Row:{0} failed!", index);
                    //LogSystem.Assert(ret, info);
                    if (ret)
                    {
                        m_DataContainer.Add(data);
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public int GetDataCount()
        {
            return m_DataContainer.Count;
        }

        public List<TData> GetData()
        {
            return m_DataContainer;
        }

        public void Clear()
        {
            m_DataContainer.Clear();
        }

        private List<TData> m_DataContainer = new List<TData>();
    }
}
