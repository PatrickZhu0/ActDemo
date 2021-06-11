using System;
using System.Collections.Generic;

namespace StarWars
{
    /// <summary>
    /// GfxLogic约定的游戏逻辑层通知接口，用于Gfx对游戏逻辑层的事件触发。
    /// 这里未采用PublishSubscribe是出于性能考虑。
    /// </summary>
    public interface IGameLogicNotification
    {
        void OnGfxStartStory(int id);
        void OnGfxSendStoryMessage(string msgId, object[] args);
        void OnGfxControlMoveStart(int objId, int id, bool isSkill);
        void OnGfxControlMoveStop(int objId, int id, bool isSkill);
        void OnGfxHitTarget(int id, int impactId, int targetId, int hitCount, int skillId, int duration, float x, float y, float z, float dir);
        void OnGfxMoveMeetObstacle(int id);
        void OnGfxStartSkill(int id, SkillCategory category, float x, float y, float z);
        void OnGfxForceStartSkill(int id, int skillId);
        void OnGfxStopSkill(int id, int skillId);
        void OnGfxStartAttack(int id, float x, float y, float z);
        void OnGfxStopAttack(int id);
        void OnGfxSkillBreakSection(int id, int skillid, int breaktype, int startime, int endtime, bool isinterrupt);
        void OnGfxStopImpact(int id, int impactId);
        void OnGfxSetCrossFadeTime(int id, string fadeTargetAnim, float crossTime);
        void OnGfxAddLockInputTime(int id, SkillCategory category, float lockinputtime);
        void OnGfxSummonNpc(int owner_id, int owner_skill_id, int npc_type_id, string modelPrefab, int skillid, float x, float y, float z);
        void OnGfxDestroyObj(int id);
        void OnGfxDestroySummonObject(int id);
        void OnGfxSetObjLifeTime(int id, long life_remaint_time);
        void OnGfxSimulateMove(int id);
    }
}
