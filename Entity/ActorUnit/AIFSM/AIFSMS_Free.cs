
namespace MirRemake {
    struct AIFSMS_Free : IAIFSMState {
        public AIFSMStateType m_Type { get { return AIFSMStateType.FREE; } }
        public E_ActorUnit m_Self { get; set; }
        public AIFSMS_Free (E_ActorUnit self) {
            m_Self = self;
        }
        public void Tick (float dT) {

        }
        public IAIFSMState NextState () {
            return null;
        }
    }
}