using System;
using System.Collections.Generic;

namespace MirRemakeBackend {
    enum CampType : byte {
        SELF,
        FRIEND,
        ENEMY
    }
    /// <summary>
    /// 技能瞄准类型
    /// 单体或多体由SkillTargetChooser决定
    /// </summary>
    enum SkillAimType : byte {
        /// <summary>
        /// 指向性圆形
        /// </summary>
        AIM_CIRCLE,
        /// <summary>
        /// 指向性自身出发的扇形
        /// </summary>
        AIM_SELF_SECTOR,
        /// <summary>
        /// 非指向性圆形
        /// </summary>
        NOT_AIM_CIRCLE,
        /// <summary>
        /// 非指向性自身出发的圆形
        /// </summary>
        NOT_AIM_SELF_CIRCLE,
        /// <summary>
        /// 非指向性自身出发的扇形
        /// </summary>
        NOT_AIM_SELF_SECTOR,
        /// <summary>
        /// 非指向性自身出发的矩形
        /// </summary>
        NOT_AIM_SELF_RECT
    }
    /// <summary>
    /// 技能瞄准的参数类型
    /// </summary>
    enum SkillAimParamType : byte {
        /// <summary>
        /// 半径
        /// </summary>
        RADIUS,
        /// <summary>
        /// 角度
        /// </summary>
        RADIAN,
        /// <summary>
        /// 长度
        /// </summary>
        LENGTH,
        /// <summary>
        /// 宽度
        /// </summary>
        WIDTH
    }
    [Obsolete("请使用EffectType代替, 一个Effect只能有一个EffectType")]
    enum EffectDeltaHPType : byte {
        PHYSICS,
        MAGIC,
        CONSUMABLE
    }
    [Obsolete("请使用EffectType代替, 一个Effect只能有一个EffectType")]
    enum EffectDeltaMPType : byte {
        MAGIC,
        CONSUMABLE
    }
    enum EffectType : byte {
        PHYSICS,
        MAGIC,
        CONSUMABLE
    }
    enum StatusType : byte {
        BUFF,
        DEBUFF
    }
    enum ActorUnitMainAttributeType : byte {
        STRENGTH,
        INTELLIGENCE,
        AGILITY,
        SPIRIT
    }
    enum ActorUnitConcreteAttributeType : byte {
        MAX_HP,
        MAX_MP,
        CURRENT_HP,
        CURRENT_MP,
        DELTA_HP_PER_SECOND, // 每秒回复生命值
        DELTA_MP_PER_SECOND, // 每秒回复法力值
        ATTACK, // 物理攻击
        MAGIC, // 法术攻击
        DEFENCE, // 物理防御
        RESISTANCE, // 法术抗性
        TENACITY, // 抵抗能力 /100
        SPEED, // 速度 /100
        CRITICAL_RATE, // 暴击率 /100
        CRITICAL_BONUS, // 暴击伤害 /100
        HIT_RATE, // 命中率 /100
        DODGE_RATE, // 闪避 /100
        LIFE_STEAL, // 吸取生命值 /100
        HP_DAMAGE_PER_SECOND_PHYSICS,
        HP_DAMAGE_PER_SECOND_MAIGC,
        SHIELD,
        PHYSICS_VULERNABILITY, // 物理易伤
        MAGIC_VULERNABILITY, // 魔法易伤
        DAMAGE_REDUCTION // 百分比减伤，物理魔法通用，绝对防御 /100
    }
    enum ActorUnitSpecialAttributeType : byte {
        FAINT,
        SILENT,
        IMMOBILE
    }
    enum ActorUnitType {
        PLAYER,
        MONSTER,
        NPC
    }
    enum ItemType : byte {
        EMPTY,
        CONSUMABLE,
        MATERIAL,
        EQUIPMENT,
        GEM
    }
    enum CurrencyType : byte {
        VIRTUAL,
        CHARGE
    }
    enum OccupationType : byte{
        NONE = 0,
        WARRIOR = 1,
        ROGUE = 2,
        MAGE = 4,
        TAOIST = 8,
        ALL = WARRIOR | ROGUE | MAGE | TAOIST,
        PHYSICS_OCCUPATION = WARRIOR | ROGUE,
        MAGIC_OCCUPATION = MAGE | TAOIST
    }
    enum NpcActionType : byte {
        MISSION_ACCEPT,
        MISSION_DELIVERY,
        MISSION_TALK,
        SHOP,
        FORGE,
        STRENGTHEN,
        ENCHANT
    }
    enum MissionTargetType : byte {
        KILL_MONSTER,
        GAIN_ITEM,
        LEVEL_UP_SKILL,
        TALK_TO_NPC
    }
    enum EquipmentPosition : byte{
        HELMET,//头
        NECKLACE,//项链(颈部)
        SHOULDER,//护肩
        CHEST,//胸甲
        WRISTER,//护腕
        GLOVES,//护手
        GAITER,//腿甲
        BOOT,//鞋
        RING,//戒指
        WEAPON
    }
    enum MissionOccupation : byte{
        WARRIOR,
        ROGUE,
        MAGE,
        TAOIST,
        ALL
    }
    enum BuildingEquipmentFortune : byte {
        // 欧皇
        EUROPE_THE_EMPEROR,
        // 欧洲贵族
        EUROPE_ARISTOCRATS,
        // 欧洲人
        EUROPEAN,
        // 亚洲皇帝
        WINNIE_THE_EMPEROR,
        // 资深元老
        FROG,
        // 阿联酋皇室
        THE_UNITED_ARAB_EMIRATES_ROYAL,
        // 山西煤老板
        SHANXI_COAL_BOSS,
        // 土豪
        LOCAL_TYRANT,
        // 亚洲人
        ASIAN,
        // 非洲人
        AFRICAN,
        // 非酋
        TIGGER
    }
    enum ItemQuality : byte {
        POOR, // 粗糙
        COMMON, // 普通
        UNCOMMON, // 非凡
        RARE, // 稀有
        EPIC, // 史诗
        LEGENDARY, // 传说
        HEIRLOOM // 传家宝
    }
    
}