using UnityEngine;

namespace MirRemake {
    struct FSMS_CastSingCancel : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_SING_CANCEL; } }
        public E_ActorUnit m_Self { get; set; }
        public FSMS_CastSingCancel (E_ActorUnit self) {
            m_Self = self;
        }
        public bool HasActiveTransitionTo (FSMStateType next) { return false; }
        public bool CanActiveExit (FSMStateType next) { return false; }
        public bool CanActiveEnter (FSMStateType prev) { return true; }
        public void OnEnter (FSMStateType prevType) { }
        public void OnTick (float dT) { }
        public IFSMState GetNextState () {
            return new FSMS_Free (m_Self);
        }
        public void OnExit (FSMStateType nextType) { }
    }
}