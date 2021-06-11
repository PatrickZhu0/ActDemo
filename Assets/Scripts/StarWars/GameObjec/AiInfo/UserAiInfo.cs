using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public class AiData_PvpUser_General
    {
        public long Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        public StarWars.AiPathData PatrolPath
        {
            get { return m_PatrolPath; }
        }
        public StarWars.AiPathData FoundPath
        {
            get { return m_FoundPath; }
        }
        public Vector3 EnemyPos
        {
            get { return m_EnemyPos; }
            set { m_EnemyPos = value; }
        }

        private long m_Time = 0;
        private AiPathData m_PatrolPath = new AiPathData();
        private AiPathData m_FoundPath = new AiPathData();
        private Vector3 m_EnemyPos = new Vector3();
    }

    public class AiData_UserSelf_General
    {
        public long Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        public StarWars.AiPathData FoundPath
        {
            get { return m_FoundPath; }
        }
        public Vector3 TergetPos
        {
            get { return m_TergetPos; }
            set { m_TergetPos = value; }
        }
        private long m_Time = 0;
        private AiPathData m_FoundPath = new AiPathData();
        private Vector3 m_TergetPos = new Vector3();
    }
}
