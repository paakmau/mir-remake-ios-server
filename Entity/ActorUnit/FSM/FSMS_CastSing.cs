using UnityEngine;

namespace MirRemake {
    struct FSMS_CastSing : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_SING; } }
        public E_ActorUnit m_Self { get; set; }
        private E_Skill m_skill;
        private TargetPosition m_tarPos;
        private Vector2 m_parm;
        private Vector2 m_castDir;
        private float m_timer;
        public FSMS_CastSing (E_ActorUnit self, E_Skill skill, TargetPosition tarPos, Vector2 parm) {
            m_Self = self;
            m_skill = skill;
            m_tarPos = tarPos;
            m_parm = parm;
            m_castDir = Vector2.zero;
            m_timer = 0.0f;
        }
        public bool HasActiveTransitionTo (FSMStateType next) {
            if (next == FSMStateType.CAST_SING_CANCEL)
                return true;
            return false;
        }
        public bool CanActiveExit (FSMStateType next) {
            return true;
        }
        public bool CanActiveEnter (FSMStateType prev) {
            return false;
        }
        public void OnEnter (FSMStateType prevType) {
            m_castDir = m_skill.GetCastDirection (m_Self, m_tarPos, m_parm);
            m_Self.m_entityView.PlaySingSkill (m_skill.m_id, m_castDir);
            m_timer = 0;
        }
        public void OnTick (float dT) {
            m_timer += dT;
            // 施法时移动
            if (m_Self.m_IsSelf) {
                if (m_Self.m_fSMMoveDir.sqrMagnitude >= 0.01f)
                    m_Self.m_entityView.Move(m_Self.m_fSMMoveDir * m_Self.m_Speed * 0.3f, dT);
            } else {
                m_Self.m_entityView.SetPositionLerp(m_Self.m_fSMSyncPosition, dT);
            }
        }
        public IFSMState GetNextState () {
            // 咏唱结束
            if (m_timer >= m_skill.m_singTime)
                return new FSMS_CastFront (m_Self, m_skill, m_tarPos, m_parm);
            // 被沉默
            if (m_Self.m_IsSilent)
                return new FSMS_Free (m_Self);
            return null;
        }
        public void OnExit (FSMStateType nextType) { }
    }
}