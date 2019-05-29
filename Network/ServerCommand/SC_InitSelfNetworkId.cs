using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_InitSelfNetworkId : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_NETWORK_ID; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public IReadOnlyList<int> m_ToClientList { get; }
        private int m_networkId;
        public SC_InitSelfNetworkId (IReadOnlyList<int> toClientList, int netId) {
            m_ToClientList = toClientList;
            m_networkId = netId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_networkId);
        }
    }
}