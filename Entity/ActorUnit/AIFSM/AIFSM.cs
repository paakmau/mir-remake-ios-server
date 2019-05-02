
namespace MirRemake {
    class AIFSM {
        private IAIFSMState m_curState;
        public void Tick (float dT) {
            m_curState.Tick (dT);
        }
    }
}