using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    struct MFSMS_CastSingAndFront : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.CAST_SING_AND_FRONT; } }
        public E_Monster m_Self { get; set; }
        private E_Skill m_skill;
        private TargetPosition m_tarPos;
        private float m_timer;
        public MFSMS_CastSingAndFront (E_Monster self, E_Skill skill, TargetPosition tarPos) {
            m_Self = self;
            m_skill = skill;
            m_tarPos = tarPos;
            m_timer = 0f;
        }
        public void OnEnter (MFSMStateType prevType) {
            m_timer = m_skill.m_singTime + m_skill.m_castFrontTime;
            // TODO: 发送普通FSM状态机
        }
        public void OnTick (float dT) {
            m_timer -= dT;
        }
        public IMFSMState GetNextState () {
            // 咏唱结束
            if (m_timer <= 0f)
                return new MFSMS_CastBack (m_Self, m_skill.m_castBackTime);
            // 被沉默
            if (m_Self.m_IsSilent)
                return new MFSMS_Free (m_Self);
            return null;
        }
        public void OnExit (MFSMStateType nextType) {
            if (nextType == MFSMStateType.CAST_BACK) {
                List<E_ActorUnit> unitList = m_skill.GetEffectTargets(m_Self, m_tarPos, Vector2.zero);
                int[] unitNetIdArr = new int[unitList.Count];
                for (int i=0; i<unitList.Count; i++)
                    unitNetIdArr[i] = unitList[i].m_networkId;
                SM_ActorUnit.s_instance.CommandApplyCastSkill (m_Self.m_networkId, m_skill.m_id, unitNetIdArr);
            }
        }
    }
}