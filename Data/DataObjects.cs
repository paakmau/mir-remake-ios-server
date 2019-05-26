using System;
using System.Collections.Generic;

namespace MirRemakeBackend.Data {
    struct DO_Monster {
        public short m_monsterId;
        public short m_level;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_attrArr;
        public ValueTuple<short, short>[] m_skillIdAndLevelArr;
        public short[] m_dropItemIdArr;
    }
    struct DO_Character {
        public OccupationType m_occupation;
        public short m_level;
        public ValueTuple<ActorUnitMainAttributeType, int>[] m_mainAttributeArr;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_concreteAttributeArr;
        public short m_mainAttrPointNumber;
        public int m_upgradeExperienceInNeed;
    }
    /// <summary>
    /// 一种技能
    /// </summary>
    struct DO_Skill {
        public short m_skillId;
        public short m_skillMaxLevel;
        public short[] m_fatherIdArr;
        public short[] m_childrenIdArr;
        public SkillAimType m_skillAimType;
        public CampType m_targetCamp;
        public DO_SkillData[] m_skillDataAllLevel;
    }
    /// <summary>
    /// 同种技能的不同等级数据
    /// </summary>
    struct DO_SkillData {
        public short m_skillLevel;
        public short m_upgradeCharacterLevelInNeed;
        public long m_upgradeMoneyInNeed;
        public int m_upgradeMasterlyInNeed;
        public int m_mpCost;
        public float m_singTime;
        public float m_castFrontTime;
        public float m_castBackTime;
        public float m_coolDownTime;
        public byte m_targetNumber;
        public float m_castRange;
        /// <summary>
        /// 技能伤害判定的范围参数
        /// </summary>
        public ValueTuple<SkillAimParamType, float>[] m_damageParamArr;
        public DO_Effect m_skillEffect;
    }
    struct DO_Effect {
        public EffectType m_type;
        public short m_animId;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHp;
        public int m_deltaMp;
        public ValueTuple<ActorUnitConcreteAttributeType, float>[] m_attributeArr;
        public ValueTuple<short, float, float>[] m_statusIdAndValueAndTimeArr;
    }
    struct DO_Status {
        public short m_statusId;
        public StatusType m_type;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_affectAttributeArr;
        public ActorUnitSpecialAttributeType[] m_specialAttributeArr;
    }
    struct DO_Item {
        public short m_itemId;
        public ItemType m_type;
        public short m_maxNum;
        public ItemQuality m_quality;
        public long m_price;
    }
    struct DO_Consumable {
        public short m_itemId;
        public DO_Effect m_effect;
    }
    struct DO_Equipment {
        public short m_itemId;
        public OccupationType m_validOccupation;
        public short m_equipLevelInNeed;
        public EquipmentPosition m_equipPosition;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_equipmentAttributeArr;
        public float m_attrWave;
    }
    struct DO_Gem {
        public short m_itemId;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_equipmentAttributeArr;
    }
    struct DO_Mission {
        public short m_id;
        public MissionOccupation m_missionOccupation;
        public string m_title;
        public string m_details;
        public string[] m_conversationsWhenAccepting;
        public string[] m_conversationWhenDelivering;
        public int m_acceptingNPCID;
        public int m_deliveringNPCID;
        public short m_levelInNeed;
        public int[] m_fatherMissionIdArr;
        public int[] m_childrenMissions;
        public ValueTuple<MissionTargetType, ValueTuple<short, int>>[] m_missionTarget;
        public int m_bonusMoney;
        public int m_bonusExperiences;
        public ValueTuple<short, short>[] m_bonusItemIdAndNumList;
    }
}