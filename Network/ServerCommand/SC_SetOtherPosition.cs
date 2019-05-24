using System;
using System.Numerics;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_SetOtherPosition : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_OTHER_POSITION; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        public IReadOnlyList<int> m_ToClientList { get; }
        List<ValueTuple<int, Vector2>> m_otherNetIdAndPosList;
        public SC_SetOtherPosition (IReadOnlyList<int> toClientList, List<ValueTuple<int, Vector2>> otherNetIdAndPosList) {
            m_ToClientList = toClientList;
            m_otherNetIdAndPosList = otherNetIdAndPosList;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_otherNetIdAndPosList.Count);
            for (int i = 0; i < m_otherNetIdAndPosList.Count; i++) {
                writer.Put (m_otherNetIdAndPosList[i].Item1);
                writer.Put (m_otherNetIdAndPosList[i].Item2);
            }
        }
    }
}