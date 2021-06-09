using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public sealed class SharedGameObjectInfo
    {
        public int m_ActorId = 0;
        public int m_LogicObjectId = 0;
        public int SummonOwnerActorId = -1;
        public int SummonOwnerSkillId = -1;
        public List<int> Summons = new List<int>();
        public bool IsFloat = false;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float sx = 1;
        public float sy = 1;
        public float sz = 1;
        public float Blood = 0;
        public float MaxBlood = 0;
        public float Energy = 0;
        public float MaxEnergy = 0;
        public float Rage = 0;
        public float MaxRage = 0;
        public float VerticlaSpeed = 0;
        public int GfxStateFlag = 0;
        public bool IsDead = false;
        public int AnimConfigId = 0;
        public bool DataChangedByLogic = false;
        public bool DataChangedByGfx = false;
        public bool WantDirChangedByGfx = false;
        public bool IsSkillGfxAnimation = false;
        public bool IsImpactGfxAnimation = false;
        public bool IsSkillGfxMoveControl = false;
        public bool IsImpactGfxMoveControl = false;
        public bool IsSkillGfxRotateControl = false;
        public bool IsImpactGfxRotateControl = false;
        public bool IsGfxAnimation
        {
            get { return IsSkillGfxAnimation || IsImpactGfxAnimation; }
        }
        public bool IsGfxMoveControl
        {
            get { return IsSkillGfxMoveControl || IsImpactGfxMoveControl; }
        }
        public bool IsGfxRotateControl
        {
            get { return IsSkillGfxRotateControl || IsImpactGfxRotateControl; }
        }
        // character type
        public bool IsNpc = false;
        public bool IsPlayer = false;
        public bool IsSuperArmor = false;
        public int CampId = 0;
        public bool IsTouchDown = false;
        public float LastTouchX;
        public float LastTouchY;
        public float LastTouchZ;
        //逻辑层移动控制
        public bool CanHitMove = true;
        public bool IsLogicMoving = false;
        public float MoveCos = 0;
        public float MoveSin = 0;
        public float MoveSpeed = 0;
        public float MoveTargetDistanceSqr = 0;
        public float FaceDir = 0;
        //位置调节数据
        public float AdjustDx = 0;
        public float AdjustDz = 0;
        public float CurTime = 0;
        public float TotalTime = 0;
        //输入朝向控制（仅由逻辑层决定）
        public float WantFaceDir = 0;
        //是否接受受击特效
        public bool AcceptStiffEffect = true;
        //渲染层缓存
        public List<object> m_SkinedOriginalMaterials = new List<object>();
        public bool m_SkinedMaterialChanged;
        public List<object> m_MeshOriginalMaterials = new List<object>();
        public bool m_MeshMaterialChanged;
    }
}
