using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class SC_ApplyOtherUseEquipmentItem : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_USE_EQUIPMENT_ITEM; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private short m_itemId;
        public SC_ApplyOtherUseEquipmentItem (List<int> toClientList, short itemId) {
            m_ToClientList = toClientList;
            m_itemId = itemId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_itemId);
        }
    }
}