
namespace MirRemake {
    interface IAIFSMState {
        AIFSMStateType m_Type { get; }
        E_ActorUnit m_Self { get; set; }
        void Tick (float dT);
        IAIFSMState NextState ();
    }
}