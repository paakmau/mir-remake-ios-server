
using LiteNetLib.Utils;


namespace MirRemake {
    interface IClientCommand {
        void ResetData(NetDataReader reader);
        void Execute(E_ActorUnit unit);
    }
}