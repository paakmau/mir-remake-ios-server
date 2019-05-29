using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_ApplyAllDead : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_DEAD; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public IReadOnlyList<int> m_ToClientList { get; }
        private int m_killerNetId;
        private int m_deadNetId;
        public SC_ApplyAllDead (IReadOnlyList<int> toClientList, int killNetId, int deadNetId) {
            m_ToClientList = toClientList;
            m_killerNetId = killNetId;
            m_deadNetId = deadNetId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_killerNetId);
            writer.Put (m_deadNetId);
        }
    }
}