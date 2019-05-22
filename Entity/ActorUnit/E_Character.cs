using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using UnityEngine;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.Entity {
    class E_Character : E_ActorUnit {
        private DE_Character m_characterDe;
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.PLAYER; } }
        public int m_UpgradeExperienceInNeed { get { return m_dataEntity.m_upgradeExperienceInNeed; } }
        public int m_characterId;
        public int m_experience;
        // // 技能释放计时器及技能信息
        // private MyTimer.Time m_skillToCastTimer;
        // private E_Skill m_skillToCast;
        // private SkillParam m_skillToCastParam;
        public void Reset (int netId, int charId, DE_ActorUnit auDe, DE_Character charDe, DDO_Character charDdo) {
            base.Reset (auDe);
            m_characterDe = charDe;
            m_networkId = netId;
            m_characterId = charId;
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
        public void CastSkillBegin (E_Skill skill, SkillParam parm) {
            // TODO: 有缘改为FSM
            float castAndFront = skill.m_castFrontTime + skill.m_singTime;
            m_skillToCastTimer = MyTimer.s_CurTime.Ticked (castAndFront);
            m_skillToCast = skill;
            m_skillToCastParam = parm;
        }
    }
}