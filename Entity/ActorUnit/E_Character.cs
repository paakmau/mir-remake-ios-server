using System.Collections.Generic;
using UnityEngine;
namespace MirRemakeBackend {
    class E_Character : E_ActorUnit {
        public int m_playerId;
        private int m_experience;
        public int m_Experience { get { return m_experience; } }
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.PLAYER; } }
        public int m_upgradeExperienceInNeed;
        // 技能释放计时器及技能信息
        private MyTimer.Time m_skillToCastTimer;
        private E_Skill m_skillToCast;
        private SkillParam m_skillToCastParam;
        public E_Character (int netId, int playerId, DO_Character charDo, DDO_Character charDdo) {
            m_networkId = netId;
            m_playerId = playerId;
            foreach (var item in charDo.m_concreteAttributeArr)
                m_concreteAttributeDict.Add (item.Key, item.Value);
            m_upgradeExperienceInNeed = charDo.m_experienceInNeed;
            m_level = charDdo.m_level;
            m_experience = charDdo.m_experience;
        }
        public override void Tick (float dT) {
            base.Tick (dT);

            // 释放技能
            if (m_skillToCast != null) {
                if (MyTimer.CheckTimeUp (m_skillToCastTimer)) {
                    List<E_ActorUnit> unitList = m_skillToCast.GetEffectTargets (this, m_skillToCastParam);
                    SM_ActorUnit.s_instance.NotifyApplyCastSkillSettle (this, m_skillToCast, unitList);
                }
            }
        }
        public void GetAllLearnedSkill (out short[] skillIdArr, out short[] skillLvArr, out int[] skillMasterlyArr) {
            // TODO: 技能相关需要访问数据库
            skillIdArr = new short[3] { 0, 1, 2 };
            skillLvArr = new short[3] { 2, 1, 4 };
            skillMasterlyArr = new int[3] { 10053, 233, 15 };
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