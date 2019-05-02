
namespace MirRemake {
    interface IAIFSMState {
        AIFSMStateType m_Type { get; }
        void Tick (float dT);
        IAIFSMState NextState ();
    }
}