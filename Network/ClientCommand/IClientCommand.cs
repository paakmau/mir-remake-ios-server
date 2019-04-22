using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemake {
    interface IClientCommand {
        void SetData(NetDataReader reader);
        void Execute(E_ActorUnit unit);
    }
}