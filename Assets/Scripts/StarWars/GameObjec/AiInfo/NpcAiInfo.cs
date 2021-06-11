using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
  public class AiData_ForMoveCommand
  {
    public List<Vector3> WayPoints { get; set; }
    public int Index { get; set; }
    public bool IsFinish { get; set; }
    public float EstimateFinishTime { get; set; }

    public AiData_ForMoveCommand(List<Vector3> way_points)
    {
      WayPoints = way_points;
      Index = 0;
      EstimateFinishTime = 0;
      IsFinish = false;
    }
  }
  public class AiData_ForPatrol
  {
    public bool IsLoopPatrol
    {
      get { return m_IsLoopPatrol; }
      set { m_IsLoopPatrol = value; }
    }
    public StarWars.AiPathData PatrolPath
    {
      get { return m_PatrolPath; }
    }
    public StarWars.AiPathData FoundPath
    {
      get { return m_FoundPath; }
    }

    private AiPathData m_PatrolPath = new AiPathData();
    private AiPathData m_FoundPath = new AiPathData();
    private bool m_IsLoopPatrol = false;
  }
  public class AiData_ForPatrolCommand
  {
    public bool IsLoopPatrol
    {
      get { return m_IsLoopPatrol; }
      set { m_IsLoopPatrol = value; }
    }
    public StarWars.AiPathData PatrolPath
    {
      get { return m_PatrolPath; }
    }
    public StarWars.AiPathData FoundPath
    {
      get { return m_FoundPath; }
    }

    private AiPathData m_PatrolPath = new AiPathData();
    private AiPathData m_FoundPath = new AiPathData();
    private bool m_IsLoopPatrol = false;
  }
  public class AiData_ForPursuitCommand
  {
    public StarWars.AiPathData FoundPath
    {
      get { return m_FoundPath; }
    }

    private AiPathData m_FoundPath = new AiPathData();
  }
  public class AiData_PveNpc_Trap
  {
    public bool IsTriggered
    {
      get { return m_IsTriggered; }
      set { m_IsTriggered = value; }
    }
    public float RadiusOfTrigger
    {
      get { return m_RadiusOfTrigger; }
      set { m_RadiusOfTrigger = value; }
    }
    public float RadiusOfDamage
    {
      get { return m_RadiusOfDamage; }
      set { m_RadiusOfDamage = value; }
    }
    public int DamageCount
    {
      get { return m_DamageCount; }
      set { m_DamageCount = value; }
    }
    public int ImpactToMyself
    {
      get { return m_ImpactToMyself; }
      set { m_ImpactToMyself = value; }
    }
    public int Impact1ToTarget
    {
      get { return m_Impact1ToTarget; }
      set { m_Impact1ToTarget = value; }
    }
    public int Impact2ToTarget
    {
      get { return m_Impact2ToTarget; }
      set { m_Impact2ToTarget = value; }
    }
    public int HideImpact
    {
      get { return m_HideImpact; }
      set { m_HideImpact = value; }
    }

    private bool m_IsTriggered = false;
    private float m_RadiusOfTrigger = 2;
    private float m_RadiusOfDamage = 4;
    private int m_DamageCount = 1;
    private int m_ImpactToMyself = 10504;
    private int m_Impact1ToTarget = 10502;
    private int m_Impact2ToTarget = 10503;
    private int m_HideImpact = 10514;
  }
  public class AiData_PveNpc_General
  {
    public long Time
    {
      get { return m_Time; }
      set { m_Time = value; }
    }
    public long FindPathTime
    {
      get { return m_FindPathTime; }
      set { m_FindPathTime = value; }
    }
    public StarWars.AiPathData FoundPath
    {
      get { return m_FoundPath; }
    }

    private long m_Time = 0;
    private long m_FindPathTime = 0;
    private AiPathData m_FoundPath = new AiPathData();
  }
  public class AiData_PveNpc_Monster : AiData_PveNpc_General
  {
    public int Skill
    {
      get { return m_Skill; }
      set { m_Skill = value; }
    }
    public int ShootDistance
    {
      get { return m_ShootDistance; }
      set { m_ShootDistance = value; }
    }

    private int m_Skill = 10165;//飞扑
    private int m_ShootDistance = 5;
  }
  public class AiData_PveNpc_Monster_CloseCombat : AiData_PveNpc_General
  {
    public int FastMoveImpact
    {
      get { return m_Impact1; }
      set { m_Impact1 = value; }
    }
    public int PreAttackImpact
    {
      get { return m_Impact2; }
      set { m_Impact2 = value; }
    }
    public int PreAttackDistance
    {
      get { return m_PreAttackDistance; }
      set { m_PreAttackDistance = value; }
    }
    public int StandShootDistance
    {
      get { return m_StandShootDistance; }
      set { m_StandShootDistance = value; }
    }

    public int TestFlag { set; get; }

    private int m_Impact1 = 20020;//战斗移动，加速
    private int m_Impact2 = 20022;//预热
    private int m_PreAttackDistance = 15;
    private int m_StandShootDistance = 3;
  }
  public class AiData_PveNpc_RadioactiveMan : AiData_PveNpc_General
  {
    public long LeftAdjustMoveTime
    {
      get { return m_LeftAdjustMoveTime; }
      set { m_LeftAdjustMoveTime = value; }
    }
    public Vector3 AdjustMoveTarget
    {
      get { return m_AdjustMoveTarget; }
      set { m_AdjustMoveTarget = value; }
    }
    public int Skill
    {
      get { return m_Skill; }
      set { m_Skill = value; }
    }
    public int Impact
    {
      get { return m_Impact; }
      set { m_Impact = value; }
    }
    public int MinDistanceSquareToSkill
    {
      get { return m_MinDistanceSquareToSkill; }
      set { m_MinDistanceSquareToSkill = value; }
    }
    public int MinHpToImpact
    {
      get { return m_MinHpToImpact; }
      set { m_MinHpToImpact = value; }
    }
    public int MoveProbability
    {
      get { return m_MoveProbability; }
      set { m_MoveProbability = value; }
    }

    private long m_LeftAdjustMoveTime = 0;
    private Vector3 m_AdjustMoveTarget = new Vector3();
    private int m_Skill = 10167;//近身击退
    private int m_Impact = 52;//暴怒
    private int m_MinDistanceSquareToSkill = 4;
    private int m_MinHpToImpact = 10;
    private int m_MoveProbability = 60;
  }
  public class AiData_PveNpc_Soldier : AiData_PveNpc_General
  {
    public int Impact
    {
      get { return m_Impact; }
      set { m_Impact = value; }
    }
    public int StandShootDistance
    {
      get { return m_StandShootDistance; }
      set { m_StandShootDistance = value; }
    }

    private int m_Impact = 20021;//战斗移动，减速
    private int m_StandShootDistance = 8;
  }
  public class AiData_PveBoss_UcaCommander : AiData_PveNpc_General
  {
    public const int c_FarDistance = 20;
    public const int c_NearDistance = 10;
    public int Skill
    {
      get { return m_Skill; }
      set { m_Skill = value; }
    }
    public int MaxCount
    {
      get { return m_MaxCount; }
      set { m_MaxCount = value; }
    }
    public int CurCount
    {
      get { return m_CurCount; }
      set { m_CurCount = value; }
    }

    private int m_Skill = 0;
    private int m_MaxCount = 0;
    private int m_CurCount = 0;
  }
  public class AiData_PvpNpc_General
  {
    public long Time
    {
      get { return m_Time; }
      set { m_Time = value; }
    }
    public long ThinkingTime
    {
      get { return m_ThinkingTime; }
      set { m_ThinkingTime = value; }
    }
    public StarWars.AiPathData PatrolPath
    {
      get { return m_PatrolPath; }
    }
    public StarWars.AiPathData FoundPath
    {
      get { return m_FoundPath; }
    }

    private long m_ThinkingTime = 0;
    private long m_Time = 0;
    private AiPathData m_PatrolPath = new AiPathData();
    private AiPathData m_FoundPath = new AiPathData();
  }

  public class AiData_Npc_Bluelf : AiData_PveNpc_General {

    public long MeetEnemyWalkTime {
      get { return m_MeetEnemyWalkTime; }
      set { m_MeetEnemyWalkTime = value; }
    }
    public long ChaseWalkTime {
      get { return m_ChaseWalkTime; }
      set { m_ChaseWalkTime = value; }
    }
    public long ChaseStandTime {
      get { return m_ChaseStandTime; }
      set { m_ChaseStandTime = value; }
    }
    public long WaitTime {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }

    public bool HasMeetEnemy {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }
    public long TauntTime {
      get { return m_TauntTime; }
      set { m_TauntTime = value; }
    }

    public int SkillToCast {
      get { return m_SkillToCast; }
      set { m_SkillToCast = value; }
    }

    public bool HasPatrolData {
      get { return m_HasPatrolData; }
      set { m_HasPatrolData = value; }
    }

    public long LastUseSkillTime {
      get { return m_LastUseSkillTime; }
      set { m_LastUseSkillTime = value; }
    }
    public int CurAiAction {
      get { return m_CurAiAction; }
      set { m_CurAiAction = value; }
    }
    public long EscapeTime {
      get { return m_EscapeTime; }
      set { m_EscapeTime = value; }
    }
    public long ControlTime
    {
      get { return m_ControlTime; }
      set { m_ControlTime = value; }
    }
    private bool m_HasMeetEnemy = false;
    private long m_MeetEnemyWalkTime = 0;
    private long m_ChaseWalkTime = 0;
    private long m_ChaseStandTime = 0;
    private long m_TauntTime = 0;
    private long m_WaitTime = 0;
    private int m_SkillToCast = 0;
    private bool m_HasPatrolData = false;
    private long m_LastUseSkillTime = 0;
    private int m_CurAiAction = 0;
    private long m_EscapeTime = 0;
    private long m_ControlTime = 0;
  }

  public class AiData_Demo_Melee : AiData_PveNpc_General {

    public long WalkTime {
      get { return m_WalkTime; }
      set { m_WalkTime = value; }
    }

    public long WaitTime {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }

    public bool HasMeetEnemy {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }

    public int SkillToCast {
      get { return m_SkillToCast; }
      set { m_SkillToCast = value; }
    }

    public bool HasPatrolData {
      get { return m_HasPatrolData; }
      set { m_HasPatrolData = value; }
    }

    public Animation_Type MeetEnemyAnim {
      get { return m_MeetEnemyAnim; }
      set { m_MeetEnemyAnim = value; }
    }

    public long LastUseSkillTime {
      get { return m_LastUseSkillTime; }
      set { m_LastUseSkillTime = value; }
    }
    private bool m_HasMeetEnemy = false;
    private Animation_Type m_MeetEnemyAnim = Animation_Type.AT_None;
    private long m_WalkTime = 0;
    private long m_WaitTime = 0;
    private int m_SkillToCast = 0;
    private bool m_HasPatrolData = false;
    private long m_LastUseSkillTime = 0;
  }
  public class AiData_Npc_CommonMelee : AiData_PveNpc_General {
    public long MeetEnemyWalkTime {
      get { return m_MeetEnemyWalkTime; }
      set { m_MeetEnemyWalkTime = value; }
    }
    public long ChaseWalkTime {
      get { return m_ChaseWalkTime; }
      set { m_ChaseWalkTime = value; }
    }
    public long ChaseStandTime {
      get { return m_ChaseStandTime; }
      set { m_ChaseStandTime = value; }
    }
    public long WaitTime {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }

    public bool HasMeetEnemy {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }
    public long TauntTime {
      get { return m_TauntTime; }
      set { m_TauntTime = value; }
    }

    public int SkillToCast {
      get { return m_SkillToCast; }
      set { m_SkillToCast = value; }
    }

    public bool HasPatrolData {
      get { return m_HasPatrolData; }
      set { m_HasPatrolData = value; }
    }

    public long LastUseSkillTime {
      get { return m_LastUseSkillTime; }
      set { m_LastUseSkillTime = value; }
    }
    public int CurAiAction {
      get { return m_CurAiAction; }
      set { m_CurAiAction = value; }
    }
    public long EscapeTime {
      get { return m_EscapeTime; }
      set { m_EscapeTime = value; }
    }
    public long ControlTime
    {
      get { return m_ControlTime; }
      set { m_ControlTime = value; }
    }
    private bool m_HasMeetEnemy = false;
    private long m_MeetEnemyWalkTime = 0;
    private long m_ChaseWalkTime = 0;
    private long m_ChaseStandTime = 0;
    private long m_TauntTime = 0;
    private long m_WaitTime = 0;
    private int m_SkillToCast = 0;
    private bool m_HasPatrolData = false;
    private long m_LastUseSkillTime = 0;
    private int m_CurAiAction = 0;
    private long m_EscapeTime = 0;
    private long m_ControlTime = 0;
  }
  public class AiData_Npc_CommonRange : AiData_PveNpc_General {
    public long MeetEnemyWalkTime {
      get { return m_MeetEnemyWalkTime; }
      set { m_MeetEnemyWalkTime = value; }
    }
    public long ChaseWalkTime {
      get { return m_ChaseWalkTime; }
      set { m_ChaseWalkTime = value; }
    }
    public long ChaseStandTime {
      get { return m_ChaseStandTime; }
      set { m_ChaseStandTime = value; }
    }
    public long WaitTime {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }

    public bool HasMeetEnemy {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }
    public long TauntTime {
      get { return m_TauntTime; }
      set { m_TauntTime = value; }
    }

    public int SkillToCast {
      get { return m_SkillToCast; }
      set { m_SkillToCast = value; }
    }

    public bool HasPatrolData {
      get { return m_HasPatrolData; }
      set { m_HasPatrolData = value; }
    }

    public long LastUseSkillTime {
      get { return m_LastUseSkillTime; }
      set { m_LastUseSkillTime = value; }
    }
    public int CurAiAction {
      get { return m_CurAiAction; }
      set { m_CurAiAction = value; }
    }
    public long EscapeTime {
      get { return m_EscapeTime; }
      set { m_EscapeTime = value; }
    }
    private bool m_HasMeetEnemy = false;
    private long m_MeetEnemyWalkTime = 0;
    private long m_ChaseWalkTime = 0;
    private long m_ChaseStandTime = 0;
    private long m_TauntTime = 0;
    private long m_WaitTime = 0;
    private int m_SkillToCast = 0;
    private bool m_HasPatrolData = false;
    private long m_LastUseSkillTime = 0;
    private int m_CurAiAction = 0;
    private long m_EscapeTime = 0;
  }
  public class AiData_Npc_CommonBoss : AiData_PveNpc_General {
    public long WaitTime {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }

    public bool HasMeetEnemy {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }

    public int SkillToCast {
      get { return m_SkillToCast; }
      set { m_SkillToCast = value; }
    }

    public bool HasPatrolData {
      get { return m_HasPatrolData; }
      set { m_HasPatrolData = value; }
    }

    public long LastUseSkillTime {
      get { return m_LastUseSkillTime; }
      set { m_LastUseSkillTime = value; }
    }
    public long ControlTime
    {
      get { return m_ControlTime; }
      set { m_ControlTime = value; }
    }
    private bool m_HasMeetEnemy = false;
    private long m_WaitTime = 0;
    private int m_SkillToCast = 0;
    private bool m_HasPatrolData = false;
    private long m_LastUseSkillTime = 0;
    private long m_ControlTime = 0;
  }
  public class AiData_Npc_BossXiLie : AiData_PveNpc_General {
    public long WaitTime
    {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }
    public bool HasMeetEnemy
    {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }
    public long ControlTime
    {
      get { return m_ControlTime; }
      set { m_ControlTime = value; }
    }
    public int RangeSkill
    {
      get
      {
        int index = Helper.Random.Next(m_RangeSkills.Length);
        return m_RangeSkills[index];
      }
    }
    public int MeeleSkill
    {
      get
      {
        int index = Helper.Random.Next(m_MeeleSkills.Length);
        return m_MeeleSkills[index];
      }
    }
    public int[] MeeleCombo
    {
      get 
      {
        int index = Helper.Random.Next(2);
        if (index > 0) {
          return m_MeeleCombo;
        } else {
          return m_MeeleComboPlus;
        }
      }
    }
    public int[] DecontrolCombo
    {
      get { return m_DecontrolCombo; }
    }
    public int[] StandCombo
    {
      get { return m_StandCombo; }
    }
    public int[] FlyCombo
    {
      get { return m_FlyCombo; }
    }
    public int DecontrolSkill
    {
      get { return m_DecontrolSkill; }
    }
    public bool IsInDecontrolCombo
    {
      get { return m_IsInDecontrolCombo; }
      set { m_IsInDecontrolCombo = value; }
    }
    public int[] CurSkillCombo
    {
      get { return m_CurSkillCombo; }
      set { m_CurSkillCombo = value; }
    }
    public int CurSkillComboIndex
    {
      get { return m_CurSkillComboIndex; }
      set { m_CurSkillComboIndex = value; }
    }
    private long m_WaitTime = 0;
    private bool m_HasMeetEnemy = false;
    private long m_ControlTime = 0;
    private int[] m_RangeSkills = { 380207, 380205, 380203, 380209 };
    private int[] m_MeeleSkills = { 380201, 380203, 380205, 380207, 380209, 380210};
    private int[] m_MeeleCombo = { 380209, 380210, 380201, 380207 };
    private int[] m_MeeleComboPlus = { 380201, 380207, 380210 };
    private int[] m_DecontrolCombo = { 380205, 380207, 380209 };
    private int[] m_StandCombo = { 380201, 380210, 380207, 380210 };
    private int[] m_FlyCombo = { 380210, 380207, 380210 };
    private int[] m_CurSkillCombo;
    private bool m_IsInDecontrolCombo = false;
    private int m_CurSkillComboIndex;
    private int m_DecontrolSkill = 380202;
  }
  public class AiData_Npc_BossDevilWarrior : AiData_PveNpc_General {
    public long WaitTime
    {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }
    public bool HasMeetEnemy
    {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }
    public long ControlTime
    {
      get { return m_ControlTime; }
      set { m_ControlTime = value; }
    }
    public int RangeSkill
    {
      get
      {
        int index = Helper.Random.Next(m_RangeSkills.Length);
        return m_RangeSkills[index];
      }
    }
    public int FlyRangeSkill
    {
      get
      {
        int index = Helper.Random.Next(m_RangeSkills.Length);
        return m_RangeSkills[index];
      }
    }

    public int FlyMeeleSkill
    {
      get{ return m_MeeleFlySkill;}
    }
    public int MeeleSkill
    {
      get
      {
        int index = Helper.Random.Next(10);
        if (index < 7) {
          return m_MeeleSkill;
        } else {
          index = Helper.Random.Next(m_MeelePlus.Length);
          return m_MeelePlus[index];
        }
      }
    }
    public int[] DecontrolCombo
    {
      get { return m_DecontrolCombo; }
    }
    public int DecontrolSkill
    {
      get { return m_DecontrolSkill; }
    }
    public int[] CurSkillCombo
    {
      get { return m_CurSkillCombo; }
      set { m_CurSkillCombo = value; }
    }
    public int CurSkillComboIndex
    {
      get { return m_CurSkillComboIndex; }
      set { m_CurSkillComboIndex = value; }
    }
    private long m_WaitTime = 0;
    private bool m_HasMeetEnemy = false;
    private long m_ControlTime = 0;
    private int m_RangeFlySkill =  380105;
    private int m_MeeleFlySkill =  380103;
    private int[] m_RangeSkills = { 380102, 380103};
    private int m_MeeleSkill = 380104;
    private int[] m_MeelePlus = { 380101, 380103 };
    private int[] m_DecontrolCombo = { 380102, 380103, 380105, 380101 };
    private int[] m_CurSkillCombo;
    private int m_CurSkillComboIndex;
    private int m_DecontrolSkill = 380106;
  }

  public class AiData_Npc_BossHulun : AiData_PveNpc_General
  { 
    public long WaitTime
    {
      get { return m_WaitTime; }
      set { m_WaitTime = value; }
    }
    public bool HasMeetEnemy
    {
      get { return m_HasMeetEnemy; }
      set { m_HasMeetEnemy = value; }
    }
    public int[] m_StageOneSkillCombo
    {
      get
      {
        int[] result = new int[3];
        result[0] = m_StageOneFirst[Helper.Random.Next(m_StageOneFirst.Length)];
        result[1] = m_StageOneSecond[Helper.Random.Next(m_StageOneSecond.Length)];
        result[2] = m_StageOneThird[Helper.Random.Next(m_StageOneThird.Length)];
        return result;
      }
    }
    public int[] m_StageTwoSkillCombo
    {
      get
      {
        int[] result = new int[3];
        result[0] = m_StageTwoFirst[Helper.Random.Next(m_StageTwoFirst.Length)];
        result[1] = m_StageTwoSecond[Helper.Random.Next(m_StageTwoSecond.Length)];
        result[2] = m_StageTwoThird[Helper.Random.Next(m_StageTwoThird.Length)];
        return result;
      }
    }
    public int[] m_StageThreeSkillCombo
    {
      get
      {
        int[] result = new int[1];
        result[0] = m_StageThree[Helper.Random.Next(m_StageThree.Length)];
        return result;
      }
    }
    public int CurStage
    {
      get { return m_CurStage; }
      set { m_CurStage = value; }
    }
    public float StageTwoLimit
    {
      get { return m_StageTwoLimit; }
    }
    public int EnterStageTwoSkill
    {
      get { return m_EnterStageTwoSkill; }
      set { m_EnterStageTwoSkill = value; }
    }
    public float StageThreeLimit
    {
      get { return m_StageThreeLimit; }
    }
    public int EnterStageThreeSkill
    {
      get { return m_EnterStageThreeSkill; }
      set { m_EnterStageThreeSkill = value; }
    }
    public int[] CurSkillCombo
    {
      get { return m_CurSkillCombo; }
      set { m_CurSkillCombo = value; }
    }
    public int CurSkillComboIndex
    {
      get { return m_CurSkillComboIndex; }
      set { m_CurSkillComboIndex = value; }
    }
    public int ChaseTargetSkill
    {
      get { return m_ChaseTargetSkill; }
    }
    public bool IsUsingChaseSkill
    {
      get { return m_IsUsingChaseSkill; }
      set { m_IsUsingChaseSkill = value; }
    }
    public long ControlTime
    {
      get { return m_ControlTime; }
      set { m_ControlTime = value; }
    }
    private int[] m_CurSkillCombo = null;
    private int m_CurSkillComboIndex = 0;
    private int[] m_StageOneFirst = { 380301, 380302, 380303 };
    private int[] m_StageOneSecond = { 380307, 380301, 380302, 380303 };
    private int[] m_StageOneThird = { 380308 };
    private float m_StageTwoLimit = 0.6f;
    private int m_EnterStageTwoSkill = 380304;
    private int[] m_StageTwoFirst = { 380301, 380302, 380303 };
    private int[] m_StageTwoSecond = { 380305 };
    private int[] m_StageTwoThird = { 380306 };
    private int[] m_StageThree = {380303, 380305, 380306, 380307, 380308};
    private float m_StageThreeLimit = 0.2f;
    private int m_EnterStageThreeSkill = 380304;
    private int m_ChaseTargetSkill = 380306;
    private bool m_IsUsingChaseSkill = false;
    private int m_CurStage = 1;
    private long m_WaitTime = 0;
    private bool m_HasMeetEnemy = false;
    private long m_ControlTime = 0;
  }

}
