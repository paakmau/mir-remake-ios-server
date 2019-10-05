using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    partial class EM_Item {
        public static EM_Item s_instance;
        private DEM_Item m_dem;
        private ItemFactory m_itemFactory;
        private ItemDynamicDataHelper m_ddh;
        private const int c_repositoryPoolSize = 400;
        private const int c_groundItemPoolSize = 2000;
        private const int c_marketItemPoolSize = 2000;
        private const int c_marketPoolSize = 200;
        public ObjectPool<E_Bag> m_bagPool = new ObjectPool<E_Bag> (c_repositoryPoolSize);
        public ObjectPool<E_StoreHouse> m_storeHousePool = new ObjectPool<E_StoreHouse> (c_repositoryPoolSize);
        public ObjectPool<E_EquipmentRegion> m_equipmentRegionPool = new ObjectPool<E_EquipmentRegion> (c_repositoryPoolSize);
        public ObjectPool<E_GroundItem> m_groundItemPool = new ObjectPool<E_GroundItem> (c_groundItemPoolSize);
        public ObjectPool<E_MarketItem> m_marketItemPool = new ObjectPool<E_MarketItem> (c_marketItemPoolSize);
        public ObjectPool<E_Market> m_marketPool = new ObjectPool<E_Market> (c_marketPoolSize);
        private Dictionary<int, E_Bag> m_bagDict = new Dictionary<int, E_Bag> ();
        private Dictionary<int, E_StoreHouse> m_storeHouseDict = new Dictionary<int, E_StoreHouse> ();
        private Dictionary<int, E_EquipmentRegion> m_equipmentRegionDict = new Dictionary<int, E_EquipmentRegion> ();
        public EM_Item (DEM_Item dem, IDDS_Item dds) {
            m_dem = dem;
            m_itemFactory = new ItemFactory (dem);
            m_ddh = new ItemDynamicDataHelper (dem, dds, m_itemFactory);
            // 可再生道具
            var itemIdAndPosList = dem.GetAllRenewableItemList ();
            m_renewableItemList = new List < (short, Vector2, MyTimer.Time) > (itemIdAndPosList.Count);
            for (int i = 0; i < itemIdAndPosList.Count; i++)
                m_renewableItemList.Add ((itemIdAndPosList[i].Item1, itemIdAndPosList[i].Item2, MyTimer.s_CurTime.Ticked (MyRandom.NextFloat (c_renewableItemRefreshTimeMin, c_renewableItemRefreshTimeMax))));
        }

        public void CreateCharacter (int charId) {
            // 背包, 仓库, equipment dds
            short bagSize = 40;
            short storeHouseSize = 8;
            short eqSize = 10;
            for (short i = 0; i < bagSize; i++)
                m_ddh.InsertSlot (charId, ItemPlace.BAG, i);
            for (short i = 0; i < storeHouseSize; i++)
                m_ddh.InsertSlot (charId, ItemPlace.STORE_HOUSE, i);
            for (short i = 0; i < eqSize; i++)
                m_ddh.InsertSlot (charId, ItemPlace.EQUIPMENT_REGION, i);
        }

        /// <summary>初始化新的角色的所有物品</summary>
        public void InitCharacter (
            int netId,
            int charId,
            out E_Bag resBag,
            out E_StoreHouse resStoreHouse,
            out E_EquipmentRegion resEqRegion
        ) {
            // 若角色已经初始化
            if (m_bagDict.ContainsKey (netId)) {
                resBag = GetBag (netId);
                resStoreHouse = GetStoreHouse (netId);
                resEqRegion = GetEquiped (netId);
                return;
            }
            // 初始化背包, 仓库, 装备区
            resBag = m_bagPool.GetInstance ();
            resStoreHouse = m_storeHousePool.GetInstance ();
            resEqRegion = m_equipmentRegionPool.GetInstance ();

            E_Item[] itemInBag, itemInStoreHouse, itemEquiped;
            m_ddh.GetAndResetCharacterItemInstance (charId, out itemInBag, out itemInStoreHouse, out itemEquiped);
            resBag.Reset (itemInBag);
            resStoreHouse.Reset (itemInStoreHouse);
            resEqRegion.Reset (itemEquiped);

            // 索引各区域
            m_bagDict[netId] = resBag as E_Bag;
            m_storeHouseDict[netId] = resStoreHouse as E_StoreHouse;
            m_equipmentRegionDict[netId] = resEqRegion as E_EquipmentRegion;

            // 地面物品视野
            m_characterGroundItemSightDict.TryAdd (netId, new List<E_GroundItem> ());
        }
        public void RemoveCharacter (int netId) {
            E_Bag bag;
            m_bagDict.TryGetValue (netId, out bag);
            E_StoreHouse storeHouse;
            m_storeHouseDict.TryGetValue (netId, out storeHouse);
            E_EquipmentRegion equiped;
            m_equipmentRegionDict.TryGetValue (netId, out equiped);
            if (bag == null || storeHouse == null || equiped == null)
                return;
            // 移除索引
            m_bagDict.Remove (netId);
            m_storeHouseDict.Remove (netId);
            m_equipmentRegionDict.Remove (netId);
            AutoPickOff (netId);
            // 回收
            for (int i = 0; i < bag.m_itemList.Count; i++)
                m_itemFactory.RecycleItem (bag.m_itemList[i]);
            for (int i = 0; i < storeHouse.m_itemList.Count; i++)
                m_itemFactory.RecycleItem (storeHouse.m_itemList[i]);
            for (int i = 0; i < equiped.m_itemList.Count; i++)
                m_itemFactory.RecycleItem (equiped.m_itemList[i]);
            m_bagPool.RecycleInstance (bag);
            m_storeHousePool.RecycleInstance (storeHouse);
            m_equipmentRegionPool.RecycleInstance (equiped);

            // 地面物品视野
            m_characterGroundItemSightDict.Remove (netId);
        }
        public long GetItemBuyPrice (short itemId) {
            var itemDe = m_dem.GetItemById (itemId);
            return itemDe == null ? -1 : itemDe.m_buyPrice;
        }
        /// <summary>
        /// 获取装备区
        /// </summary>
        public E_EquipmentRegion GetEquiped (int netId) {
            E_EquipmentRegion er = null;
            m_equipmentRegionDict.TryGetValue (netId, out er);
            return er;
        }
        public E_Bag GetBag (int netId) {
            E_Bag res = null;
            m_bagDict.TryGetValue (netId, out res);
            return res;
        }
        public E_StoreHouse GetStoreHouse (int netId) {
            E_StoreHouse res = null;
            m_storeHouseDict.TryGetValue (netId, out res);
            return res;
        }
        public short CharacterGainEnchantmentItem (int charId, List < (ActorUnitConcreteAttributeType, int) > attrList, E_Bag bag, out E_Item resStoreItem, out short resStorePos) {
            E_EnchantmentItem ecmt = m_itemFactory.GetEnchantmentItemInstance ();
            ecmt.ResetEnchantmentData (attrList);
            List < (short, E_Item) > changedItemList;
            return GainItem (charId, ecmt, bag, out changedItemList, out resStoreItem, out resStorePos);
        }
        public short CharacterGainItem (int charId, short itemId, short itemNum, E_Bag bag, out List < (short, E_Item) > resChangedItemList, out E_Item resStoreItem, out short resStorePos) {
            E_Item itemObj = m_itemFactory.GetAndInitInstance (itemId, itemNum);
            if (itemObj == null) {
                resChangedItemList = null;
                resStoreItem = null;
                resStorePos = -3;
                return 0;
            }
            return GainItem (charId, itemObj, bag, out resChangedItemList, out resStoreItem, out resStorePos);
        }
        /// <summary>
        /// 返回pos处现在的item
        /// </summary>
        public E_Item CharacterLoseItem (E_Item item, short num, int charId, E_Bag repo, short pos) {
            // 移除num个该物品
            bool runOut = item.RemoveNum (num);
            long realId = item.m_realId;
            short curNum = item.m_num;
            // 实例 与 数据
            if (runOut)
                // 物品消失
                item = CharacterLoseWholeItem (item, charId, repo, pos);
            else
                CharacterUpdateItem (item, charId, repo.m_repositoryPlace, pos);
            return item;
        }
        /// <summary>
        /// 会回收 item  
        /// 返回该位置的 slot  
        /// </summary>
        public E_Item CharacterLoseWholeItem (E_Item item, int charId, E_Bag repo, short pos) {
            // 持久层
            m_ddh.Delete (item);
            var emptyItem = m_itemFactory.GetEmptyItemInstance ();
            m_ddh.Insert (emptyItem, charId, repo.m_repositoryPlace, pos);
            // 回收实例
            m_itemFactory.RecycleItem (item);
            repo.SetItem (emptyItem, pos);
            return emptyItem;
        }
        public void CharacterSwapItem (int charId, E_Bag srcRepo, short srcPos, E_Item srcItem, E_Bag tarRepo, short tarPos, E_Item tarItem) {
            srcRepo.SetItem (tarItem, srcPos);
            tarRepo.SetItem (srcItem, tarPos);
            CharacterUpdateItem (tarItem, charId, srcRepo.m_repositoryPlace, srcPos);
            CharacterUpdateItem (srcItem, charId, tarRepo.m_repositoryPlace, tarPos);
        }
        public void CharacterUpdateItem (E_Item item, int charId, ItemPlace ip, short pos) {
            m_ddh.Save (item, charId, ip, pos);
        }
        /// <summary>
        /// 会为 itemObj 在 dds 中添加记录
        /// </summary>
        private short GainItem (int charId, E_Item itemObj, E_Bag bag, out List < (short, E_Item) > resChangedItemList, out E_Item resStoreItem, out short resStorePos) {
            short piledNum = 0;
            short realStoreNum = 0;
            E_EmptyItem oriBagSlot;
            resStorePos = bag.AutoPileAndStoreItem (itemObj, out resChangedItemList, out piledNum, out realStoreNum, out oriBagSlot);
            // 处理原有物品的堆叠
            // dds 更新
            for (int j = 0; j < resChangedItemList.Count; j++)
                CharacterUpdateItem (resChangedItemList[j].Item2, charId, ItemPlace.BAG, resChangedItemList[j].Item1);

            // 若该物品单独占有一格
            if (resStorePos >= 0) {
                // 实例化 持久层 回收
                m_ddh.Delete (oriBagSlot);
                m_ddh.Insert (itemObj, charId, bag.m_repositoryPlace, resStorePos);
                m_itemFactory.RecycleItem (oriBagSlot);
                resStoreItem = itemObj;
            } else {
                resStoreItem = null;
                m_itemFactory.RecycleItem (itemObj);
            }
            return realStoreNum;
        }
    }
}