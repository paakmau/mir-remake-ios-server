using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MirRemakeBackend {
    abstract class DE_ActorUnit {
        public readonly short m_level;
        public readonly IReadOnlyList<KeyValuePair<ActorUnitConcreteAttributeType, int>> m_concreteAttributeList;
        public DE_ActorUnit (short lv, KeyValuePair<ActorUnitConcreteAttributeType, int>[] attrArr) {
            m_level = lv;
            m_concreteAttributeList = new List<KeyValuePair<ActorUnitConcreteAttributeType, int>> (attrArr);
        }
    }
    class DE_Monster : DE_ActorUnit {
        public readonly short m_monsterId;
        public readonly IReadOnlyList<KeyValuePair<short, short>> m_skillIdAndLevelList;
        public DE_Monster (DO_Monster monsterDo) : base (monsterDo.m_level, monsterDo.m_attrArr) {
            m_monsterId = monsterDo.m_monsterId;
            m_skillIdAndLevelList = new List<KeyValuePair<short, short>>(monsterDo.m_skillIdAndLevelArr);
        }
    }
    class DE_Character : DE_ActorUnit {
        public readonly OccupationType m_occupation;
        public readonly int m_upgradeExperienceInNeed;
        public readonly IReadOnlyList<KeyValuePair<ActorUnitMainAttributeType, int>> m_mainAttributeList;
        public readonly int m_mainAttributePointNum;
        public DE_Character (DO_Character charDo) : base (charDo.m_level, charDo.m_concreteAttributeArr) {
            m_occupation = charDo.m_occupation;
            m_upgradeExperienceInNeed = charDo.m_upgradeExperienceInNeed;
            m_mainAttributeList = new List<KeyValuePair<ActorUnitMainAttributeType, int>> (charDo.m_mainAttributeArr);
            m_mainAttributePointNum = charDo.m_mainAttrPointNumber;
        }
    }
    class DE_Skill {
        public readonly short m_skillId;
        public readonly short m_skillLevel;
        public readonly short m_skillMaxLevel;
        public readonly short m_upgradeCharacterLevelInNeed;
        public readonly long m_upgradeMoneyInNeed;
        public readonly int m_upgradeMasterlyInNeed;
        public readonly int m_mpCost;
        public readonly float m_singTime;
        public readonly float m_castFrontTime;
        public readonly float m_castBackTime;
        public readonly float m_coolDownTime;
        public readonly SkillAimType m_skillAimType;
        public readonly CampType m_targetCamp;
        public readonly byte m_targetNumber;
        public readonly float m_castRange;
        /// <summary>
        /// 技能伤害判定的范围参数
        /// </summary>
        public readonly IReadOnlyList<KeyValuePair<SkillAimParamType, float>> m_damageParamList;
        public readonly DE_Effect m_skillEffect;
        public DE_Skill (DO_Skill skillDo) {
            m_skillId = skillDo.m_skillId;
            m_skillLevel = skillDo.m_skillLevel;
            m_skillMaxLevel = skillDo.m_skillMaxLevel;
            m_upgradeCharacterLevelInNeed = skillDo.m_upgradeCharacterLevelInNeed;
            m_upgradeMoneyInNeed = skillDo.m_upgradeMoneyInNeed;
            m_upgradeMasterlyInNeed = skillDo.m_upgradeMasterlyInNeed;
            m_mpCost = skillDo.m_mpCost;
            m_singTime = skillDo.m_singTime;
            m_castFrontTime = skillDo.m_castFrontTime;
            m_castBackTime = skillDo.m_castBackTime;
            m_coolDownTime = skillDo.m_coolDownTime;
            m_skillAimType = skillDo.m_skillAimType;
            m_targetCamp = skillDo.m_targetCamp;
            m_targetNumber = skillDo.m_targetNumber;
            m_castRange = skillDo.m_castRange;
            m_damageParamList = new List<KeyValuePair<SkillAimParamType, float>>(skillDo.m_damageParamArr);
            m_skillEffect = new DE_Effect (skillDo.m_skillEffect);
        }
    }
    class DE_Effect {
        public readonly EffectType m_type;
        public readonly short m_animId;
        public readonly float m_hitRate;
        public readonly float m_criticalRate;
        public readonly int m_deltaHP;
        public readonly int m_deltaMP;
        public readonly IReadOnlyList<KeyValuePair<short, KeyValuePair<float, float>>> m_statusIdAndValueAndTimeList;
        public DE_Effect (DO_Effect effectDo) {
            m_type = effectDo.m_type;
            m_animId = effectDo.m_animId;
            m_hitRate = effectDo.m_hitRate;
            m_criticalRate = effectDo.m_criticalRate;
            m_deltaHP = effectDo.m_deltaHP;
            m_deltaMP = effectDo.m_deltaMP;
            m_statusIdAndValueAndTimeList = new List<KeyValuePair<short, KeyValuePair<float, float>>> (effectDo.m_statusIdAndValueAndTimeArr);
        }
    }
    class DE_Status {
        public readonly short m_statusId;
        public readonly StatusType m_type;
        public readonly IReadOnlyList<KeyValuePair<ActorUnitConcreteAttributeType, int>> m_affectAttributeArr;
        public readonly IReadOnlyList<ActorUnitSpecialAttributeType> m_specialAttributeArr;
    }
}