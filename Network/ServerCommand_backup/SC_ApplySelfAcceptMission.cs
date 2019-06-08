using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_ApplySelfAcceptMission {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_ACCECPT_MISSION; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public IReadOnlyList<int> m_ToClientList { get; }
        private short m_missionId;
        public SC_ApplySelfAcceptMission (IReadOnlyList<int> toClientList, short missionId) {
            m_ToClientList = toClientList;
            m_missionId = missionId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
        }
    }
}