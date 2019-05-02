using UnityEngine;
namespace MirRemake {
    class FSM {
        public IFSMState m_curState;
        public FSM (IFSMState initState) {
            m_curState = initState;
        }
        /// <summary>
        /// 主动转移状态, 由Entity驱动
        /// </summary>
        /// <param name="nextState"></param>
        /// <returns>若转移成功返回true, 否则返回false</returns>
        public bool ActiveTransit (IFSMState nextState) {
            if (m_curState.HasActiveTransitionTo (nextState.m_Type) && m_curState.CanActiveExit (nextState.m_Type) && nextState.CanActiveEnter (m_curState.m_Type)) {
                m_curState.OnExit (nextState.m_Type);
                nextState.OnEnter (m_curState.m_Type);
                m_curState = nextState;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 强制转移状态, 仅用于网络FSM同步时
        /// </summary>
        /// <param name="nextState"></param>
        public void ForceTransit (IFSMState nextState) {
            m_curState.OnExit (nextState.m_Type);
            nextState.OnEnter (m_curState.m_Type);
            m_curState = nextState;
        }
        public void Tick (float dT) {
            m_curState.OnTick (dT);
            IFSMState nextState = m_curState.GetNextState();
            if(nextState != null) {
                // 自动转移
                m_curState.OnExit (nextState.m_Type);
                nextState.OnEnter (m_curState.m_Type);
                m_curState = nextState;
            }
        }
    }
}