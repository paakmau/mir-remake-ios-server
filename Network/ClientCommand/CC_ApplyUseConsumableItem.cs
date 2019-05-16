using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemakeBackend {
    class CC_ApplyUseConsumableItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_CONSUMABLE_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            SM_ActorUnit.s_instance.CommandUseConsumableItem (netId, itemRealId);
        }
    }
}