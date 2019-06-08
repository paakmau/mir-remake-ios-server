using System;
using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    class DE_Unit {
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_concreteAttributeList;
        private DE_Unit (ValueTuple<ActorUnitConcreteAttributeType, int>[] attrArr) {
            m_concreteAttributeList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (attrArr);
        }
        public DE_Unit (DO_Monster monster) : this (monster.m_attrArr) { }
        public DE_Unit (DO_Character charDo) : this (charDo.m_concreteAttributeArr) { }
    }
    class DE_MonsterData {
        public readonly short m_monsterId;
        public readonly short m_level;
        public readonly IReadOnlyList<ValueTuple<short, short>> m_skillIdAndLevelList;
        public readonly IReadOnlyList<short> m_dropItemIdList;
        public DE_MonsterData (DO_Monster monsterDo) {
            m_monsterId = monsterDo.m_monsterId;
            m_level = monsterDo.m_level;
            m_skillIdAndLevelList = new List<ValueTuple<short, short>> (monsterDo.m_skillIdAndLevelArr);
            m_dropItemIdList = new List<short> (monsterDo.m_dropItemIdArr);
        }
    }
    class DE_CharacterData {
        public readonly int m_upgradeExperienceInNeed;
        public readonly IReadOnlyList<ValueTuple<ActorUnitMainAttributeType, int>> m_mainAttributeList;
        public readonly int m_mainAttributePointNum;
        public DE_CharacterData (DO_Character charDo) {
            m_upgradeExperienceInNeed = charDo.m_upgradeExperienceInNeed;
            m_mainAttributeList = new List<ValueTuple<ActorUnitMainAttributeType, int>> (charDo.m_mainAttributeArr);
            m_mainAttributePointNum = charDo.m_mainAttrPointNumber;
        }
    }
    class DE_SkillData {
        public readonly short m_upgradeCharacterLevelInNeed;
        public readonly long m_upgradeMoneyInNeed;
        public readonly int m_upgradeMasterlyInNeed;
        public readonly int m_mpCost;
        public readonly float m_singTime;
        public readonly float m_castFrontTime;
        public readonly float m_castBackTime;
        public readonly float m_coolDownTime;
        public readonly byte m_targetNumber;
        public readonly float m_castRange;
        /// <summary>
        /// 技能伤害判定的范围参数
        /// </summary>
        public readonly IReadOnlyList<ValueTuple<SkillAimParamType, float>> m_damageParamList;
        public readonly DE_Effect m_skillEffect;
        public DE_SkillData (DO_SkillData dataObj) {
            m_upgradeCharacterLevelInNeed = dataObj.m_upgradeCharacterLevelInNeed;
            m_upgradeMoneyInNeed = dataObj.m_upgradeMoneyInNeed;
            m_upgradeMasterlyInNeed = dataObj.m_upgradeMasterlyInNeed;
            m_mpCost = dataObj.m_mpCost;
            m_singTime = dataObj.m_singTime;
            m_castFrontTime = dataObj.m_castFrontTime;
            m_castBackTime = dataObj.m_castBackTime;
            m_coolDownTime = dataObj.m_coolDownTime;
            m_targetNumber = dataObj.m_targetNumber;
            m_castRange = dataObj.m_castRange;
            m_damageParamList = new List<ValueTuple<SkillAimParamType, float>> (dataObj.m_damageParamArr);
            m_skillEffect = new DE_Effect (dataObj.m_skillEffect);
        }
    }
    class DE_Skill {
        public readonly short m_skillId;
        public readonly short m_skillMaxLevel;
        public readonly IReadOnlyList<short> m_fatherIdList;
        public readonly IReadOnlyList<short> m_childrenIdList;
        public readonly SkillAimType m_skillAimType;
        public readonly CampType m_targetCamp;
        public readonly IReadOnlyList<DE_SkillData> m_skillDataAllLevel;
        public DE_Skill (DO_Skill skillDo) {
            m_skillMaxLevel = skillDo.m_skillMaxLevel;
            m_skillAimType = skillDo.m_skillAimType;
            m_targetCamp = skillDo.m_targetCamp;
            DE_SkillData[] dataArr = new DE_SkillData[skillDo.m_skillDataAllLevel.Length];
            for (int i = 0; i < skillDo.m_skillDataAllLevel.Length; i++)
                dataArr[i] = new DE_SkillData (skillDo.m_skillDataAllLevel[i]);
            m_skillDataAllLevel = new List<DE_SkillData> (dataArr);
        }
    }
    class DE_Effect {
        public readonly EffectType m_type;
        public readonly short m_animId;
        public readonly float m_hitRate;
        public readonly float m_criticalRate;
        public readonly int m_deltaHp;
        public readonly int m_deltaMp;
        public readonly IReadOnlyList<ValueTuple<short, float, float>> m_statusIdAndValueAndTimeList;
        public DE_Effect (DO_Effect effectDo) {
            m_type = effectDo.m_type;
            m_animId = effectDo.m_animId;
            m_hitRate = effectDo.m_hitRate;
            m_criticalRate = effectDo.m_criticalRate;
            m_deltaHp = effectDo.m_deltaHp;
            m_deltaMp = effectDo.m_deltaMp;
            m_statusIdAndValueAndTimeList = new List<ValueTuple<short, float, float>> (effectDo.m_statusIdAndValueAndTimeArr);
        }
    }
    class DE_Status {
        public readonly StatusType m_type;
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_affectAttributeList;
        public readonly IReadOnlyList<ActorUnitSpecialAttributeType> m_specialAttributeList;
        public DE_Status (DO_Status statusDo) {
            m_type = statusDo.m_type;
            m_affectAttributeList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (statusDo.m_affectAttributeArr);
            m_specialAttributeList = new List<ActorUnitSpecialAttributeType> (statusDo.m_specialAttributeArr);
        }
    }
    class DE_Item {
        public readonly short m_id;
        public readonly ItemType m_type;
        public readonly short m_maxNum;
        public readonly ItemQuality m_quality;
        public readonly long m_price;
        public DE_Item (DO_Item itemDo) {
            m_id = itemDo.m_itemId;
            m_type = itemDo.m_type;
            m_maxNum = itemDo.m_maxNum;
            m_quality = itemDo.m_quality;
            m_price = itemDo.m_price;
        }
    }
    class DE_ConsumableData {
        public readonly DE_Effect m_itemEffect;
        public DE_ConsumableData (DO_Consumable consumDo) {
            m_itemEffect = new DE_Effect (consumDo.m_effect);
        }
    }
    class DE_EquipmentData {
        public readonly OccupationType m_validOccupation;
        public readonly short m_equipLevelInNeed;
        public readonly EquipmentPosition m_equipPosition;
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_attrList;
        public readonly float m_attrWave;
        public DE_EquipmentData (DO_Equipment equipDo) {
            m_validOccupation = equipDo.m_validOccupation;
            m_equipLevelInNeed = equipDo.m_equipLevelInNeed;
            m_equipPosition = equipDo.m_equipPosition;
            m_attrList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (equipDo.m_equipmentAttributeArr);
            m_attrWave = equipDo.m_attrWave;
        }
    }
    class DE_GemData {
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_attrList;
        public DE_GemData (DO_Gem gemDo) {
            m_attrList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (gemDo.m_equipmentAttributeArr);
        }
    }
    class DE_Mission {
        public readonly short m_id;
        public readonly OccupationType m_occupation;
        public readonly short m_levelInNeed;
        public readonly IReadOnlyList<short> m_childrenIdList;
        /// <summary>
        /// 一个任务有多个目标  
        /// 一个目标由 目标类型, Id参数, 数值参数 三个变量描述  
        /// 例: 与Npc交流 Id参数为NpcId 数值参数为1  
        /// 例: 击杀怪物 Id参数为怪物Id 数值参数为要击杀的怪物数量  
        /// </summary>
        public readonly IReadOnlyList < (MissionTargetType, short, int) > m_targetAndParamList;
        public readonly int m_bonusCoin;
        public readonly int m_bonusExperience;
        public readonly IReadOnlyList < (short, short) > m_bonusItemIdAndNumList;
        public DE_Mission (DO_Mission mDo) {
            m_id = mDo.m_id;
            m_occupation = mDo.m_missionOccupation;
            m_levelInNeed = mDo.m_levelInNeed;
            m_childrenIdList = new List<short> (mDo.m_childrenMissionArr);
            m_targetAndParamList = new List<ValueTuple<MissionTargetType, short, int>> (mDo.m_missionTargetArr);
            m_bonusCoin = mDo.m_bonusCoin;
            m_bonusExperience = mDo.m_bonusExperience;
            m_bonusItemIdAndNumList = new List<ValueTuple<short, short>> (mDo.m_bonusItemIdAndNumArr);
        }
    }
}