using System;
using System.Collections.Generic;

namespace StarWars
{
    public enum SlotPosition : int
    {
        SP_None = 0,
        SP_A,
        SP_B,
        SP_C,
        SP_D,
    }
    public class SkillTransmitArg
    {
        public SkillTransmitArg()
        {
            SkillId = 0;
            SkillLevel = 0;
        }
        public int SkillId;
        public int SkillLevel;
    }
    public class PresetInfo
    {
        public PresetInfo()
        {
            for (int i = 0; i < PresetNum; i++)
            {
                Presets[i] = SlotPosition.SP_None;
            }
        }
        public void Reset()
        {
            for (int i = 0; i < PresetNum; i++)
            {
                Presets[i] = SlotPosition.SP_None;
            }
        }
        public void SetCurSkillSlotPos(int preset, SlotPosition pos)
        {
            if (preset >= 0 && preset < PresetNum)
            {
                Presets[preset] = pos;
            }
        }
        public const int PresetNum = 4;
        public SlotPosition[] Presets = new SlotPosition[PresetNum];
    }
    public class BreakSection
    {
        public BreakSection(int breaktype, int starttime, int endtime, bool isinterrupt)
        {
            BreakType = breaktype;
            StartTime = starttime;
            EndTime = endtime;
            IsInterrupt = isinterrupt;
        }
        public int BreakType;
        public int StartTime;
        public int EndTime;
        public bool IsInterrupt;
    }

    public class SkillInfo
    {
        public int SkillId;                // 技能Id
        public int SkillLevel;             // 技能等级
        public bool IsSkillActivated;      // 是否正在释放技能    
        public bool IsItemSkill;
        public bool IsMarkToRemove;
        public PresetInfo Postions;         // 技能挂载位置信息
        //public SkillLogicData ConfigData = null;

        public float StartTime;
        public bool IsInterrupted;
        private List<BreakSection> BreakSections = new List<BreakSection>();
        //private Dictionary<SkillCategory, float> m_CategoryLockinputTime = new Dictionary<SkillCategory, float>();
        private float m_CDEndTime;

        //校验数据
        public int m_LeftEnableMoveCount = 0;
        public Dictionary<int, List<int>> m_LeftEnableImpactsToOther = new Dictionary<int, List<int>>();
        public List<int> m_LeftEnableImpactsToMyself = new List<int>();
        public int m_EnableMoveCount = 0;
        public float m_MaxMoveDistanceSqr = 0.0f;
        public List<int> m_EnableImpactsToOther = null;
        public List<int> m_EnableImpactsToMyself = null;

        public SkillInfo(int skillId, int level = 0)
        {
            SkillId = skillId;
            SkillLevel = level;
            IsSkillActivated = false;
            IsItemSkill = false;
            IsMarkToRemove = false;
            IsInterrupted = false;
            Postions = new PresetInfo();
            //ConfigData = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId) as SkillLogicData;
        }

        public virtual void Reset()
        {
            IsSkillActivated = false;
            IsItemSkill = false;
            IsMarkToRemove = false;
            IsInterrupted = false;
            BreakSections.Clear();
            //m_CategoryLockinputTime.Clear();
        }

        public void BeginCD()
        {
            // m_CDEndTime = StartTime + ConfigData.CoolDownTime;
        }

        public void AddCD(float time)
        {
            m_CDEndTime += time;
        }

        public float GetCD(float now)
        {
            return m_CDEndTime - now;
        }

        public bool IsInCd(float now)
        {
            if (now < m_CDEndTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddBreakSection(int breaktype, int starttime, int endtime, bool isinterrupt)
        {
            BreakSection section = new BreakSection(breaktype, starttime, endtime, isinterrupt);
            BreakSections.Add(section);
        }

        //public void AddLockInputTime(SkillCategory category, float lockinputtime)
        //{
        //    if (m_CategoryLockinputTime.ContainsKey(category))
        //    {
        //        m_CategoryLockinputTime[category] = lockinputtime;
        //    }
        //    else
        //    {
        //        m_CategoryLockinputTime.Add(category, lockinputtime);
        //    }
        //}

        //public float GetLockInputTime(SkillCategory category)
        //{
        //    if (m_CategoryLockinputTime.ContainsKey(category))
        //    {
        //        return m_CategoryLockinputTime[category];
        //    }
        //    else
        //    {
        //        return ConfigData.LockInputTime;
        //    }
        //}

        public bool CanBreak(int breaktype, long time, out bool isInterrupt)
        {
            isInterrupt = false;
            if (!IsSkillActivated)
            {
                return true;
            }
            foreach (BreakSection section in BreakSections)
            {
                if (section.BreakType == breaktype &&
                    (StartTime * 1000 + section.StartTime) <= time
                    && time <= (StartTime * 1000 + section.EndTime))
                {
                    isInterrupt = section.IsInterrupt;
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsNull()
        {
            return false;
        }
    }

    public class NullSkillInfo : SkillInfo
    {
        public NullSkillInfo() : base(-1)
        {
            SkillLevel = 0;
        }
        public override bool IsNull()
        {
            return true;
        }
    }
}
