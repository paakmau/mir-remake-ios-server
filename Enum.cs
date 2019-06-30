namespace MirRemakeBackend {
    enum CampType : byte {
        SELF,
        FRIEND,
        ENEMY
    }
    /// <summary>
    /// 技能类型，以备以后扩展用
    /// </summary>
    enum SkillType : byte {

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
    public enum ItemType : byte {
        EMPTY,
        CONSUMABLE,
        MATERIAL,
        EQUIPMENT,
        GEM
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
        POOR, // 粗糙
        COMMON, // 普通
        UNCOMMON, // 非凡
        RARE, // 稀有
        EPIC, // 史诗
        LEGENDARY, // 传说
        HEIRLOOM // 传家宝
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
    public enum DirectionType : byte {
        NORTH = 0,
        NORTH_EAST = 1,
        EAST = 2,
        SOUTH_EAST = 3,
        SOUTH = 4,
        SOUTH_WEST = 5,
        WEST = 6,
        NORTH_WEST = 7,
        NONE = 8
    }
    /// <summary>
    /// 人物状态特效类型
    /// </summary>
    public enum CharacterStateEffectType : byte {
        /// <summary>
        /// 眩晕
        /// </summary>
        DIZZY,
        /// <summary>
        /// 流血
        /// </summary> 
        BLEEDING,
        /// <summary>
        /// 中毒
        /// </summary>
        POISONING,
        /// <summary>
        /// 被点燃
        /// </summary>
        FIRING,
        /// <summary>
        /// 死亡
        /// </summary>
        DEATH,
        /// <summary>
        /// 红buff，持续回血
        /// </summary>
        HP_BUFF,
        /// <summary>
        /// 蓝buff，持续回蓝
        /// </summary>
        MP_BUFF,
    }
    /// <summary>
    /// 文本框样式类型
    /// </summary>
    public enum TextBoxStyleType : byte {
        /// <summary>
        /// 无
        /// </summary>
        NONE,
        /// <summary>
        /// 普通的角色名
        /// </summary>
        CHARACTER_NAME_NORMAL,
        /// <summary>
        /// 高级的角色名
        /// </summary>
        CHARACTER_NAME_HIGH_GRADE,
        /// <summary>
        /// 卓越的角色名
        /// </summary>
        CHARACTER_NAME_BRILLIANT,
        /// <summary>
        /// 普通的角色称号
        /// </summary>
        CHARACTER_TITLE_NORMAL,
        /// <summary>
        /// 高级的角色称号
        /// </summary>
        CHARACTER_TITLE_HIGH_GRADE,
        /// <summary>
        /// 卓越的角色称号
        /// </summary>
        CHARACTER_TITLE_BRILLIANT,
    }
    /// <summary>
    /// 伤害类型
    /// </summary>
    public enum DamageType : byte {
        /// <summary>
        /// 普通伤害
        /// </summary>
        NORMAL,
        /// <summary>
        /// 暴击伤害
        /// </summary>
        CRITICAL,
        /// <summary>
        /// 流血
        /// </summary>
        BLEEDING,
        /// <summary>
        /// 中毒
        /// </summary>
        POISONING,
        /// <summary>
        /// 火烧
        /// </summary>
        FIRING,
    }
    public enum GenderType : byte {
        MALE,
        FEMALE
    }
    public enum OutLookAnimType : byte {
        STAND = 0,
        WALK = 1,
        HIT_0 = 2,
        HIT_1 = 8, //暂时用不到
        HIT_2 = 3,
        RAISE = 4,
        PICK = 5,
        BEEN_HIT = 6,
        DIE = 7,
    }
    enum MissionStatus {
        LOCKED = 0,
        UNLOCKED_BUT_UNACCEPTABLE = 1,
        ACCEPTABLE = 2,
        ACCEPTED = 3
    }
}