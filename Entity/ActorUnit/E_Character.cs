
using System.Collections.Generic;
using UnityEngine;
namespace MirRemakeBackend {
    class E_Character : E_ActorUnit {
        public int m_playerId = -1;
        private int m_experience;
        public int m_Experience { get { return m_experience; } }
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.PLAYER; } }
        public int m_upgradeExperienceInNeed;
        // 技能释放计时器及技能信息
        private MyTimer.Time m_skillToCastTimer;
        private E_Skill m_skillToCast;
        private SkillParam m_skillToCastParam;
        public E_Character (int netId, int playerId) {
            m_networkId = netId;
            m_playerId = playerId;
            // TODO: 从数据库获取玩家等级装备技能等级, 并自动计算具体属性
            m_level = 15;
            m_experience = 15053;
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.MAX_HP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.MAX_MP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CURRENT_HP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CURRENT_MP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, 5);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND, 5);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.ATTACK, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.MAGIC, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DEFENCE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.RESISTANCE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.TENACITY, 15);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.SPEED, 700);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_BONUS, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.HIT_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DODGE_RATE, 150);
            m_specialAttributeDict.Add (ActorUnitSpecialAttributeType.FAINT, 0);
            m_specialAttributeDict.Add (ActorUnitSpecialAttributeType.SILENT, 0);
            m_specialAttributeDict.Add (ActorUnitSpecialAttributeType.IMMOBILE, 0);
        }
        public override void Tick(float dT) {
            base.Tick (dT);
            
            // 释放技能
            if (m_skillToCast != null) {
                if (MyTimer.CheckTimeUp (m_skillToCastTimer)) {
                List<E_ActorUnit> unitList = m_skillToCast.GetEffectTargets(this, m_skillToCastParam);
                    SM_ActorUnit.s_instance.NotifyApplyCastSkillSettle (this, m_skillToCast, unitList);
                }
            }
        }
        public void GetAllLearnedSkill (out short[] skillIdArr, out short[] skillLvArr, out int[] skillMasterlyArr) {
            // TODO: 技能相关需要访问数据库
            skillIdArr = new short[3] { 0, 1, 2};
            skillLvArr = new short[3] { 2, 1, 4};
            skillMasterlyArr = new int[3] { 10053, 233, 15};
        }
        public void CastSkillBegin (E_Skill skill, SkillParam parm) {
            // TODO: 有缘改为FSM
            float castAndFront = skill.m_castFrontTime + skill.m_singTime;
            m_skillToCastTimer = MyTimer.s_CurTime.Ticked (castAndFront);
            m_skillToCast = skill;
            m_skillToCastParam = parm;
        }
    }
}