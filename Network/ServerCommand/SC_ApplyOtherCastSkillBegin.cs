using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_ApplyOtherCastSkillBegin : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_CAST_SKILL_BEGIN; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public IReadOnlyList<int> m_ToClientList { get; }
        private int m_casterNetId;
        private short m_skillId;
        private NO_SkillParam m_parm;
        public SC_ApplyOtherCastSkillBegin (IReadOnlyList<int> toClientList, int casterNetId, short skillId, NO_SkillParam parm) {
            m_ToClientList = toClientList;
            m_casterNetId = casterNetId;
            m_skillId = skillId;
            m_parm = parm;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_casterNetId);
            writer.Put (m_skillId);
            writer.Put (m_parm);
        }
    }
}