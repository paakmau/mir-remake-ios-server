using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    class GL_Item : GameLogicBase {
        private IDDS_Item m_itemDds;
        public GL_Item (IDDS_Item itemDds, INetworkService netService) : base (netService) {
            m_itemDds = itemDds;
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, long> ("CommandApplyUseConsumableItem", CommandApplyUseConsumableItem);
            Messenger.AddListener<int, long> ("CommandApplyUseEquipmentItem", CommandApplyUseEquipmentItem);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            var bagDdo = m_itemDds.GetBagByCharacterId (charId);
            var storeHouseDdo = m_itemDds.GetStoreHouseByCharacterId (charId);
            var eqRegionDdo = m_itemDds.GetEquipmentRegionByCharacterId (charId);
            var equipmentDdo = m_itemDds.GetAllEquipmentByCharacterId (charId);
            EM_Item.s_instance.InitCharacterItem(netId, bagDdo, storeHouseDdo, eqRegionDdo, equipmentDdo);
            // TODO: 读取角色bag, storeHouse, equiped道具, 并发送给Client
        }
        public void CommandRemoveCharacter (int netId) {
            // TODO: 移除角色相关的道具, 仓库, 装备区
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            E_Item item = EM_Item.s_instance.GetItemByRealId (realId);
            if (item == null) return;
            Messenger.Broadcast<int, E_ConsumableItem> ("NotifyUseConsumable", netId, (E_ConsumableItem) item);
            // TODO: 向客户端发送道具消耗
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_EquipmentItem eq = EM_Item.s_instance.GetItemByRealId (realId) as E_EquipmentItem;
            E_EquipmentRegion equips = EM_Item.s_instance.GetEquipedByNetworkId (netId);
            E_Repository bag = EM_Item.s_instance.GetBagByNetworkId (netId);
            if (eq == null || equips == null || bag == null) return;
            if (bag.RemoveItem (realId, 1) != 1) return;
            E_EquipmentItem oriEq = equips.PutOnEquipment (eq);
            bag.StoreItem (oriEq);
            if (oriEq != null)
                Messenger.Broadcast<int, E_EquipmentItem> ("NotifyTakeOffEquipment", netId, oriEq);
            Messenger.Broadcast<int, E_EquipmentItem> ("NotifyPutOnEquipment", netId, eq);
            // TODO: 向客户端发送道具移动
        }
    }
}