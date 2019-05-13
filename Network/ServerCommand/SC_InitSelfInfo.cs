using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class SC_InitSelfInfo : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_INFO; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private short m_level;
        private int m_exp;
        private short[] m_skillIds;
        private short[] m_skillLevels;
        private int[] m_skillMasterlys;
        public SC_InitSelfInfo (List<int> toClientList, short level, int exp, short[] skillIds, short[] skillLevels, int[] skillMasterlys) {
            m_ToClientList = toClientList;
            m_level = level;
            m_exp = exp;
            m_skillIds = skillIds;
            m_skillLevels = skillLevels;
            m_skillMasterlys = skillMasterlys;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_level);
            writer.Put (m_exp);
            writer.Put ((short) m_skillIds.Length);
            for (int i = 0; i < m_skillIds.Length; i++) {
                writer.Put (m_skillIds[i]);
                writer.Put (m_skillLevels[i]);
                writer.Put (m_skillMasterlys[i]);
            }
        }
    }
}