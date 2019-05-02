using UnityEngine;
namespace MirRemake {
    struct FSMS_CastBack : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_BACK; } }
        public E_ActorUnit m_Self { get; set; }
        private E_Skill m_skill;
        private Vector2 m_castDir;
        private float m_timer;
        public FSMS_CastBack (E_ActorUnit self, E_Skill skill, Vector2 castDir) {
            m_Self = self;
            m_skill = skill;
            m_castDir = castDir;
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
            m_timer = 0.0f;
        }
        public void OnTick (float dT) {
            m_timer += dT;
        }
        public IFSMState GetNextState () {
            if (m_timer >= m_skill.m_castBackTime)
                return new FSMS_Free (m_Self);;
            return null;
        }
        public void OnExit (FSMStateType nextType) { }
    }
}