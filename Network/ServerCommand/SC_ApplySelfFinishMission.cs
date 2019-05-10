using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    struct SC_ApplySelfFinishMission : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_FINISH_MISSION; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private short m_missionId;
        public SC_ApplySelfFinishMission (List<int> toClientList, short missionId) {
            m_ToClientList = toClientList;
            m_missionId = missionId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
        }
    }
}