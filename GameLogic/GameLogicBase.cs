using MirRemakeBackend.Network;


namespace MirRemakeBackend.GameLogic {
    abstract class GameLogicBase {
        protected INetworkService m_networkService;
        public GameLogicBase (INetworkService networkService) {
            m_networkService = networkService;
        }
        public abstract void Tick (float dT);
        public abstract void NetworkTick ();
    }
}