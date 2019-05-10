using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyUseConsumableItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_CONSUMABLE_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            short itemId = reader.GetShort ();
            SM_ActorUnit.s_instance.CommandUseConsumableItem (netId, itemId);
        }
    }
}