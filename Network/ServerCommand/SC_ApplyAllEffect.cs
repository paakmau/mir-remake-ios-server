using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_ApplyAllEffect : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_EFFECT; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private short m_effectAnimId;
        private byte m_statusNum;
        private KeyValuePair<int, E_Status[]>[] m_allNetIdAndStatusArrPairArr;
        public SC_ApplyAllEffect (List<int> toClientList, short effectAnimId, byte statusNum, KeyValuePair<int, E_Status[]>[] allNetIdAndStatusArrPairArr) {
            m_ToClientList = toClientList;
            m_effectAnimId = effectAnimId;
            m_statusNum = statusNum;
            m_allNetIdAndStatusArrPairArr = allNetIdAndStatusArrPairArr;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_effectAnimId);
            writer.Put (m_statusNum);
            int unitHitNum = 0;
            if (m_statusNum != 0)
                foreach (var pair in m_allNetIdAndStatusArrPairArr)
                    if (pair.Value != null)
                        unitHitNum++;
            writer.Put ((byte)unitHitNum);
            foreach (var pair in m_allNetIdAndStatusArrPairArr)
                if (pair.Value != null) {
                    writer.Put(pair.Key);
                    foreach (var status in pair.Value)
                        writer.Put(status.GetNo ());
                }
            writer.Put ((byte)(m_allNetIdAndStatusArrPairArr.Length - unitHitNum));
            foreach (var pair in m_allNetIdAndStatusArrPairArr)
                if (pair.Value == null)
                    writer.Put(pair.Key);
        }
    }
}