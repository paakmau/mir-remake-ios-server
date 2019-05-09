using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemake {
    interface IClientCommand {
        NetworkToServerDataType m_DataType { get; }
        void Execute(NetDataReader reader, int netId);
    }
}