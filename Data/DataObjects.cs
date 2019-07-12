using System;
namespace MirRemakeBackend.Data {
    struct DO_Monster {
        public short m_monsterId;
        public MonsterType m_monsterType;
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
        public OccupationType m_occupation;
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
        public float m_castFrontTime;
        public float m_castBackTime;
        public float m_coolDownTime;
        public byte m_targetNumber;
        public float m_castRange;
        /// <summary>
        /// 技能伤害判定的范围参数
        /// </summary>
        public ValueTuple<SkillAimParamType, float>[] m_damageParamArr;
        public float m_secondParameter;
        public DO_Effect m_skillEffect;
    }
    struct DO_Effect {
        public EffectType m_type;
        public short m_animId;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHp;
        public int m_deltaMp;
        public ValueTuple<ActorUnitConcreteAttributeType, float>[] m_attributeBonusArr;
        public ValueTuple<short, float, float>[] m_statusIdAndValueAndTimeArr;
    }
    struct DO_Status {
        public short m_statusId;
        public StatusType m_type;
        public bool m_isBuff;
    }
    struct DO_ConcreteAttributeStatus {
        public (ActorUnitConcreteAttributeType, int) [] m_conAttrArr;
    }
    struct DO_SpecialAttributeStatus {
        public ActorUnitSpecialAttributeType m_spAttr;
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
        public ItemQuality m_quality;
    }
    struct DO_Gem {
        public short m_itemId;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_equipmentAttributeArr;
    }
    struct DO_Mission {
        public short m_id;
        public OccupationType m_missionOccupation;
        public short m_levelInNeed;
        public short[] m_fatherMissionIdArr;
        public short[] m_childrenMissionArr;
        public int m_acceptingNPCID;
        public int m_deliveringNPCID;
        /// <summary>
        /// 一个任务有多个目标  
        /// 一个目标由 目标类型, Id参数, 数值参数 三个变量描述  
        /// 例: 与Npc交流 Id参数为NpcId 数值参数为1  
        /// 例: 击杀怪物 Id参数为怪物Id 数值参数为要击杀的怪物数量  
        /// </summary>
        public ValueTuple<MissionTargetType, short>[] m_missionTargetArr;
        public long m_bonusMoney;
        public int m_bonusExperience;
        public ValueTuple<short, short>[] m_bonusItemIdAndNumArr;
    }
    struct DO_MallItem {
        public ValueTuple<short, ValueTuple<CurrencyType, int>[]> m_itemIdAndPrice;
    }

    struct DO_MissionTargetGainItemData {
        public MissionTargetType m_TargetType { get { return MissionTargetType.GAIN_ITEM; } }
        public short m_Id { get { return m_id; } set { m_id = value; } }
        short m_id;
        public short m_targetItemId;
        public short m_targetNum;
    }
    struct DO_MissionTargetKillMonsterData {
        public MissionTargetType m_TargetType { get { return MissionTargetType.KILL_MONSTER; } }
        short m_id;
        public short m_Id { get { return m_id; } set { m_id = value; } }
        public short m_targetMonsterId;
        public short m_targetNum;
    }
    struct DO_MissionTargetLevelUpSkillData {
        public MissionTargetType m_TargetType { get { return MissionTargetType.LEVEL_UP_SKILL; } }
        short m_id;
        public short m_Id { get { return m_id; } set { m_id = value; } }
        public short m_targetSkillId;
        public short m_targetLevel;
    }
    struct DO_MissionTargetTalkToNpcData {
        public MissionTargetType m_TargetType { get { return MissionTargetType.TALK_TO_NPC; } }
        short m_id;
        public short m_Id { get { return m_id; } set { m_id = value; } }
        public short m_targetNpcId;
        // 与NPC的对话
        public string[] m_conversation;
    }

    struct DO_MissionTargetArrangement {
        public readonly DO_MissionTargetKillMonsterData[] m_killMonster;
        public readonly DO_MissionTargetGainItemData[] m_gainItem;
        public readonly DO_MissionTargetLevelUpSkillData[] m_levelUpSkill;
        public readonly DO_MissionTargetTalkToNpcData[] m_talkToNPC;

        public DO_MissionTargetArrangement (DO_MissionTargetKillMonsterData[] monster, DO_MissionTargetGainItemData[] item, DO_MissionTargetLevelUpSkillData[] skill, DO_MissionTargetTalkToNpcData[] talk) {
            m_killMonster = monster;
            m_gainItem = item;
            m_levelUpSkill = skill;
            m_talkToNPC = talk;
        }
    }
}