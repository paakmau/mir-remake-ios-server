using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_ApplyAllEffect : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_EFFECT; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public IReadOnlyList<int> m_ToClientList { get; }
        private short m_effectAnimId;
        private byte m_statusNum;
        private ValueTuple<int, NO_Status[]>[] m_allNetIdAndStatusArrPairArr;
        public SC_ApplyAllEffect (IReadOnlyList<int> toClientList, short effectAnimId, byte statusNum, ValueTuple<int, NO_Status[]>[] allNetIdAndStatusArrPairArr) {
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
                    if (pair.Item2 != null)
                        unitHitNum++;
            writer.Put ((byte)unitHitNum);
            foreach (var pair in m_allNetIdAndStatusArrPairArr)
                if (pair.Item2 != null) {
                    writer.Put(pair.Item1);
                    foreach (var status in pair.Item2)
                        writer.Put(status);
                }
            writer.Put ((byte)(m_allNetIdAndStatusArrPairArr.Length - unitHitNum));
            foreach (var pair in m_allNetIdAndStatusArrPairArr)
                if (pair.Item2 == null)
                    writer.Put(pair.Item1);
        }
    }
}