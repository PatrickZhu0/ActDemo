
/**
 * @file SceneElements.cs
 * @brief 单场景元素数据
 *              负责：
 *                  场景上的数据格式定义,及其解析方法
 *                  服务器与客户端公用
 *
 * @author 
 * @version 
 * @date 
 */
using System.Configuration;
using System;
using System.Collections.Generic;
//using System.Diagnostics;

namespace StarWars
{
    /**
     * @brief 单元数据
     */
    public class Data_Unit : IData
    {
        public const int c_MaxAiParamNum = 8;
        public const int c_MaxInteractionParamNum = 8;
        // 基础
        public int m_Id;
        public int m_LinkId;
        public int m_CampId;
        public UnityEngine.Vector3 m_Pos;
        public UnityEngine.Vector3 m_Pos2;
        public float m_RotAngle;
        public bool m_IsEnable;
        public List<int> m_IdleAnims;
        public List<int> m_AiActions;

        public int m_AiLogic;
        public string[] m_AiParam = new string[c_MaxAiParamNum];

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
            m_LinkId = DBCUtil.ExtractNumeric<int>(node, "LinkId", 0, true);
            m_CampId = DBCUtil.ExtractNumeric<int>(node, "CampId", 0, true);
            m_Pos = Converter.ConvertVector3D(DBCUtil.ExtractString(node, "Pos", "0.0,0.0,0.0", true));
            m_Pos2 = Converter.ConvertVector3D(DBCUtil.ExtractString(node, "Pos2", "0.0,0.0,0.0", false));
            m_RotAngle = DBCUtil.ExtractNumeric<float>(node, "RotAngle", 0.0f, true) * (float)Math.PI / 180;
            m_IsEnable = DBCUtil.ExtractBool(node, "IsEnable", false, true);
            m_AiLogic = DBCUtil.ExtractNumeric<int>(node, "AiLogic", 0, false);
            m_IdleAnims = DBCUtil.ExtractNumericList<int>(node, "IdleAnimSet", 0, false);
            m_AiActions = DBCUtil.ExtractNumericList<int>(node, "Actions", 0, false);
            for (int i = 0; i < c_MaxAiParamNum; ++i)
            {
                m_AiParam[i] = DBCUtil.ExtractString(node, "AiParam" + i, "", false);
            }

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

        /**
         * @brief 克隆函数
         *
         * @return 
         */
        public object Clone()
        {
            Data_Unit data = new Data_Unit();
            data.m_Id = m_Id;
            data.m_LinkId = m_LinkId;
            data.m_CampId = m_CampId;
            data.m_Pos = new UnityEngine.Vector3(m_Pos.x, m_Pos.y, m_Pos.z);
            data.m_RotAngle = m_RotAngle;

            data.m_IsEnable = m_IsEnable;
            data.m_AiLogic = m_AiLogic;

            for (int i = 0; i < c_MaxAiParamNum; ++i)
            {
                data.m_AiParam[i] = m_AiParam[i];
            }

            return data;
        }
    }
}

