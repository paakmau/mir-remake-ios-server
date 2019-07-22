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
    partial class EM_Item : EntityManagerBase {
        public static EM_Item s_instance;
        private DEM_Item m_dem;
        private ItemFactory m_itemFactory;
        private ItemDynamicDataHelper m_ddh;
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
        /// <summary>初始化新的角色的所有物品</summary>
        public void InitCharacter (
            int netId,
            int charId,
            out E_RepositoryBase bag,
            out E_RepositoryBase storeHouse,
            out E_RepositoryBase eqRegion
        ) {
            // 若角色已经初始化
            if (m_bagDict.ContainsKey (netId)) {
                bag = GetBag (netId);
                storeHouse = GetStoreHouse (netId);
                eqRegion = GetEquiped (netId);
                return;
            }
            // 初始化背包, 仓库, 装备区
            bag = s_entityPool.m_bagPool.GetInstance ();
            storeHouse = s_entityPool.m_storeHousePool.GetInstance ();
            eqRegion = s_entityPool.m_equipmentRegionPool.GetInstance ();

            E_Item[] itemInBag, itemInStoreHouse, itemEquiped;
            m_ddh.GetAndResetCharacterItemInstance (charId, out itemInBag, out itemInStoreHouse, out itemEquiped);
            bag.Reset (itemInBag);
            storeHouse.Reset (itemInStoreHouse);
            eqRegion.Reset (itemEquiped);
            // 索引各区域
            m_bagDict[netId] = bag as E_Bag;
            m_storeHouseDict[netId] = storeHouse as E_StoreHouse;
            m_equipmentRegionDict[netId] = eqRegion as E_EquipmentRegion;

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
            for (int i = 0; i < bag.m_ItemList.Count; i++)
                m_itemFactory.RecycleItem (bag.m_ItemList[i]);
            for (int i = 0; i < storeHouse.m_ItemList.Count; i++)
                m_itemFactory.RecycleItem (storeHouse.m_ItemList[i]);
            for (int i = 0; i < equiped.m_ItemList.Count; i++)
                m_itemFactory.RecycleItem (equiped.m_ItemList[i]);
            s_entityPool.m_bagPool.RecycleInstance (bag);
            s_entityPool.m_storeHousePool.RecycleInstance (storeHouse);
            s_entityPool.m_equipmentRegionPool.RecycleInstance (equiped);

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
        public E_Item CharacterLoseItem (E_Item item, short num, int charId, E_RepositoryBase repo, short pos) {
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
        public E_Item CharacterLoseWholeItem (E_Item item, int charId, E_RepositoryBase repo, short pos) {
            // 持久层
            m_ddh.Delete (item);
            var emptyItem = m_itemFactory.GetEmptyItemInstance ();
            m_ddh.Insert (emptyItem, charId, repo.m_repositoryPlace, pos);
            // 回收实例
            m_itemFactory.RecycleItem (item);
            repo.SetItem (emptyItem, pos);
            return emptyItem;
        }
        public void CharacterSwapItem (int charId, E_RepositoryBase srcRepo, short srcPos, E_Item srcItem, E_RepositoryBase tarRepo, short tarPos, E_Item tarItem) {
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