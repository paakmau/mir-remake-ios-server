
namespace MirRemake {
    class AIFSM {
        private IAIFSMState m_curState;
        public AIFSM (IAIFSMState initState) {
            m_curState = initState;
        }
        public void Tick (float dT) {
            m_curState.Tick (dT);
        }
    }
}