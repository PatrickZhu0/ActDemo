using System;
using System.Collections.Generic;

namespace StarWars
{
    /**
     * @brief 数值操作类型
     */
    public enum Operate_Type
    {
        // 设置绝对值，直接设置当前值
        OT_Absolute,

        // 设置相对值，即在当前基础上的增加值，可以为负数
        OT_Relative,

        // 设置相对当前值的百分比
        OT_PercentCurrent,

        // 设置相对最大值百分比，[!!!此类操作必须要求改值存在最大值，比如HP，ENERGY, MOVESPEED, SHOOTSPEED]
        OT_PercentMax,

        // 设置位
        OT_AddBit,

        // 取消位
        OT_RemoveBit,
    }

    public enum RelMoveDir
    {
        Forward = 1,
        Backward = 2,
        Leftward = 4,
        Rightward = 8,
        //FL,
        //FR,
        //BL,
        //BR,
    }

    public enum CampIdEnum : int
    {
        Unkown = 0,
        Friendly,
        Hostile,
        Blue,
        Red,
    }

    // 关系
    public enum CharacterRelation : int
    {
        RELATION_INVALID = -1,
        RELATION_ENEMY,             // 敌对
        RELATION_FRIEND,            // 友好
        RELATION_NUMBERS
    };

    public enum GfxCharacterState_Type
    {
        Stiffness = 1 << 0, // 硬直
        HitFly = 1 << 1, //击飞
        KnockDown = 1 << 2, // 倒地
        GetUp = 1 << 3, // 起身
    }
}

