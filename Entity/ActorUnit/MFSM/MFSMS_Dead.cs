namespace MirRemakeBackend {
    struct MFSMS_Dead : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.DEAD; } }
        public E_Monster m_Self { get; set; }
        private const float c_bodyDisappearTime = 10f;
        private float m_timer;
        public MFSMS_Dead (E_Monster self) {
            m_Self = self;
            m_timer = 0f;
        }
        public void OnEnter (MFSMStateType prevType) {
            m_timer = c_bodyDisappearTime;
        }
        public void OnTick (float dT) {
            m_timer -= dT;
            if (m_timer <= 0f)
                SM_ActorUnit.s_instance.NotifyUnitBodyDisappear (m_Self);
        }
        public IMFSMState GetNextState () {
            return null;
        }
        public void OnExit (MFSMStateType nextType) { }
    }
}