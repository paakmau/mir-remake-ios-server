namespace MirRemake {
    struct FSMS_Faint : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.FAINT; } }
        public E_ActorUnit m_Self { get; set; }
        public int m_SelfNetworkId { get { return m_Self.m_NetworkId; } }
        private float m_lastTime;
        public float m_LastTime { get { return m_lastTime; } }
        public FSMS_Faint (E_ActorUnit self, float lastTime) {
            m_Self = self;
            m_lastTime = lastTime;
        }
        public void SetSelf (E_ActorUnit self) {
            m_Self = self;
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
            m_Self.m_entityView.PlayFaint ();
        }
        public void OnTick (float dT) {
            m_lastTime -= dT;
        }
        public IFSMState GetNextState () {
            if (m_lastTime <= 0.0f)
                return new FSMS_Free (m_Self);
            return null;
        }
        public void OnExit (FSMStateType nextType) { }
    }
}