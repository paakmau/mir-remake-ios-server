using System.Collections.Generic;
namespace MirRemake {
    enum CampType : byte {
        SELF,
        FRIEND,
        ENEMY
    }
    enum SkillAimType : byte {
        SELF,
        OTHER,
        NOT_AIM
    }
    enum SkillRangeType : byte {
        SECTOR,
        LINE
    }
    enum EffectDeltaHPType : byte {
        PHYSICS,
        MAGIC,
        CONSUMABLE
    }
    enum EffectDeltaMPType : byte {
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
        DELTA_HP_PER_SECOND,
        DELTA_MP_PER_SECOND,
        ATTACK,
        MAGIC,
        DEFENCE,
        RESISTANCE,
        TENACITY,
        SPEED,
        CRITICAL_RATE,
        CRITICAL_BONUS,
        HIT_RATE,
        DODGE_RATE,
        FAINT,
        SILENT,
        IMMOBILE
    }
    /// <summary>
    /// 据说可以减少GC
    /// </summary>
    struct ActorUnitConcreteAttributeTypeComparer : IEqualityComparer<ActorUnitConcreteAttributeType> {
        public bool Equals(ActorUnitConcreteAttributeType a, ActorUnitConcreteAttributeType b) {
            return (byte)a == (byte)b;
        }
        public int GetHashCode(ActorUnitConcreteAttributeType obj) {
            return (int)obj;
        }
    }
    enum ActorUnitType {
        Player,
        Monster,
        NPC
    }
    enum RepositoryType : byte {
        BAG,
        STOREHOUSE,
        EQUIPMENT_LIST
    }
    enum ItemType : byte {
        CONSUMABLE,
        MATERIAL,
        EQUIPMENT
    }
    enum CurrencyType : byte {
        VIRTUAL,
        CHARGE
    }
    enum OccupationType : byte{
        WARRIOR,
        ROGUE,
        MAGE,
        TAOIST
    }    
    enum MassageType : byte {
        GET_ITEM,
        LOSE_ITEM,
        KILL_MONSTER
    }
    enum NPCType : byte {
        NORMAL,
        SHOP
    }
    enum MissionTarget : byte {
        MONSTER_KILLS,
        ITEMS,
        SKILL_LEVEL,
        NPC_INTERACTION
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
    enum MissionOccupation : byte {
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
}