using System.Collections.Generic;

namespace MirRemakeBackend {
    struct DO_Monster{
        public short m_monsterId;
        public short m_level;
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_attrArr;
        public KeyValuePair<short, short>[] m_skillIdAndLevelArr;
        public short[] m_dropItemIdArr;
    }
    struct DO_Skill{
            public short m_skillId;
            public short m_skillLevel;
            public short m_skillMaxLevel;
            public short m_upgradeCharacterLevelInNeed;
            public long m_upgradeMoneyInNeed;
            public int m_upgradeMasterlyInNeed;
            public short[] m_fatherIdArr;
            public short[] m_childrenIdArr;
            public int m_manaCost;
            public float m_singTime;
            public float m_castFrontTime;
            public float m_castBackTime;
            public float m_coolDownTime;
            public SkillAimType m_skillAimType;
            public CampType m_targetCamp;
            public byte m_targetNumber;
            public float m_castRange;
            /// <summary>
            /// 技能伤害判定的范围参数
            /// </summary>
            public KeyValuePair<SkillAimParamType, float>[] m_damageParamArr;
            public DO_Effect m_skillEffect;
        }
    struct DO_Effect{
        public short m_animId;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHP;
        public EffectDeltaHPType m_deltaHPType;
        public int m_deltaMP;
        public EffectDeltaMPType m_deltaMPType;
        public DO_Status[] m_statusAttachArray;
    }
    struct DO_Status{
        public short m_id;
        public StatusType m_type;
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_affectAttributeArr;
        public ActorUnitSpecialAttributeType[] m_specialAttributeArr;
        public float m_value;
        public float m_lastingTime;
    }
    struct DO_Item{
        public short m_id;
        public ItemType m_type;
        public short m_maxNum;
        public ItemQuality m_quality;
        public long m_price;
    }
    struct DO_ConsumableInfo {
        public DO_Effect m_effect;
    }
    struct DO_EquipmentInfo {
        public OccupationType m_occupation;
        public short m_levelInNeed;
        public EquipmentPosition m_position;
        public KeyValuePair<ActorUnitConcreteAttributeType,int>[] m_equipmentAttributeArr;
        public float m_wave;
        public ItemQuality m_quality;
    }
    struct DO_Character{
        public KeyValuePair<ActorUnitMainAttributeType,int>[] m_mainAttributeArr;
        public KeyValuePair<ActorUnitConcreteAttributeType,int>[] m_concreteAttributeArr;
        public short m_giftPointNumber;
        public int m_experienceInNeed;
    }
    struct DO_Mission{
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
        public KeyValuePair<MissionTargetType,KeyValuePair<short,int>>[] m_missionTarget;
        public int m_bonusMoney;
        public int m_bonusExperiences;
        public KeyValuePair<short,short>[] m_bonusItemIdAndNumList;
    }
}