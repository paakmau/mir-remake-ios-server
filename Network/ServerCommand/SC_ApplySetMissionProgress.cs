using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_ApplySelfTalkToMissionNpc : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_SET_MISSION_PROGRESS; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private short m_missionId;
        private byte m_targetNum;
        private short m_value;
        public SC_ApplySelfTalkToMissionNpc (List<int> toClientList, short missionId, byte targetNum, short value) {
            m_ToClientList = toClientList;
            m_missionId = missionId;
            m_targetNum = targetNum;
            m_value = value;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
            writer.Put (m_targetNum);
            writer.Put (m_value);
        }
    }
}