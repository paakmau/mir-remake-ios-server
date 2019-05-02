using UnityEngine;
namespace MirRemake {
    struct FSMS_CastFront : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_FRONT; } }
        public E_ActorUnit m_Self { get; set; }
        private E_Skill m_skill;
        private TargetPosition m_tarPos;
        private Vector2 m_parm;
        private Vector2 m_castDir;
        private float m_timer;
        public FSMS_CastFront (E_ActorUnit self, E_Skill skill, TargetPosition tarPos, Vector2 parm) {
            m_Self = self;
            m_skill = skill;
            m_tarPos = tarPos;
            m_parm = parm;
            m_castDir = Vector2.zero;
            m_timer = 0.0f;
        }
        public bool HasActiveTransitionTo (FSMStateType next) {
            return false;
        }
        public bool CanActiveExit (FSMStateType next) {
            return false;
        }
        public bool CanActiveEnter (FSMStateType prev) {
            return false;
        }
        public void OnEnter (FSMStateType prevType) {
            m_castDir = m_skill.GetCastDirection (m_Self, m_tarPos, m_parm);
            m_Self.m_entityView.PlayCastSkillFront (m_skill.m_id, m_castDir);
            m_timer = 0;
        }
        public void OnTick (float dT) {
            m_timer += dT;
        }
        public IFSMState GetNextState () {
            if (m_timer >= m_skill.m_castFrontTime)
                return new FSMS_CastBack (m_Self, m_skill, m_castDir);
            return null;
        }
        public void OnExit (FSMStateType nextType) {
            m_Self.RequestCastSkill (m_skill, m_tarPos, m_parm);
        }
    }
}