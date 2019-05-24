using System;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_ApplyUseConsumableItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_CONSUMABLE_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            Messenger.Broadcast<int, int> ("CommandApplyUseConsumableItem", netId, itemRealId);
        }
    }
}