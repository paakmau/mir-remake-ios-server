using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemakeBackend.Network {
    interface IClientCommand {
        NetworkToServerDataType m_DataType { get; }
        void Execute(NetDataReader reader, int netId);
    }
}