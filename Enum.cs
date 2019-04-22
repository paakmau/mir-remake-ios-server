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
    enum FSMStateType : byte {
        FREE,
        CAST_BEGIN,
        CAST_SING,
        CAST_SING_CANCEL,
        CAST_FRONT,
        CAST_BACK,
        FAINT
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
}