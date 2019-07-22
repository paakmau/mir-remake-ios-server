using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    partial class GL_Item : GameLogicBase {
        public static GL_Item s_instance;
        private const float c_groundItemSightRadius = 12;
        private const int c_groundItemSightMaxNum = 31;
        private Dictionary<ItemType, IItemInfoNetworkSender> m_netSenderDict = new Dictionary<ItemType, IItemInfoNetworkSender> ();
        public GL_Item (INetworkService netService) : base (netService) {
            // 实例化所有 IItemInitializer 的子类
            var baseType = typeof (IItemInfoNetworkSender);
            var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
            foreach (var type in implTypes) {
                IItemInfoNetworkSender impl = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IItemInfoNetworkSender;
                m_netSenderDict.Add (impl.m_Type, impl);
            }
        }
        public override void Tick (float dT) {
            // 地面道具消失
            EM_Item.s_instance.GroundItemAutoDisappear ();
            var gndItemList = EM_Item.s_instance.GetRawGroundItemList ();
            // 地面道具视野
            var charEn = EM_Character.s_instance.GetCharacterEnumerator ();
            var charDspprItemIdList = new List<long> ();
            var charShowItemList = new List<NO_GroundItem> ();
            while (charEn.MoveNext ()) {
                var netId = charEn.Current.Key;
                var charObj = charEn.Current.Value;

                var oriSight = EM_Item.s_instance.GetCharacterGroundItemRawSight (netId);
                if (oriSight == null) continue;
                // 计算新视野
                var newSight = new List<E_GroundItem> (oriSight.Count);
                for (int i = 0; i < gndItemList.Count; i++)
                    if ((gndItemList[i].m_position - charObj.m_position).LengthSquared () <= c_groundItemSightRadius * c_groundItemSightRadius) {
                        newSight.Add (gndItemList[i]);
                        if (newSight.Count > c_groundItemSightMaxNum)
                            break;
                    }

                charDspprItemIdList.Clear ();
                charShowItemList.Clear ();
                for (int i = 0; i < oriSight.Count; i++) {
                    long gndItemId = oriSight[i].m_groundItemId;
                    bool removed = true;
                    for (int j = 0; j < newSight.Count; j++)
                        if (newSight[j].m_groundItemId == gndItemId) {
                            removed = false;
                            break;
                        }
                    if (removed)
                        charDspprItemIdList.Add (gndItemId);
                }
                for (int i = 0; i < newSight.Count; i++) {
                    long gndItemId = newSight[i].m_groundItemId;
                    bool isNew = true;
                    for (int j = 0; j < oriSight.Count; j++)
                        if (oriSight[j].m_groundItemId == gndItemId) {
                            isNew = false;
                            break;
                        }
                    if (isNew)
                        charShowItemList.Add (newSight[i].GetNo ());
                }
                oriSight.Clear ();
                for (int i = 0; i < newSight.Count; i++)
                    oriSight.Add (newSight[i]);

                // client
                if (charDspprItemIdList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplyGroundItemDisappear.Instance (netId, charDspprItemIdList));
                if (charShowItemList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplyGroundItemShow.Instance (netId, charShowItemList));
            }
            // 地面可再生道具刷新
            EM_Item.s_instance.RefreshRenewableItem ();
        }
        public override void NetworkTick () { }
        public void CommandApplyBuyItemIntoBag (int netId, short itemId, short num) {
            if (num == 0) return;
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            long needCy = EM_Item.s_instance.GetItemBuyPrice (itemId);
            if (needCy == -1) return;
            needCy *= num;
            long charCy = charObj.m_virtualCurrency;
            if (charCy < needCy) return;
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -needCy);
            NotifyCharacterGainItem (netId, charObj.m_characterId, itemId, num);
        }
        public void CommandApplySellItemInBag (int netId, long realId, short num) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short pos;
            E_Item item = bag.GetItemByRealId (realId, out pos);
            if (item == null)
                return;
            // 失去物品
            NotifyCharacterLoseItem (charObj, item, num, pos, bag);
            // 拿钱
            var virCy = (int) (item.m_SellPrice * num);
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, virCy);
        }
        public void CommandGainItem (int netId, short itemId, short num) {
            var charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyCharacterGainItem (netId, charObj.m_characterId, itemId, num);
        }
        public void CommandPickUpGroundItem (int netId, long gndItemId) {
            var charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            var bag = EM_Item.s_instance.GetBag (netId);
            var gndItem = EM_Item.s_instance.GetGroundItem (gndItemId);
            if (gndItem == null || bag == null || charId == -1) return;
            List < (short, E_Item) > posAndItemChanged;
            E_Item storeItem;
            short storePos;
            EM_Item.s_instance.CharacterPickGroundItem (charId, gndItem, bag, out posAndItemChanged, out storeItem, out storePos);
            // client 更新Bag中原有
            if (posAndItemChanged.Count != 0) {
                var itemNoList = new List<NO_Item> (posAndItemChanged.Count);
                for (int i = 0; i < posAndItemChanged.Count; i++)
                    itemNoList.Add (posAndItemChanged[i].Item2.GetItemNo (ItemPlace.BAG, posAndItemChanged[i].Item1));
                m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, itemNoList));
            }
            // client 整格放入
            if (storeItem != null) {
                // 基础信息 client
                m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (
                    netId,
                    new List<NO_Item> { storeItem.GetItemNo (ItemPlace.BAG, storePos) }));
                // 附加信息 (装备等) client
                m_netSenderDict[storeItem.m_Type].SendItemInfo (storeItem, netId, m_networkService);
            }
        }
        public void CommandDropItemOntoGround (int netId, long realId, short num) {
            var charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            var bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null || charObj == null) return;
            short itemPos;
            var item = bag.GetItemByRealId (realId, out itemPos);
            if (item == null) return;
            if (item.m_num < num) return;
            EM_Item.s_instance.CharacterDropItemOntoGround (item, num, charObj.m_characterId, bag, itemPos, charObj.m_position);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, new List<NO_Item> () { item.GetItemNo (ItemPlace.BAG, itemPos) }));
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null || charObj == null) return;
            short posInBag = -1;
            E_ConsumableItem item = bag.GetItemByRealId (realId, out posInBag) as E_ConsumableItem;
            if (item == null) return;
            GL_UnitBattleAttribute.s_instance.NotifyApplyEffect (item.m_consumableDe.m_itemEffect, -1, charObj, charObj);
            NotifyCharacterLoseItem (charObj, item, 1, posInBag, bag);
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_EquipmentRegion eqRegion = EM_Item.s_instance.GetEquiped (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || eqRegion == null || bag == null) return;
            short posInBag = -1;
            var eq = bag.GetItemByRealId (realId, out posInBag) as E_EquipmentItem;
            if (eq == null) return;
            // 该位置原有装备卸下
            var oriEq = eqRegion.GetEquipmentByEquipPosition (eq.m_EquipmentPosition);
            if (oriEq.m_Type == ItemType.EQUIPMENT)
                GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq as E_EquipmentItem, -1));
            // 装备穿上Attr
            GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (eq, 1));
            NotifyCharacterSwapItemPlace (charObj.m_networkId, charObj.m_characterId, eqRegion, (short) eq.m_EquipmentPosition, oriEq, bag, posInBag, eq);
            // client
            m_networkService.SendServerCommand (SC_ApplyAllChangeEquipment.Instance (EM_Sight.s_instance.GetInSightCharacterNetworkId (netId, true), eq.m_ItemId));
        }
        public void CommandApplyBuildEquipment (int netId, (short, short) [] matArr) {
            // TODO: 打造装备
        }
        /// <summary> 强化装备 </summary>
        public void CommandApplyStrengthenEquipment (int netId, long realId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short eqPos;
            var eq = bag.GetItemByRealId (realId, out eqPos) as E_EquipmentItem;
            if (eq == null) return;
            if (eq.m_strengthenNum == E_EquipmentItem.c_maxStrengthenNum) return;
            var curCy = charObj.m_virtualCurrency;
            long needCy = (1L << eq.m_strengthenNum) * 80L;
            if (needCy > curCy) return;
            // 花钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -needCy);
            // 强化
            eq.m_strengthenNum++;
            EM_Item.s_instance.CharacterUpdateItem (eq, charObj.m_characterId, ItemPlace.BAG, eqPos);
        }
        public void CommandApplyEnchantEquipment (int netId, long eqRealId, long enchantmentRealId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short eqPos, encmPos;
            var eq = bag.GetItemByRealId (eqRealId, out eqPos) as E_EquipmentItem;
            var encm = bag.GetItemByRealId (enchantmentRealId, out encmPos) as E_EnchantmentItem;
            if (eq == null || encm == null) return;
            // 判断钱充足
            var curCy = charObj.m_virtualCurrency;
            long needCy = (1L << (int) encm.m_Quality) * (1L << (eq.m_LevelInNeed >> 4)) * 3L;
            if (needCy > curCy) return;
            // 花钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -needCy);
            // 失去附魔符
            var slot = EM_Item.s_instance.CharacterLoseWholeItem (encm, charObj.m_characterId, bag, encmPos);
            // 附魔
            eq.m_enchantAttrList.Clear ();
            for (int i = 0; i < encm.m_attrList.Count; i++)
                eq.m_enchantAttrList.Add (encm.m_attrList[i]);
            EM_Item.s_instance.CharacterUpdateItem (eq, charObj.m_characterId, ItemPlace.BAG, eqPos);

            // client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, new NO_Item[] { slot.GetItemNo (bag.m_repositoryPlace, encmPos) }));
            m_netSenderDict[eq.m_Type].SendItemInfo (eq, netId, m_networkService);
        }
        /// <summary> 镶嵌宝石 </summary>
        public void CommandApplyInlayGemInEquipment (int netId, long eqRealId, long gemRealId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short eqPos, gemPos;
            var eq = bag.GetItemByRealId (eqRealId, out eqPos) as E_EquipmentItem;
            var gem = bag.GetItemByRealId (gemRealId, out gemPos) as E_GemItem;
            if (eq == null || gem == null) return;
            // 判断钱充足
            var curCy = charObj.m_virtualCurrency;
            long needCy = (1L << (int) gem.m_Quality) * (1L << (eq.m_LevelInNeed >> 5));
            if (needCy > curCy) return;
            // 有插槽
            int gemInlayPos = -1;
            for (int i = 0; i < eq.m_InlaidGemList.Count; i++)
                if (eq.m_InlaidGemList[i] == null) {
                    gemInlayPos = i;
                    break;
                }
            if (gemInlayPos == -1) return;
            // 花钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -needCy);
            // 失去宝石
            var slot = EM_Item.s_instance.CharacterLoseWholeItem (gem, charObj.m_characterId, bag, gemPos);
            // 镶嵌
            eq.InlayGem (gemInlayPos, gem.m_ItemId, gem.m_gemDe);
            EM_Item.s_instance.CharacterUpdateItem (eq, charObj.m_characterId, ItemPlace.BAG, eqPos);

            // client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, new NO_Item[] { slot.GetItemNo (bag.m_repositoryPlace, gemPos) }));
            m_netSenderDict[eq.m_Type].SendItemInfo (eq, netId, m_networkService);
        }
        /// <summary> 装备打孔 </summary>
        public void CommandApplyMakeHoleInEquipment (int netId, long realId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short eqPos;
            var eq = bag.GetItemByRealId (realId, out eqPos) as E_EquipmentItem;
            if (eq == null) return;
            long curCy = charObj.m_virtualCurrency;
            long needCy = (1L << eq.m_InlaidGemList.Count) * 100;
            if (needCy > curCy) return;
            // 花钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -needCy);
            // 打孔
            eq.MakeHole ();
            EM_Item.s_instance.CharacterUpdateItem (eq, charObj.m_characterId, ItemPlace.BAG, eqPos);

            // client
            m_netSenderDict[eq.m_Type].SendItemInfo (eq, netId, m_networkService);
        }
        public void CommandApplyAutoDisjointEquipment (int netId, byte qualities) {
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null) return;
            var itemList = bag.m_ItemList;
            var realIdList = new List<long> (bag.m_ItemList.Count);
            for (int i = 0; i < itemList.Count; i++)
                if (itemList[i].m_Type == ItemType.EQUIPMENT && ((byte) itemList[i].m_Quality & qualities) != 0)
                    realIdList.Add (itemList[i].m_realId);
            for (int i = 0; i < realIdList.Count; i++)
                CommandApplyDisjointEquipment (netId, realIdList[i]);
        }
        /// <summary> 装备分解 </summary>
        public void CommandApplyDisjointEquipment (int netId, long realId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short eqPos;
            E_EquipmentItem eq = bag.GetItemByRealId (realId, out eqPos) as E_EquipmentItem;
            if (eq == null) return;
            long curCy = charObj.m_virtualCurrency;
            long gainCy = (1L << (eq.m_LevelInNeed >> 4)) * 8L;
            // 失去装备
            var slot = EM_Item.s_instance.CharacterLoseWholeItem (eq, charObj.m_characterId, bag, eqPos);
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, new NO_Item[] { slot.GetItemNo (bag.m_repositoryPlace, eqPos) }));
            // 得到钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, gainCy);
        }
        /// <summary> 装备熔炼, 获得附魔符 </summary>
        public void CommandApplySmeltEquipment (int netId, long realId) {
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_Bag bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null) return;
            short eqPos;
            E_EquipmentItem eq = bag.GetItemByRealId (realId, out eqPos) as E_EquipmentItem;
            if (eq == null) return;
            long curCy = charObj.m_chargeCurrency;
            long needCy = ((long) eq.m_Quality + 1L) * 10L;
            if (curCy < needCy) return;
            // 花钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.CHARGE, -needCy);
            // 得到属性
            var attrList = new List < (ActorUnitConcreteAttributeType, int) > ();
            for (int i = 0; i < eq.m_RawAttrList.Count; i++)
                if (MyRandom.NextInt (0, 100) >= 80)
                    attrList.Add (eq.m_RawAttrList[i]);
            // 消耗装备
            var eqSlot = EM_Item.s_instance.CharacterLoseWholeItem (eq, charObj.m_characterId, bag, eqPos);
            // 获取附文
            E_Item ecmt;
            short ecmtPos;
            var ecmtNum = EM_Item.s_instance.CharacterGainEnchantmentItem (charObj.m_characterId, attrList, bag, out ecmt, out ecmtPos);
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, new List<NO_Item> () { eqSlot.GetItemNo (bag.m_repositoryPlace, eqPos), ecmt.GetItemNo (bag.m_repositoryPlace, ecmtPos) }));
            m_netSenderDict[ecmt.m_Type].SendItemInfo (ecmt, netId, m_networkService);
        }
        public void CommandApplyPSetUpMarket (int netId, IReadOnlyList<NO_MarketItem> marketItemNoList) {
            var itemToSellArr = new (long, short, long, long) [marketItemNoList.Count];
            for (int i = 0; i < marketItemNoList.Count; i++)
                itemToSellArr[i] = (marketItemNoList[i].m_realId, marketItemNoList[i].m_onSaleNum, marketItemNoList[i].m_virtualCyPrice, marketItemNoList[i].m_chargeCyPrice);
            E_Market market;
            EM_Item.s_instance.CharacterSetUpMarket (netId, itemToSellArr, out market);
            if (market == null) return;
            List<NO_MarketItem> marketNo = new List<NO_MarketItem> (market.m_itemList.Count);
            for (int i = 0; i < market.m_itemList.Count; i++)
                marketNo.Add (market.m_itemList[i].GetNo ());
            m_networkService.SendServerCommand (SC_ApplySelfSetUpMarket.Instance (netId, marketNo));
        }
        public void CommandApplyPackUpMarket (int netId) {
            EM_Item.s_instance.CharacterPackUpMarket (netId);
            m_networkService.SendServerCommand (SC_ApplySelfPackUpMarket.Instance (netId));
        }
        public void CommandApplyEnterMarket (int netId, int holderNetId) {
            var holder = EM_Character.s_instance.GetCharacterByNetworkId (holderNetId);
            var market = EM_Item.s_instance.GetMarket (holderNetId);
            if (holder == null || market == null) return;
            List<NO_MarketItem> marketNo = new List<NO_MarketItem> (market.m_itemList.Count);
            for (int i = 0; i < market.m_itemList.Count; i++)
                marketNo.Add (market.m_itemList[i].GetNo ());
            m_networkService.SendServerCommand (SC_ApplySelfEnterOtherMarket.Instance (netId, holderNetId, holder.m_name, marketNo));
        }
        public void CommandApplyBuyItemInMarket (int buyerNetId, int holderNetId, long itemRealId, short num, CurrencyType cyType) {
            if (num == 0) return;
            var buyer = EM_Character.s_instance.GetCharacterByNetworkId (buyerNetId);
            var holder = EM_Character.s_instance.GetCharacterByNetworkId (holderNetId);
            var market = EM_Item.s_instance.GetMarket (holderNetId);
            var buyerBag = EM_Item.s_instance.GetBag (buyerNetId);
            var holderBag = EM_Item.s_instance.GetBag (holderNetId);
            if (buyer == null || holder == null || market == null || buyerBag == null || holderBag == null) return;
            short marketPos;
            var marketItem = EM_Item.s_instance.GetMarketItem (holderNetId, itemRealId, out marketPos);
            if (marketItem == null) return;
            long needCy = cyType == CurrencyType.VIRTUAL ? marketItem.m_virtualCyPrice : marketItem.m_chargeCyPrice;
            if (needCy == -1) return;
            if (marketItem.m_ItemNum < num || marketItem.m_onSaleNum < num) return;
            needCy *= num;
            long charCy = cyType == CurrencyType.VIRTUAL ? buyer.m_virtualCurrency : buyer.m_chargeCurrency;
            if (charCy < needCy) return;
            // 花钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (buyer, cyType, -needCy);
            // 收钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (holder, cyType, needCy);
            // 交易物品
            E_Item holderChangedItem;
            List < (short, E_Item) > buyerChangedItemList;
            E_Item buyerStoreItem;
            short buyerStorePos;
            EM_Item.s_instance.CharacterBuyItemInMarket (holder.m_characterId, buyerNetId, buyer.m_characterId, market, marketItem, num, marketPos, holderBag, buyerBag, out holderChangedItem, out buyerChangedItemList, out buyerStoreItem, out buyerStorePos);
            // client
            m_networkService.SendServerCommand (SC_ApplyOtherUpdateMarketItem.Instance (buyerNetId, holderNetId, itemRealId, marketItem.m_onSaleNum));
            m_networkService.SendServerCommand (SC_ApplySelfUpdateMarketItem.Instance (holderNetId, itemRealId, marketItem.m_onSaleNum));
            if (buyerChangedItemList.Count != 0) {
                var changedItemNoArr = new NO_Item[buyerChangedItemList.Count];
                for (int i = 0; i < buyerChangedItemList.Count; i++)
                    changedItemNoArr[i] = buyerChangedItemList[i].Item2.GetItemNo (buyerBag.m_repositoryPlace, buyerChangedItemList[i].Item1);
                m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (buyerNetId, changedItemNoArr));
            }
            if (buyerStoreItem != null) {
                m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (buyerNetId, new NO_Item[] { buyerStoreItem.GetItemNo (buyerBag.m_repositoryPlace, buyerStorePos) }));
                m_netSenderDict[buyerStoreItem.m_Type].SendItemInfo (buyerStoreItem, buyerNetId, m_networkService);
            }
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (holderNetId, new NO_Item[] { holderChangedItem.GetItemNo (holderBag.m_repositoryPlace, marketItem.m_bagPos) }));

        }
        public void NotifyInitCharacter (int netId, int charId) {
            E_RepositoryBase bag, storeHouse, eqRegion;
            EM_Item.s_instance.InitCharacter (netId, charId, out bag, out storeHouse, out eqRegion);
            // client
            m_networkService.SendServerCommand (SC_InitSelfItem.Instance (netId, bag.GetNo (), storeHouse.GetNo (), eqRegion.GetNo ()));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Item.s_instance.RemoveCharacter (netId);
        }
        /// <summary>
        /// 失去确定位置的物品
        /// </summary>
        public void NotifyCharacterLoseItem (E_Character charObj, E_Item item, short num, short pos, E_RepositoryBase repo) {
            item = EM_Item.s_instance.CharacterLoseItem (item, num, charObj.m_characterId, repo, pos);
            // Client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (
                charObj.m_networkId, new List<NO_Item> { item.GetItemNo (repo.m_repositoryPlace, pos) }));
        }
        public void NotifyCharacterSwapItemPlace (int netId, int charId, E_RepositoryBase srcRepo, short srcPos, E_Item srcItem, E_RepositoryBase tarRepo, short tarPos, E_Item tarItem) {
            EM_Item.s_instance.CharacterSwapItem (charId, srcRepo, srcPos, srcItem, tarRepo, tarPos, tarItem);
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId,
                new List<NO_Item> { srcItem.GetItemNo (tarRepo.m_repositoryPlace, tarPos), tarItem.GetItemNo (srcRepo.m_repositoryPlace, srcPos) }));
            m_netSenderDict[srcItem.m_Type].SendItemInfo (srcItem, netId, m_networkService);
            m_netSenderDict[tarItem.m_Type].SendItemInfo (tarItem, netId, m_networkService);
        }
        public void NotifyCharacterGainItems (int netId, int charId, IReadOnlyList < (short, short) > itemIdAndNumList) {
            var bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null) return;
            for (int i = 0; i < itemIdAndNumList.Count; i++)
                NotifyCharacterGainItem (netId, charId, itemIdAndNumList[i].Item1, itemIdAndNumList[i].Item2);
        }
        public void NotifyCharacterGainItem (int netId, int charId, short itemId, short itemNum) {
            var bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null) return;
            List < (short, E_Item) > changedItemList;
            E_Item storeItem;
            short storePos;
            var realStoreNum = EM_Item.s_instance.CharacterGainItem (charId, itemId, itemNum, bag, out changedItemList, out storeItem, out storePos);
            // client
            if (changedItemList.Count != 0) {
                List<NO_Item> changedItemNoList = new List<NO_Item> (changedItemList.Count);
                for (int j = 0; j < changedItemList.Count; j++)
                    changedItemNoList.Add (changedItemList[j].Item2.GetItemNo (ItemPlace.BAG, changedItemList[j].Item1));
                m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (netId, changedItemNoList));
            }
            // client 基础信息 与附加信息
            if (storeItem != null) {
                m_networkService.SendServerCommand (SC_ApplySelfUpdateItem.Instance (
                    netId,
                    new List<NO_Item> { storeItem.GetItemNo (ItemPlace.BAG, storePos) }));
                m_netSenderDict[storeItem.m_Type].SendItemInfo (storeItem, netId, m_networkService);
            }

            // 通知 log
            GL_MissionLog.s_instance.NotifyLog (MissionTargetType.GAIN_ITEM, netId, itemId, realStoreNum);
        }
        public void NotifyMonsterDropLegacy (E_Monster monObj, E_Unit killer) {
            IReadOnlyList<short> monLegacyList = monObj.m_DropItemIdList;
            List < (short, short) > dropItemIdAndNumList = new List < (short, short) > ();
            for (int i = 0; i < monLegacyList.Count; i++) {
                short id = monLegacyList[i];
                if (id >= 30000) {
                    if (id == 30003) {
                        dropItemIdAndNumList.Add ((id, (short) 1));
                    } else {
                        dropItemIdAndNumList.Add ((id, (short) (MyRandom.NextInt (1, 2))));
                    }
                } else if (id >= 20000) {
                    int level = (id - 20000) % 220 / 22 + 1;
                    bool drop = MyRandom.NextInt (0, 1000) <= 15;
                    if (level == 10) {
                        drop = MyRandom.NextInt (0, 1000) <= 5;
                    }
                    if (drop)
                        dropItemIdAndNumList.Add ((id, (short) 1));
                } else {
                    bool drop = MyRandom.NextInt (0, 10000) <= 250;
                    if (drop)
                        dropItemIdAndNumList.Add ((id, (short) 1));
                }
            }
            int charId = (killer.m_UnitType == ActorUnitType.PLAYER) ? ((E_Character) killer).m_characterId : -1;
            EM_Item.s_instance.GenerateGroundItem (dropItemIdAndNumList, charId, monObj.m_position);
        }
        public void NotifyCharacterDropLegacy (E_Character charObj, E_Unit killer) {
            var charBag = EM_Item.s_instance.GetBag (charObj.m_networkId);
            if (charBag == null) return;
            var bagItemList = charBag.m_ItemList;
            for (int i = 0; i < bagItemList.Count; i++) {
                int dropFlag = MyRandom.NextInt (1, 1001);
                if (dropFlag >= 25) continue;
                int dropNum = MyRandom.NextInt (1, bagItemList[i].m_num + 1);
                EM_Item.s_instance.CharacterDropItemOntoGround (bagItemList[i], (short) dropNum, charObj.m_characterId, charBag, (short) i, charObj.m_position);
            }
        }
        private List < (ActorUnitConcreteAttributeType, int) > EquipmentToAttrList (E_EquipmentItem eqObj, int k) {
            List < (ActorUnitConcreteAttributeType, int) > res = new List < (ActorUnitConcreteAttributeType, int) > ();
            // 处理强化与基础属性
            var attrList = eqObj.m_RawAttrList;
            for (int i = 0; i < attrList.Count; i++)
                res.Add ((attrList[i].Item1, k * eqObj.CalcStrengthenedAttr (attrList[i].Item2)));
            // 处理附魔
            var enchantAttr = eqObj.m_enchantAttrList;
            foreach (var attr in enchantAttr)
                res.Add ((attr.Item1, k * attr.Item2));
            // 处理镶嵌
            var gemList = eqObj.m_InlaidGemList;
            for (int i = 0; i < gemList.Count; i++) {
                var gemDe = gemList[i];
                for (int j = 0; j < gemDe.m_attrList.Count; j++)
                    res.Add ((gemDe.m_attrList[j].Item1, k * gemDe.m_attrList[j].Item2));
            }
            return res;
        }
    }
}