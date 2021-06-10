/**
 * @file Data_Map.cs
 * @brief 地图数据管理类
 *              负责：
 *                  维护地图数据
 *                  读取及解析
 *                  提供部分API供游戏使用
 *
 * @author lixiaojiang
 * @version 1.0.0
 * @date 2012-12-16
 */

using System;
using System.Collections.Generic;
//using System.Diagnostics;

namespace StarWars
{

    /**
     * @brief 地图数据
     */
    public class MapDataProvider
    {
        //---------------------------------------------------------
        // 属性
        //---------------------------------------------------------    
        /**
         * @brief 单元数据
         */
        public DataDictionaryMgr<Data_Unit> m_UnitMgr;
        public DataDictionaryMgr<SceneLogicConfig> m_SceneLogicMgr;

        /**
         * @brief 构造函数
         */
        public MapDataProvider()
        {
            m_UnitMgr = new DataDictionaryMgr<Data_Unit>();
            m_SceneLogicMgr = new DataDictionaryMgr<SceneLogicConfig>();
        }

        /**
         * @brief 读取数据
         *
         * @param node
         *
         * @return 
         */
        public bool CollectData(DataMap_Type type, string file, string rootLabel)
        {
            bool result = false;
            switch (type)
            {
                case DataMap_Type.DT_Unit:
                    {
                        result = m_UnitMgr.CollectDataFromDBC(file, rootLabel);
                    }
                    break;
                case DataMap_Type.DT_SceneLogic:
                    {
                        result = m_SceneLogicMgr.CollectDataFromDBC(file, rootLabel);
                    }
                    break;
                case DataMap_Type.DT_All:
                case DataMap_Type.DT_None:
                default:
                    {

                    }
                    break;
            }

            return result;
        }


        /**
         * @brief 提取数据
         *
         * @param node
         *
         * @return 
         */
        public IData ExtractData(DataMap_Type type, int id)
        {
            IData result = null;
            switch (type)
            {
                case DataMap_Type.DT_Unit:
                    {
                        result = m_UnitMgr.GetDataById(id);
                    }
                    break;
                case DataMap_Type.DT_SceneLogic:
                    {
                        result = m_SceneLogicMgr.GetDataById(id);
                    }
                    break;
                case DataMap_Type.DT_All:
                case DataMap_Type.DT_None:
                default:
                    {
                        result = null;
                    }
                    break;
            }

            return result;
        }
    }
}

