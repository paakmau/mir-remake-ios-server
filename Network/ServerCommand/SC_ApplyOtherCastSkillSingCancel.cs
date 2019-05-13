using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace MirRemake {
    class SC_ApplyOtherCastSkillSingCancel : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_CAST_SKILL_SING_CANCEL; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private int m_casterNetId;
        public SC_ApplyOtherCastSkillSingCancel (List<int> toClientList, int casterNetId) {
            m_ToClientList = toClientList;
            m_casterNetId = casterNetId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_casterNetId);
        }
    }
}