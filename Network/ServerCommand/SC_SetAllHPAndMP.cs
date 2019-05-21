using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class SC_SetAllHPAndMP : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_ALL_HP_AND_MP; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        public List<int> m_ToClientList { get; }
        List<int> m_allNetIdList;
        List<Dictionary<ActorUnitConcreteAttributeType, int>> m_attrList;
        public SC_SetAllHPAndMP (List<int> toClientList, List<int> allNetIdList, List<Dictionary<ActorUnitConcreteAttributeType, int>> attrList) {
            m_ToClientList = toClientList;
            m_allNetIdList = allNetIdList;
            m_attrList = attrList;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_allNetIdList.Count);
            for (int i = 0; i < m_allNetIdList.Count; i++) {
                writer.Put (m_allNetIdList[i]);
                writer.Put (m_attrList[i][ActorUnitConcreteAttributeType.CURRENT_HP]);
                writer.Put (m_attrList[i][ActorUnitConcreteAttributeType.MAX_HP]);
                writer.Put (m_attrList[i][ActorUnitConcreteAttributeType.CURRENT_MP]);
                writer.Put (m_attrList[i][ActorUnitConcreteAttributeType.MAX_MP]);
            }
        }
    }
}