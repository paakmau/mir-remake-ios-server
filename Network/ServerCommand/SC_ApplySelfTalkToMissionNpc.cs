using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    struct SC_ApplySelfTalkToMissionNpc : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_TALK_TO_MISSION_NPC; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private short m_npcId;
        public SC_ApplySelfTalkToMissionNpc (List<int> toClientList, short npcId) {
            m_ToClientList = toClientList;
            m_npcId = npcId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_npcId);
        }
    }
}