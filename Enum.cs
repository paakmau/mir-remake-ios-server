namespace MirRemakeBackend {
    enum CampType : byte {
        SELF,
        FRIEND,
        ENEMY
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
        DODGE_RATE, // 回避率 /100
        LIFE_STEAL, // 吸取生命值 /100
        PHYSICS_VULERNABILITY, // 物理易伤
        MAGIC_VULERNABILITY, // 魔法易伤
        DAMAGE_REDUCTION // 百分比减伤，物理魔法通用，绝对防御 /100
    }
    enum ActorUnitSpecialAttributeType : byte {
        FAINT,
        SILENT,
        IMMOBILE
    }
    enum ActorUnitType : byte {
        PLAYER,
        MONSTER,
        NPC
    }
    public enum GenderType : byte {
        MALE,
        FEMALE
    }
    enum OccupationType : byte {
        NONE = 0,
        WARRIOR = 1,
        ROGUE = 2,
        MAGE = 4,
        TAOIST = 8,
        ALL = WARRIOR | ROGUE | MAGE | TAOIST,
        PHYSICS_OCCUPATION = WARRIOR | ROGUE,
        MAGIC_OCCUPATION = MAGE | TAOIST
    }
    enum MonsterType : byte {
        /// <summary>普通怪物</summary>
        NORMAL,
        /// <summary>精英怪物</summary>
        ELITE,
        /// <summary>普通Boss</summary>
        BOSS,
        /// <summary>最终Boss</summary>
        FINAL_BOSS
    }
    /// <summary>
    /// 技能瞄准类型
    /// 单体或多体由SkillTargetChooser决定
    /// </summary>
    enum SkillAimType : byte {
        /// <summary>指向性圆形</summary>
        AIM_CIRCLE = 1,
        /// <summary>指向性自身出发的矩形</summary>
        AIM_SELF_RECT = 2,
        /// <summary>指向性自身出发的扇形</summary>
        AIM_SELF_SECTOR = 4,
        /// <summary>非指向性圆形</summary>
        NOT_AIM_CIRCLE = 8,
        /// <summary>非指向性自身出发的矩形</summary>
        NOT_AIM_SELF_RECT = 16,
        /// <summary>非指向性自身出发的圆形</summary>
        NOT_AIM_SELF_CIRCLE = 32,
        /// <summary>非指向性自身出发的扇形</summary>
        NOT_AIM_SELF_SECTOR = 64,
        /// <summary>指向性单体</summary>
        AIM_ONE_TARGET = 128,
        ALL = 255,
        AIM = AIM_CIRCLE | AIM_ONE_TARGET | AIM_SELF_RECT | AIM_SELF_SECTOR,
        NOT_AIM = NOT_AIM_CIRCLE | NOT_AIM_SELF_CIRCLE | NOT_AIM_SELF_RECT | NOT_AIM_SELF_SECTOR,
        SELF = AIM_SELF_RECT | AIM_SELF_SECTOR | NOT_AIM_SELF_CIRCLE | NOT_AIM_SELF_RECT | NOT_AIM_SELF_SECTOR,
        NOT_SELF = AIM_CIRCLE | NOT_AIM_CIRCLE | AIM_ONE_TARGET,
        ONE_TARGET = AIM_ONE_TARGET,
        CIRCLE = AIM_CIRCLE | NOT_AIM_CIRCLE | NOT_AIM_SELF_CIRCLE,
        RECT = AIM_SELF_RECT | NOT_AIM_SELF_RECT,
        SECTOR = AIM_SELF_SECTOR | NOT_AIM_SELF_SECTOR
    }
    /// <summary>
    /// 技能瞄准的参数类型
    /// </summary>
    enum SkillAimParamType : byte {
        /// <summary>半径</summary>
        RADIUS,
        /// <summary>角度</summary>
        RADIAN,
        /// <summary>长度</summary>
        LENGTH,
        /// <summary>宽度</summary>
        WIDTH
    }
    enum EffectType : byte {
        PHYSICS,
        MAGIC,
        CONSUMABLE
    }
    enum StatusType : byte {
        CHANGE_HP,
        CHANGE_MP,
        CONCRETE_ATTRIBUTE,
        SPECIAL_ATTRIBUTE
    }
    public enum ItemType : byte {
        EMPTY,
        CONSUMABLE,
        MATERIAL,
        EQUIPMENT,
        GEM,
        ENCHANTMENT
    }
    /// <summary>
    /// 道具所在的位置
    /// </summary>
    enum ItemPlace : byte {
        BAG,
        STORE_HOUSE,
        EQUIPMENT_REGION,
        GROUND
    }
    enum CurrencyType : byte {
        VIRTUAL,
        CHARGE
    }
    enum MissionTargetType : byte {
        KILL_MONSTER,
        GAIN_ITEM,
        LEVEL_UP_SKILL,
        LEVEL_UP,
        TALK_TO_NPC
    }
    enum MissionStatus {
        LOCKED = 0,
        UNLOCKED_BUT_UNACCEPTABLE = 1,
        ACCEPTABLE = 2,
        ACCEPTED = 3
    }
    public enum EquipmentPosition : byte {
        HELMET, //头
        NECKLACE, //项链(颈部)
        SHOULDER, //护肩
        CHEST, //胸甲
        WRISTER, //护腕
        GLOVES, //护手
        GAITER, //腿甲
        BOOT, //鞋
        RING, //戒指
        WEAPON
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
        POOR = 1, // 粗糙
        COMMON = 2, // 普通
        UNCOMMON = 4, // 非凡
        RARE = 8, // 稀有
        EPIC = 16, // 史诗
        LEGENDARY = 32, // 传说
        HEIRLOOM = 64 // 传家宝
    }
    enum ShoppingMallActionType : byte {
        /// <summary>
        /// 消耗类商品
        /// </summary>
        CONSUMABLE,
        /// <summary>
        /// 装备类商品
        /// </summary>
        EQUIPMENT,
        /// <summary>
        /// 材料类商品
        /// </summary>
        MATERIAL,
        /// <summary>
        /// 宝石类商品
        /// </summary>
        GEM,
        /// <summary>
        /// 所有商品
        /// </summary>
        ALL,
        /// <summary>
        /// 活动商品
        /// </summary>
        ACTIVITY,
        /// <summary>
        /// 限时商品
        /// </summary>
        TIME_LIMITED,
        /// <summary>
        /// 一键回收
        /// </summary>
        ONE_PRESS_RECYCLE
    }
    enum ChattingChanelType : byte {
        WORLD,
        PRIVATE,
        SYSTEM
    }
}