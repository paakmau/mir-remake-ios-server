namespace MirRemakeBackend {
    struct FSMS_Faint : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.FAINT; } }
        public E_Monster m_Self { get; set; }
        public void OnEnter (FSMStateType prevType) { }
        public void OnTick (float dT) { }
        public IFSMState GetNextState () {
            if (m_Self.m_IsDead)
                return new FSMS_Dead(m_Self);
            return null;
        }
        public void OnExit (FSMStateType nextType) { }
    }
}