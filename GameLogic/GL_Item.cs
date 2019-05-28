using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
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
        public static GL_Item s_instance;
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
            EM_Item.s_instance.InitCharacterItem (netId, bagDdo, storeHouseDdo, eqRegionDdo, equipmentDdo);
            // TODO: 把bag, storeHouse, equiped道具, 发送给Client
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Item.s_instance.RemoveCharacterItem (netId);
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            E_ConsumableItem item = EM_Item.s_instance.GetItemByRealId (realId) as E_ConsumableItem;
            E_Repository bag = EM_Item.s_instance.GetBagByNetworkId (netId);
            E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (item == null || bag == null || unit == null) return;
            // 从背包中移除一个该物品
            if (bag.RemoveItem (realId, 1) != 1) return;
            GL_Effect.s_instance.NotifyApplyEffect(item.m_consumableDe.m_itemEffect, unit, unit);
            // TODO: 考虑数据库修改
            // TODO: 向客户端发送道具消耗
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_EquipmentItem equipment = EM_Item.s_instance.GetItemByRealId (realId) as E_EquipmentItem;
            E_EquipmentRegion eqRegion = EM_Item.s_instance.GetEquipedByNetworkId (netId);
            E_Repository bag = EM_Item.s_instance.GetBagByNetworkId (netId);
            if (equipment == null || eqRegion == null || bag == null) return;
            // 从背包中移除该装备
            if (bag.RemoveItem (realId, 1) != 1) return;
            // 穿上该装备, 并卸下该位置上原有装备(如果有)
            E_EquipmentItem oriEq = eqRegion.PutOnEquipment (equipment);
            // 如果该位置原本非空, 存入背包
            if (oriEq != null) {
                bag.StoreItem (oriEq);
                // 装备被卸下Attr
                EquipmentToAttr (netId, oriEq, -1);
            }
            // 装备穿上Attr
            EquipmentToAttr (netId, oriEq, 1);
            // TODO: 考虑数据库修改
            // TODO: 向客户端发送装备更替
        }
        private void EquipmentToAttr (int netId, E_EquipmentItem eqObj, int k) {
            E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (unit == null) return;
            // 处理基础属性与强化
            var attrList = eqObj.m_equipmentDe.m_attrList;
            for (int i = 0; i < attrList.Count; i++)
                unit.AddConAttr (attrList[i].Item1, k * eqObj.CalcStrengthenedAttr (attrList[i].Item2));
            // 处理附魔
            var enchantAttr = eqObj.m_enchantAttr;
            foreach (var attr in enchantAttr)
                unit.AddConAttr (attr.Item1, k * attr.Item2);
            // 处理镶嵌
            var gemIdList = eqObj.m_inlaidGemIdList;
            for (int i = 0; i < gemIdList.Count; i++) {
                var gemDe = EM_Item.s_instance.GetGemById (gemIdList[i]);
                for (int j = 0; j < gemDe.m_attrList.Count; j++)
                    unit.AddConAttr (gemDe.m_attrList[j].Item1, k * gemDe.m_attrList[j].Item2);
            }
        }
    }
}