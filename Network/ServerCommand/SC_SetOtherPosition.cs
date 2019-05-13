using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace MirRemake {
    class SC_SetOtherPosition : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_OTHER_POSITION; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        public List<int> m_ToClientList { get; }
        List<int> m_otherIdList;
        List<Vector2> m_posList;
        public SC_SetOtherPosition (List<int> toClientList, List<int> otherIdList, List<Vector2> posList) {
            m_ToClientList = toClientList;
            m_otherIdList = otherIdList;
            m_posList = posList;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_otherIdList.Count);
            for (int i = 0; i < m_otherIdList.Count; i++) {
                writer.Put (m_otherIdList[i]);
                writer.PutVector2 (m_posList[i]);
            }
        }
    }
}