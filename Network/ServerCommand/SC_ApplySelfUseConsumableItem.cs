using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class SC_ApplySelfUseConsumableItem : IServerCommand {
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_USE_CONSUMABLE_ITEM; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public List<int> m_ToClientList { get; }
        private int m_itemRealId;
        public SC_ApplySelfUseConsumableItem (List<int> toClientList, int itemRealId) {
            m_ToClientList = toClientList;
            m_itemRealId = itemRealId;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put (m_itemRealId);
        }
    }
}