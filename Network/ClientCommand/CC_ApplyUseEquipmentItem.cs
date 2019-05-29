using System;
using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    class CC_ApplyUseEquipmentItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_EQUIPMENT_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            GL_Item.s_instance.CommandApplyUseEquipmentItem (netId, itemRealId);
        }
    }
}