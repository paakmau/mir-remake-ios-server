

namespace MirRemakeBackend {
    interface INetworkService {
        void SendServerCommand (IServerCommand command);
    }
}