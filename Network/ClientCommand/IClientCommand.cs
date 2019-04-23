using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemake {
    interface IClientCommand {
        void Execute(NetDataReader reader, int netId);
    }
}