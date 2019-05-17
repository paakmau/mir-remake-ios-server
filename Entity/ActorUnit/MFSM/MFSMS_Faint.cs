namespace MirRemakeBackend {
    struct MFSMS_Faint : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.FAINT; } }
        public E_Monster m_Self { get; set; }
        public MFSMS_Faint (E_Monster self) { m_Self = self; }
        public void OnEnter (MFSMStateType prevType) { }
        public void OnTick (float dT) { }
        public IMFSMState GetNextState () {
            if (m_Self.m_IsDead)
                return new MFSMS_Dead (m_Self);
            if (!m_Self.m_IsFaint)
                return new MFSMS_AutoBattle (m_Self);
            return null;
        }
        public void OnExit (MFSMStateType nextType) { }
    }
}