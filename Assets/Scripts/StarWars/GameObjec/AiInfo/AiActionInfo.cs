using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public class AiActionInfo
    {
        public AiActionInfo(AiActionConfig config)
        {
            m_Config = config;
            m_LastTriggerTime = 0;
        }
        public bool IsSatisfy(float dis, float selfHp, float targetHp)
        {
            // distance
            if (m_Config.DisMax < dis || m_Config.DisMin > dis)
            {
                return false;
            }
            // self hp
            if (m_Config.SelfHpMax < selfHp || m_Config.SelfHpMin > selfHp)
            {
                return false;
            }
            // target hp
            if (m_Config.TargetHpMax < targetHp || m_Config.TargetHpMin > targetHp)
            {
                return false;
            }
            // cool down
            if (TimeUtility.GetServerMilliseconds() - m_Config.Cooldown * 1000 < m_LastTriggerTime)
            {
                return false;
            }
            return true;
        }

        public void Trigger()
        {
            m_LastTriggerTime = TimeUtility.GetServerMilliseconds();
        }

        public bool IsFinish()
        {
            return TimeUtility.GetServerMilliseconds() - m_LastTriggerTime > m_Config.LastTime * 1000;
        }
        public AiActionConfig Config
        {
            get { return m_Config; }
        }
        private AiActionConfig m_Config;
        private long m_LastTriggerTime = 0;
    }
}

