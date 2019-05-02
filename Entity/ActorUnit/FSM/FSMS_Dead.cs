namespace MirRemake {
    struct FSMS_Dead : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.DEAD; } }
        public E_ActorUnit m_Self { get; set; }
        private const float c_bodyDisappearTime = 10f;
        private float m_timer;
        public FSMS_Dead (E_ActorUnit self) {
            m_Self = self;
            m_timer = 0f;
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
            m_Self.m_entityView.PlayDead();
            m_timer = 0f;
        }
        public void OnTick (float dT) {
            m_timer += dT;
            if(m_timer >= c_bodyDisappearTime)
                m_Self.m_BodyDisappear = true;
        }
        public IFSMState GetNextState () { return null; }
        public IFSMState GetAlterState () { return null; }
        public void OnExit (FSMStateType nextType) { }
        public bool m_NeedSend { get { return false; } }
    }
}