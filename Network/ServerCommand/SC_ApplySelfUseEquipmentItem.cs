using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class SC_ApplySelfUseEquipmentItem : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_USE_EQUIPMENT_ITEM; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private int m_itemRealId;
        public SC_ApplySelfUseEquipmentItem (List<int> toClientList, int itemRealId) {
            m_ToClientList = toClientList;
            m_itemRealId = itemRealId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_itemRealId);
        }
    }
}