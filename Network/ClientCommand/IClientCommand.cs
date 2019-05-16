using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemakeBackend {
    interface IClientCommand {
        NetworkToServerDataType m_DataType { get; }
        void Execute(NetDataReader reader, int netId);
    }
}