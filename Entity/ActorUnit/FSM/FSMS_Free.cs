namespace MirRemake {
    struct FSMS_Free : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.FREE; } }
        public E_ActorUnit m_Self { get; set; }
        public FSMS_Free (E_ActorUnit self) {
            m_Self = self;
        }
        public bool HasActiveTransitionTo (FSMStateType next) {
            if(next == FSMStateType.CAST_SING_CANCEL) return false;
            return true;
        }
        public bool CanActiveExit (FSMStateType next) {
            return true;
        }
        public bool CanActiveEnter (FSMStateType prev) {
            return true;
        }
        public void OnEnter (FSMStateType prevType) {
        }
        public void OnTick (float dT) {
            if (m_Self.m_IsSelf) {
                if (!m_Self.m_IsImmobile && m_Self.m_fSMMoveDir.sqrMagnitude > 0.01f)
                    m_Self.m_entityView.Move (m_Self.m_fSMMoveDir * m_Self.m_Speed, dT);
            }else {
                m_Self.m_entityView.SetPositionLerp(m_Self.m_fSMSyncPosition, dT);
            }
        }
        public void SetSelf (E_ActorUnit self) {
            m_Self = self;
        }
        public IFSMState GetNextState () { return null; }
        public IFSMState GetAlterState () { return null; }
        public void OnExit (FSMStateType nextType) { }
        public bool m_NeedSend { get { return false; } }
    }
}