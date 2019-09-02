using System;
using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    partial class EM_Item {
        private Dictionary<int, E_Market> m_marketDict = new Dictionary<int, E_Market> ();
        public E_Market GetMarket (int netId) {
            E_Market res;
            m_marketDict.TryGetValue (netId, out res);
            return res;
        }
        public E_MarketItem GetMarketItem (int netId, long realId, out short pos) {
            pos = -3;
            E_Market market;
            if (!m_marketDict.TryGetValue (netId, out market)) return null;
            return market.GetMarketItemByRealId (realId);
        }
        public bool GetCharacterMarketing (int netId) {
            return m_marketDict.ContainsKey (netId);
        }
        public void CharacterSetUpMarket (int netId, IReadOnlyList < (long, short, long, long) > itemToSellList, out E_Market resMarket) {
            resMarket = null;
            if (m_marketDict.ContainsKey (netId)) return;
            var bag = GetBag (netId);
            if (bag == null) return;
            var marketItemList = new List<E_MarketItem> (itemToSellList.Count);
            for (int i = 0; i < itemToSellList.Count; i++) {
                var item = itemToSellList[i];
                short bagPos;
                var itemObj = bag.GetItemByRealId (item.Item1, out bagPos);
                if (itemObj == null) continue;
                var marketItem = m_marketItemPool.GetInstance ();
                marketItem.Reset (itemObj, Math.Min (itemObj.m_num, item.Item2), item.Item3, item.Item4, bagPos);
                marketItemList.Add (marketItem);
            }
            var market = m_marketPool.GetInstance ();
            market.Reset (marketItemList);
            m_marketDict[netId] = market;
            resMarket = market;
        }
        public void CharacterPackUpMarket (int netId) {
            E_Market market;
            if (!m_marketDict.TryGetValue (netId, out market))
                return;
            for (int i = 0; i < market.m_itemList.Count; i++)
                m_marketItemPool.RecycleInstance (market.m_itemList[i]);
            m_marketDict.Remove (netId);
        }
        public void CharacterBuyItemInMarket (int holderCharId, int buyerNetId, int buyerCharId, E_Market market, E_MarketItem marketItem, short num, short marketPos, E_Bag holderBag, E_Bag buyerBag, out E_Item resHolderItem, out List < (short, E_Item) > resBuyerItem, out E_Item resBuyerStoreItem, out short resBuyerStorePos) {
            var item = marketItem.m_item;
            // 处理背包物品
            if (item.m_num == num) {
                // 整格交易
                var slot = m_itemFactory.GetEmptyItemInstance ();
                holderBag.SetItem (slot, marketItem.m_bagPos);
                m_ddh.Insert (slot, holderCharId, holderBag.m_repositoryPlace, marketItem.m_bagPos);
                m_ddh.Delete (item);
                item.m_realId = -1;
                resHolderItem = slot;
                GainItem (buyerCharId, item, buyerBag, out resBuyerItem, out resBuyerStoreItem, out resBuyerStorePos);
            } else {
                resHolderItem = CharacterLoseItem (item, num, holderCharId, holderBag, marketItem.m_bagPos);
                CharacterGainItem (buyerCharId, item.m_ItemId, num, buyerBag, out resBuyerItem, out resBuyerStoreItem, out resBuyerStorePos);
            }
            // 处理摊位物品
            marketItem.m_onSaleNum -= num;
            if (marketItem.m_onSaleNum <= 0) {
                market.Remove (item.m_realId);
            }
        }
    }
}