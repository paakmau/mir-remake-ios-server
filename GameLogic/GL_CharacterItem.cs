using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    class GL_CharacterItem : GameLogicBase {
        public static GL_CharacterItem s_instance;
        public GL_CharacterItem (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyRemoveCharacter (E_Character charObj) {
            EM_Item.s_instance.RemoveCharacter (charObj.m_networkId);
        }
        public void CommandGainItem (int netId, short itemId, short num) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyGainItem (charObj, new List < (short, short) > {
                (itemId, num)
            });
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null || charObj == null) return;
            short posInBag = -1;
            E_ConsumableItem item = bag.GetItemByRealId (realId, out posInBag) as E_ConsumableItem;
            if (item == null) return;
            GL_UnitBattleAttribute.s_instance.NotifyApplyEffect (item.m_consumableDe.m_itemEffect, -1, charObj, charObj);
            NotifyLostItem (charObj, item, 1, posInBag, bag);
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_EquipmentRegion eqRegion = EM_Item.s_instance.GetEquiped (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || eqRegion == null || bag == null) return;
            short posInBag = -1;
            var eq = bag.GetItemByRealId (realId, out posInBag) as E_EquipmentItem;
            if (eq == null) return;
            // 通知角色属性逻辑
            // 该位置原有装备卸下
            var oriEq = eqRegion.GetItemByPosition ((short) eq.m_EquipmentPosition) as E_EquipmentItem;
            if (oriEq != null) {
                GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq, -1));
            }
            // 装备穿上Attr
            GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq, 1));
            NotifySwapItemPlace (charObj, eqRegion, (short) eq.m_EquipmentPosition, oriEq, bag, posInBag, eq);
        }
        /// <summary>
        /// 失去确定位置的物品
        /// </summary>
        /// <param name="charObj"></param>
        /// <param name="item"></param>
        /// <param name="num"></param>
        /// <param name="pos"></param>
        /// <param name="repo"></param>
        public void NotifyLostItem (E_Character charObj, E_Item item, short num, short pos, E_RepositoryBase repo) {
            // 移除num个该物品
            bool runOut = item.RemoveNum (num);
            long realId = item.m_realId;
            short curNum = item.m_num;
            // 实例 与 数据
            if (runOut) {
                // 物品消失
                var empty = EM_Item.s_instance.ItemLose (item, charObj.m_characterId, ItemPlace.BAG, pos);
                repo.RemoveItemByRealId (empty);
            } else
                EM_Item.s_instance.ItemUpdate (item, charObj.m_characterId, ItemPlace.BAG, pos);
            // Client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItemNum.Instance (
                charObj.m_networkId, new List<long> { realId }, new List<short> { curNum }));
        }
        public void NotifySwapItemPlace (E_Character charObj, E_RepositoryBase srcRepo, short srcPos, E_Item srcItem, E_RepositoryBase tarRepo, short tarPos, E_Item tarItem) {
            srcRepo.SetItem (tarItem, srcPos);
            tarRepo.SetItem (srcItem, tarPos);
            m_networkService.SendServerCommand (SC_ApplySelfMoveItem.Instance (
                new List<int> { charObj.m_networkId }, srcRepo.m_repositoryPlace, srcPos, tarRepo.m_repositoryPlace, tarPos));
        }
        public void NotifyGainItem (E_Character charObj, IReadOnlyList < (short, short) > itemIdAndNumList) {
            var bag = EM_Item.s_instance.GetBag (charObj.m_networkId);
            if (bag == null) return;
            // 实例化
            var itemList = EM_Item.s_instance.InitItemList (itemIdAndNumList);
            // 放入背包
            for (int i = 0; i < itemList.Count; i++) {
                var realStoreNum = 0;
                List < (short, E_Item) > changedItemList;
                E_EmptyItem oriBagSlot;
                short storePos = bag.AutoStoreItem (itemList[i], out changedItemList, out realStoreNum, out oriBagSlot);
                // 处理原有物品的堆叠
                // dds 更新
                for (int j = 0; j < changedItemList.Count; j++)
                    EM_Item.s_instance.ItemUpdate (changedItemList[j].Item2, charObj.m_characterId, ItemPlace.BAG, changedItemList[j].Item1);
                // client
                List<long> changedRealIdList = new List<long> (changedItemList.Count);
                List<short> changedPosList = new List<short> (changedItemList.Count);
                for (int j = 0; j < changedItemList.Count; j++) {
                    changedRealIdList.Add (changedItemList[i].Item2.m_realId);
                    changedPosList.Add (changedItemList[i].Item1);
                }
                if (changedItemList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplySelfUpdateItemNum.Instance (
                        charObj.m_networkId, changedRealIdList, changedPosList));

                // 若该物品单独占有一格
                if (storePos != -1 && storePos != -2) {
                    // 回收原有空插槽
                    EM_Item.s_instance.ItemGain (oriBagSlot, itemList[i], charObj.m_characterId, ItemPlace.BAG, storePos);
                    // 基础信息 client
                    m_networkService.SendServerCommand (SC_ApplySelfGainItem.Instance (
                        charObj.m_networkId,
                        new List<NO_Item> { itemList[i].GetItemNo () },
                        new List<ItemPlace> { ItemPlace.BAG },
                        new List<short> { storePos }));
                    // 附加信息 (装备等) client TODO: 考虑改模式
                    if (itemList[i].m_Type == ItemType.EQUIPMENT)
                        m_networkService.SendServerCommand (
                            SC_ApplySelfUpdateEquipment.Instance (
                                charObj.m_networkId, itemList[i].m_realId,
                                ((E_EquipmentItem) itemList[i]).GetEquipmentInfoNo ()));
                }
                // 通知 log
                GL_Log.s_instance.NotifyLog (GameLogType.GAIN_ITEM, charObj.m_networkId, itemList[i].m_ItemId, realStoreNum);
            }
        }
        private List < (ActorUnitConcreteAttributeType, int) > EquipmentToAttrList (E_EquipmentItem eqObj, int k) {
            List < (ActorUnitConcreteAttributeType, int) > res = new List < (ActorUnitConcreteAttributeType, int) > ();
            // 处理强化与基础属性
            var attrList = eqObj.m_equipmentDe.m_attrList;
            for (int i = 0; i < attrList.Count; i++)
                res.Add ((attrList[i].Item1, k * eqObj.CalcStrengthenedAttr (attrList[i].Item2)));
            // 处理附魔
            var enchantAttr = eqObj.m_enchantAttrList;
            foreach (var attr in enchantAttr)
                res.Add ((attr.Item1, k * attr.Item2));
            // 处理镶嵌
            var gemList = eqObj.m_inlaidGemList;
            for (int i = 0; i < gemList.Count; i++) {
                var gemDe = gemList[i];
                for (int j = 0; j < gemDe.m_attrList.Count; j++)
                    res.Add ((gemDe.m_attrList[j].Item1, k * gemDe.m_attrList[j].Item2));
            }
            return res;
        }
    }
}