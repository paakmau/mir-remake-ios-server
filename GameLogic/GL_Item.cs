using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    class GL_Item : GameLogicBase {
        public static GL_Item s_instance;
        public GL_Item (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            // 道具消失
            var disappearedGroundItemIdList = new List<long> ();
            var groundItemList = EM_Item.s_instance.GetRawGroundItemList ();
            for (int i = groundItemList.Count - 1; i >= 0; i--)
                if (MyTimer.CheckTimeUp (groundItemList[i].m_DisappearTime)) {
                    var dsppGroundItem = groundItemList[i];
                    groundItemList.RemoveAt (i);
                }
            // 地面道具视野
            var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
            while (charEn.MoveNext ()) {
                var netId = charEn.Current.Key;
                var sight = EM_Item.s_instance.GetCharacterGroundItemRawSight (netId);
                if (sight == null) continue;
                
            }
        }
        public override void NetworkTick () { }
        public void NotifyRemoveCharacter (int netId) {
            EM_Item.s_instance.RemoveCharacter (netId);
        }
        public void CommandGainItem (int netId, short itemId, short num) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyCharacterGainItem (charObj, new List < (short, short) > {
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
            NotifyCharacterLostItem (charObj, item, 1, posInBag, bag);
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_EquipmentRegion eqRegion = EM_Item.s_instance.GetEquiped (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || eqRegion == null || bag == null) return;
            short posInBag = -1;
            var eq = bag.GetItemByRealId (realId, out posInBag) as E_EquipmentItem;
            if (eq == null) return;
            // 该位置原有装备卸下
            var oriEq = eqRegion.GetItemByPosition ((short) eq.m_EquipmentPosition) as E_EquipmentItem;
            if (oriEq != null) {
                GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq, -1));
            }
            // 装备穿上Attr
            GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq, 1));
            NotifyCharacterSwapItemPlace (charObj, eqRegion, (short) eq.m_EquipmentPosition, oriEq, bag, posInBag, eq);
        }
        public void NotifyInitCharacter (int netId, int charId) {
            E_RepositoryBase bag, storeHouse, eqRegion;
            EM_Item.s_instance.InitCharacter (netId, charId, out bag, out storeHouse, out eqRegion);
            // client
            m_networkService.SendServerCommand (SC_InitSelfItem.Instance (new List<int> () { netId }, bag.GetNo (), storeHouse.GetNo (), eqRegion.GetNo ()));
        }
        /// <summary>
        /// 失去确定位置的物品
        /// </summary>
        public void NotifyCharacterLostItem (E_Character charObj, E_Item item, short num, short pos, E_RepositoryBase repo) {
            // 移除num个该物品
            bool runOut = item.RemoveNum (num);
            long realId = item.m_RealId;
            short curNum = item.m_num;
            // 实例 与 数据
            if (runOut) {
                // 物品消失
                var empty = EM_Item.s_instance.CharacterLoseItem (item, charObj.m_characterId, ItemPlace.BAG, pos);
                repo.RemoveItemByRealId (empty);
            } else
                EM_Item.s_instance.CharacterUpdateItem (item, charObj.m_characterId, ItemPlace.BAG, pos);
            // Client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItemNum.Instance (
                charObj.m_networkId, new List<long> { realId }, new List<short> { curNum }));
        }
        public void NotifyCharacterSwapItemPlace (E_Character charObj, E_RepositoryBase srcRepo, short srcPos, E_Item srcItem, E_RepositoryBase tarRepo, short tarPos, E_Item tarItem) {
            srcRepo.SetItem (tarItem, srcPos);
            tarRepo.SetItem (srcItem, tarPos);
            m_networkService.SendServerCommand (SC_ApplySelfMoveItem.Instance (
                new List<int> { charObj.m_networkId }, srcRepo.m_repositoryPlace, srcPos, tarRepo.m_repositoryPlace, tarPos));
        }
        public void NotifyCharacterGainItem (E_Character charObj, IReadOnlyList < (short, short) > itemIdAndNumList) {
            var bag = EM_Item.s_instance.GetBag (charObj.m_networkId);
            if (bag == null) return;
            // 放入背包
            for (int i = 0; i < itemIdAndNumList.Count; i++) {
                var itemId = itemIdAndNumList[i].Item1;
                var itemNum = itemIdAndNumList[i].Item2;
                short piledNum = 0;
                short realStoreNum = 0;
                List < (short, E_Item) > changedItemList;
                E_EmptyItem oriBagSlot;
                short storePos = bag.AutoStoreItem (itemId, itemNum, out changedItemList, out piledNum, out realStoreNum, out oriBagSlot);
                // 处理原有物品的堆叠
                // dds 更新
                for (int j = 0; j < changedItemList.Count; j++)
                    EM_Item.s_instance.CharacterUpdateItem (changedItemList[j].Item2, charObj.m_characterId, ItemPlace.BAG, changedItemList[j].Item1);
                // client
                List<long> changedRealIdList = new List<long> (changedItemList.Count);
                List<short> changedPosList = new List<short> (changedItemList.Count);
                for (int j = 0; j < changedItemList.Count; j++) {
                    changedRealIdList.Add (changedItemList[i].Item2.m_RealId);
                    changedPosList.Add (changedItemList[i].Item1);
                }
                if (changedItemList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplySelfUpdateItemNum.Instance (
                        charObj.m_networkId, changedRealIdList, changedPosList));

                // 若该物品单独占有一格
                if (storePos != -1 && storePos != -2) {
                    // 回收原有空插槽
                    var itemObj = EM_Item.s_instance.CharacterGainItem (oriBagSlot, itemId, itemNum, charObj.m_characterId, ItemPlace.BAG, storePos);
                    // 基础信息 client
                    m_networkService.SendServerCommand (SC_ApplySelfGainItem.Instance (
                        charObj.m_networkId,
                        new List<NO_Item> { itemObj.GetItemNo () },
                        new List<ItemPlace> { ItemPlace.BAG },
                        new List<short> { storePos }));
                    // 附加信息 (装备等) client TODO: 考虑改模式
                    if (itemObj.m_Type == ItemType.EQUIPMENT)
                        m_networkService.SendServerCommand (
                            SC_ApplySelfUpdateEquipment.Instance (
                                charObj.m_networkId, itemObj.m_RealId,
                                ((E_EquipmentItem) itemObj).GetEquipmentInfoNo ()));
                }
                // 通知 log
                GL_Log.s_instance.NotifyLog (GameLogType.GAIN_ITEM, charObj.m_networkId, itemId, realStoreNum);
            }
        }
        public void NotifyMonsterDropLegacy (E_Monster monObj) {
            IReadOnlyList<short> monLegacyList = monObj.m_DropItemIdList;
            List < (short, short) > dropItemIdAndNumList = new List < (short, short) > ();
            for (int i = 0; i < monLegacyList.Count; i++) {
                short id = monLegacyList[i];
                if (id >= 30000) {
                    dropItemIdAndNumList.Add ((id, (short) (MyRandom.NextInt (1, 2))));
                } else if (id >= 20000) {
                    bool drop = MyRandom.NextInt (0, 1000) <= 100;
                    if (drop)
                        dropItemIdAndNumList.Add ((id, (short) 1));
                }
            }
            EM_Item.s_instance.GenerateItemOnGround (dropItemIdAndNumList);
        }
        public void NotifyCharacterDropLegacy (E_Character charObj) {
            var charBag = EM_Item.s_instance.GetBag (charObj.m_networkId);
            if (charBag == null) return;
            var bagItemList = charBag.m_ItemList;
            List<E_Item> droppedItemList = new List<E_Item> ();
            // TODO: 根据bagItemList (从角色bag中), 角色掉落遗物

            EM_Item.s_instance.DropItemOntoGround (droppedItemList);
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