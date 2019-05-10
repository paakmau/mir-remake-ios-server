using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyUseEquipmentItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_EQUIPMENT_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            SM_ActorUnit.s_instance.CommandUseConsumableItem (netId, itemRealId);
        }
    }
}