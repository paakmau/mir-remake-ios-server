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
        private List<int> t_intList;
        public GL_Item (IDDS_Item itemDds, INetworkService netService) : base (netService) {
            m_itemDds = itemDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            var bagDdo = m_itemDds.GetBagByCharacterId (charId);
            var storeHouseDdo = m_itemDds.GetStoreHouseByCharacterId (charId);
            var eqRegionDdo = m_itemDds.GetEquipmentRegionByCharacterId (charId);
            var equipmentDdo = m_itemDds.GetAllEquipmentByCharacterId (charId);
            E_Repository bag, storeHouse;
            E_EquipmentRegion eqRegion;
            EM_Item.s_instance.InitCharacterItem (netId, bagDdo, storeHouseDdo, eqRegionDdo, equipmentDdo, out bag, out storeHouse, out eqRegion);
            // 把bag, storeHouse, equiped, 发送给Client
            t_intList.Clear ();
            t_intList.Add (netId);
            m_networkService.SendServerCommand (SC_InitSelfItem.Instance (t_intList, bag.GetNo(), storeHouse.GetNo (), eqRegion.GetNo ()));
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Item.s_instance.RemoveCharacterItem (netId);
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            E_ConsumableItem item = EM_Item.s_instance.GetItemByRealId (realId) as E_ConsumableItem;
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            E_Character unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (item == null || bag == null || unit == null) return;
            // 移除一个该物品
            bool runOut = item.RemoveNum (1);
            if (runOut) {
                bag.RemoveItem (item.m_realId);
                m_itemDds.DeleteItemByRealId (item.m_realId);
                EM_Item.s_instance.UnloadItem (item);
            } else {
                int pos = bag.GetItemPosition (item.m_realId);
                m_itemDds.UpdateItem (item.GetDdo (unit.m_characterId, ItemPlace.BAG, pos));
            }
            GL_Effect.s_instance.NotifyApplyEffect (item.m_consumableDe.m_itemEffect, unit, unit);
            // TODO: 向客户端发送道具消耗
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_Character charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            E_EquipmentItem equipment = EM_Item.s_instance.GetItemByRealId (realId) as E_EquipmentItem;
            E_EquipmentRegion eqRegion = EM_Item.s_instance.GetEquiped (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (equipment == null || eqRegion == null || bag == null) return;
            // 若背包中找不到该物品
            if (bag.GetItemPosition (realId) == -1) return;
            // 从背包中移除该装备
            bag.RemoveItem (realId);
            // 穿上该装备, 并卸下该位置上原有装备(如果有)
            E_EquipmentItem oriEq = eqRegion.PutOnEquipment (equipment);
            m_itemDds.UpdateItem (equipment.GetDdo (charObj.m_characterId, ItemPlace.EQUIPMENT_REGION, -1));
            // 如果有装备卸下, 存入背包
            if (oriEq != null) {
                int pos = bag.StoreSingleItem (oriEq);
                m_itemDds.UpdateItem (oriEq.GetDdo (charObj.m_characterId, ItemPlace.BAG, pos));
                // 装备被卸下改变Attr
                EquipmentToAttr (charObj, oriEq, -1);
            }
            // 装备穿上Attr
            EquipmentToAttr (charObj, oriEq, 1);
            // TODO: 向客户端发送装备更替
        }
        private void EquipmentToAttr (E_Character unit, E_EquipmentItem eqObj, int k) {
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