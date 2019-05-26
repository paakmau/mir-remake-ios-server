using System;
using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    class DE_ActorUnit {
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_concreteAttributeList;
        private DE_ActorUnit (ValueTuple<ActorUnitConcreteAttributeType, int>[] attrArr) {
            m_concreteAttributeList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (attrArr);
        }
        public DE_ActorUnit (DO_Monster monster) : this (monster.m_attrArr) { }
        public DE_ActorUnit (DO_Character charDo) : this (charDo.m_concreteAttributeArr) { }
    }
    class DE_Monster {
        public readonly short m_level;
        public readonly IReadOnlyList<ValueTuple<short, short>> m_skillIdAndLevelList;
        public readonly IReadOnlyList<short> m_dropItemIdList;
        public DE_Monster (DO_Monster monsterDo) {
            m_skillIdAndLevelList = new List<ValueTuple<short, short>> (monsterDo.m_skillIdAndLevelArr);
            m_dropItemIdList = new List<short> (monsterDo.m_dropItemIdArr);
        }
    }
    class DE_Character {
        public readonly int m_upgradeExperienceInNeed;
        public readonly IReadOnlyList<ValueTuple<ActorUnitMainAttributeType, int>> m_mainAttributeList;
        public readonly int m_mainAttributePointNum;
        public DE_Character (DO_Character charDo) {
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
        public readonly ItemType m_type;
        public readonly short m_maxNum;
        public readonly ItemQuality m_quality;
        public readonly long m_price;
        public DE_Item (DO_Item itemDo) {
            m_type = itemDo.m_type;
            m_maxNum = itemDo.m_maxNum;
            m_quality = itemDo.m_quality;
            m_price = itemDo.m_price;
        }
    }
    class DE_Consumable {
        public readonly DE_Effect m_itemEffect;
        public DE_Consumable (DO_Consumable consumDo) {
            m_itemEffect = new DE_Effect (consumDo.m_effect);
        }
    }
    class DE_Equipment {
        public readonly OccupationType m_validOccupation;
        public readonly short m_equipLevelInNeed;
        public readonly EquipmentPosition m_equipPosition;
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_attrList;
        public readonly float m_attrWave;
        public DE_Equipment (DO_Equipment equipDo) {
            m_validOccupation = equipDo.m_validOccupation;
            m_equipLevelInNeed = equipDo.m_equipLevelInNeed;
            m_equipPosition = equipDo.m_equipPosition;
            m_attrList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (equipDo.m_equipmentAttributeArr);
            m_attrWave = equipDo.m_attrWave;
        }
    }
    class DE_Gem {
        public readonly IReadOnlyList<ValueTuple<ActorUnitConcreteAttributeType, int>> m_attrList;
        public DE_Gem (DO_Gem gemDo) {
            m_attrList = new List<ValueTuple<ActorUnitConcreteAttributeType, int>> (gemDo.m_equipmentAttributeArr);
        }
    }
}