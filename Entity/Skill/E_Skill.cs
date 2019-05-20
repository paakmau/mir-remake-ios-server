/**
 *Enity，技能实体
 *创建者 fn
 *时间 2019/4/1
 *最后修改者 yuk
 *时间 2019/4/3
 */

using System;
using System.Collections.Generic;
using UnityEngine;
namespace MirRemakeBackend {
    class E_Skill {
        public short m_id;
        public short m_level;
        // 技能熟练度
        public int m_masterly;
        public short m_maxLevel;
        public short m_upgradeCharacterLevelInNeed;
        public long m_upgradeMoneyInNeed;
        public int m_upgradeMasterlyInNeed;
        public short[] m_fatherIdArr;
        public short[] m_childrenIdArr;
        public int m_costMP;
        // 咏唱时间
        public float m_singTime;
        public bool m_NeedSing { get { return m_singTime != 0.0f; } }
        // 前摇时间
        public float m_castFrontTime;
        // 后摇时间
        public float m_castBackTime;
        // 冷却时间
        public float m_coolDownTime;
        public SkillTargetChooserBase m_targetChooser;
        public E_Effect m_skillEffect;
        public SkillAimType m_AimType { get { return m_targetChooser.m_TargetAimType; } }
        public E_Skill (DO_Skill dataObj, DDO_Skill ddo, int casterNetId) {
            m_id = ddo.m_skillId;
            m_level = ddo.m_skillLevel;
            m_masterly = ddo.m_masterly;
            m_maxLevel = dataObj.m_skillMaxLevel;
            m_upgradeCharacterLevelInNeed = dataObj.m_upgradeCharacterLevelInNeed;
            m_upgradeMoneyInNeed = dataObj.m_upgradeMoneyInNeed;
            m_upgradeMasterlyInNeed = dataObj.m_upgradeMasterlyInNeed;
            m_fatherIdArr = dataObj.m_fatherIdArr;
            m_childrenIdArr = dataObj.m_childrenIdArr;
            m_costMP = dataObj.m_manaCost;
            m_singTime = dataObj.m_singTime;
            m_castFrontTime = dataObj.m_castFrontTime;
            m_castBackTime = dataObj.m_castBackTime;
            m_coolDownTime = dataObj.m_coolDownTime;

            switch (dataObj.m_skillAimType) {
                case SkillAimType.AIM_CICLE:
                    m_targetChooser = new STC_AimCircle (dataObj.m_targetCamp, dataObj.m_targetNumber, dataObj.m_targetNumber, dataObj.m_damageParamArr);
                    break;
                case SkillAimType.NOT_AIM_SELF_CIRCLE:
                    m_targetChooser = new STC_NotAimSelfCircle (dataObj.m_targetCamp, dataObj.m_targetNumber, dataObj.m_targetNumber, dataObj.m_damageParamArr);
                    break;
                    // TODO: 
            }
            m_skillEffect = new E_Effect (dataObj.m_skillEffect, casterNetId);
        }
        public E_Skill (DO_Skill dataObj, int casterNetId) : this (dataObj, new DDO_Skill (), casterNetId) { }
        public List<E_ActorUnit> GetEffectTargets (E_ActorUnit self, SkillParam parm) {
            return m_targetChooser.GetEffectTargets (self, parm);
        }
        public SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget, SkillParam parm) {
            return m_targetChooser.CompleteSkillParam (self, aimedTarget, parm);
        }
        public bool InRange (Vector2 pos, SkillParam parm) {
            return m_targetChooser.InRange (pos, parm);
        }
    }
}