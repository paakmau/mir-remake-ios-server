using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    interface IServerCommand {
        NetworkToClientDataType m_DataType { get; }
        DeliveryMethod m_DeliveryMethod { get; }
        List<int> m_ToClientList { get; }
        void PutData (NetDataWriter writer);
    }
}