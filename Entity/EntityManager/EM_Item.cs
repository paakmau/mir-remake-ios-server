using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity
{
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    partial class EM_Item : EntityManagerBase {
        private class GroundItemIdManager {
            private long m_groundItemIdCnt = 0;
            public long AssignGroundItemId () {
                return ++m_groundItemIdCnt;
            }
        }
        public static EM_Item s_instance;
        private const float c_groundItemDisappearTime = 15;
        private DEM_Item m_dem;
        private ItemFactory m_itemFactory;
        private ItemDynamicDataHelper m_ddh;
        private Dictionary<int, E_Bag> m_bagDict = new Dictionary<int, E_Bag> ();
        private Dictionary<int, E_StoreHouse> m_storeHouseDict = new Dictionary<int, E_StoreHouse> ();
        private Dictionary<int, E_EquipmentRegion> m_equipmentRegionDict = new Dictionary<int, E_EquipmentRegion> ();
        private GroundItemIdManager m_groundItemIdManager = new GroundItemIdManager ();
        private List<E_GroundItem> m_groundItemList = new List<E_GroundItem> ();
        private Dictionary<int, List<E_GroundItem>> m_characterGroundItemSightDict = new Dictionary<int, List<E_GroundItem>> ();
        private List < (short, Vector2, MyTimer.Time) > m_renewableItemList = new List < (short, Vector2, MyTimer.Time) > ();
        private float c_renewableItemRefreshTimeMin = 12;
        private float c_renewableItemRefreshTimeMax = 18;
        private float c_renewableItemRefreshRadian = 2;
        private Dictionary<int, List<E_MarketItem>> m_marketDict;
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
        /// <summary> 
        /// 获取物品的默认单价  
        /// 若物品不可使用默认单价购买返回 -1  
        /// 如 ItemId 不正确返回 -1  
        /// </summary>
        public long GetItemPrice (short itemId) {
            var de = m_dem.GetItemById (itemId);
            if (de == null) return -1;
            return de.m_price;
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
            return CharacterGainItem (charId, ecmt, bag, out changedItemList, out resStoreItem, out resStorePos);
        }
        public short CharacterGainItem (int charId, short itemId, short itemNum, E_Bag bag, out List < (short, E_Item) > resChangedItemList, out E_Item resStoreItem, out short resStorePos) {
            E_Item itemObj = m_itemFactory.GetAndInitInstance (itemId, itemNum);
            return CharacterGainItem (charId, itemObj, bag, out resChangedItemList, out resStoreItem, out resStorePos);
        }
        private short CharacterGainItem (int charId, E_Item itemObj, E_Bag bag, out List < (short, E_Item) > resChangedItemList, out E_Item resStoreItem, out short resStorePos) {
            short piledNum = 0;
            short realStoreNum = 0;
            E_EmptyItem oriBagSlot;
            resStorePos = bag.AutoPileItemAndGetOccupiedPos (itemObj.m_ItemId, itemObj.m_num, out resChangedItemList, out piledNum, out realStoreNum, out oriBagSlot);
            // 处理原有物品的堆叠
            // dds 更新
            for (int j = 0; j < resChangedItemList.Count; j++)
                CharacterUpdateItem (resChangedItemList[j].Item2, charId, ItemPlace.BAG, resChangedItemList[j].Item1);

            // 若该物品单独占有一格
            if (resStorePos >= 0) {
                // 实例化 持久层 回收
                itemObj.m_num -= piledNum;
                if (itemObj == null)
                    itemObj = m_itemFactory.GetEmptyItemInstance ();
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
        public E_Item CharacterLoseItem (E_Item item, int charId, E_RepositoryBase repo, short pos) {
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
        public short CharacterPickGroundItem (int charId, E_GroundItem gndItem, E_Bag bag, out List < (short, E_Item) > resChangedItemList, out E_Item resStoreItem, out short resStorePos) {
            var res = CharacterGainItem (charId, gndItem.m_item, bag, out resChangedItemList, out resStoreItem, out resStorePos);
            // 移除gndItem
            for (int i = 0; i < m_groundItemList.Count; i++)
                if (m_groundItemList[i] == gndItem)
                    m_groundItemList.RemoveAt (i);
            // 回收gndItem
            s_entityPool.m_groundItemPool.RecycleInstance (gndItem);
            return res;
        }
        public void CharacterDropItemOntoGround (E_Item item, short num, int charId, E_RepositoryBase repo, short repoPos, Vector2 gndPos) {
            if (num == 0) return;
            E_GroundItem gndItem = s_entityPool.m_groundItemPool.GetInstance ();
            long gndItemId = m_groundItemIdManager.AssignGroundItemId ();
            if (num >= item.m_num) {
                // 完全丢弃
                E_EmptyItem slot = m_itemFactory.GetEmptyItemInstance ();
                repo.SetItem (slot, repoPos);
                m_ddh.Delete (item);
                m_ddh.Insert (slot, charId, repo.m_repositoryPlace, repoPos);
                item.ResetRealId (-1);
                gndItem.Reset (gndItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), item, -1, gndPos);
            } else {
                // 部分丢弃
                item.RemoveNum (num);
                m_ddh.Save (item, charId, repo.m_repositoryPlace, repoPos);
                var sepItem = m_itemFactory.GetAndInitInstance (item.m_ItemId, num);
                gndItem.Reset (gndItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), sepItem, -1, gndPos);
            }
            m_groundItemList.Add (gndItem);
        }
        public void GenerateGroundItem (IReadOnlyList < (short, short) > itemIdAndNumList, int charId, Vector2 centerPos) {
            for (int i = 0; i < itemIdAndNumList.Count; i++) {
                var pos = centerPos + new Vector2 (0.25f - MyRandom.NextFloat (0, 0.5f), 0.25f - MyRandom.NextFloat (0, 0.5f));
                GenerateGroundItem (itemIdAndNumList[i].Item1, itemIdAndNumList[i].Item2, charId, pos);
            }
        }
        /// <summary>
        /// 创建地面物品
        /// </summary>
        public void GenerateGroundItem (short itemId, short num, int charId, Vector2 pos) {
            var item = m_itemFactory.GetAndInitInstance (itemId, num);
            if (item == null)
                return;
            var gndItem = s_entityPool.m_groundItemPool.GetInstance ();
            long groundItemId = m_groundItemIdManager.AssignGroundItemId ();
            gndItem.Reset (groundItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), item, charId, pos);
            m_groundItemList.Add (gndItem);
        }
        public void GroundItemAutoDisappear () {
            for (int i = m_groundItemList.Count - 1; i >= 0; i--)
                if (MyTimer.CheckTimeUp (m_groundItemList[i].m_disappearTime)) {
                    s_entityPool.m_groundItemPool.RecycleInstance (m_groundItemList[i]);
                    m_itemFactory.RecycleItem (m_groundItemList[i].m_item);
                    m_groundItemList.RemoveAt (i);
                }
        }
        public E_GroundItem GetGroundItem (long gndItemId) {
            for (int i = 0; i < m_groundItemList.Count; i++)
                if (m_groundItemList[i].m_groundItemId == gndItemId)
                    return m_groundItemList[i];
            return null;
        }
        public List<E_GroundItem> GetRawGroundItemList () {
            return m_groundItemList;
        }
        public List<E_GroundItem> GetCharacterGroundItemRawSight (int netId) {
            List<E_GroundItem> res;
            m_characterGroundItemSightDict.TryGetValue (netId, out res);
            return res;
        }
        public void RefreshRenewableItem () {
            for (int i = 0; i < m_renewableItemList.Count; i++) {
                var itemIdPosTime = m_renewableItemList[i];
                if (MyTimer.CheckTimeUp (itemIdPosTime.Item3)) {
                    short itemId = itemIdPosTime.Item1;
                    Vector2 pos = itemIdPosTime.Item2 + new Vector2 (MyRandom.NextFloat (0, c_renewableItemRefreshRadian), MyRandom.NextFloat (0, c_renewableItemRefreshRadian));
                    // 生成地面物品
                    GenerateGroundItem (itemId, 1, -1, pos);

                    // 准备下一次刷新
                    itemIdPosTime.Item3 = MyTimer.s_CurTime.Ticked (MyRandom.NextFloat (c_renewableItemRefreshTimeMin, c_renewableItemRefreshTimeMax));
                    m_renewableItemList[i] = itemIdPosTime;
                }
            }
        }
    }
}