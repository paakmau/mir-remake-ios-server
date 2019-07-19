using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    class EM_Item : EntityManagerBase {
        private class ItemFactory {
            #region ItemInitializer
            interface IItemInitializer {
                ItemType m_ItemType { get; }
                void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num);
            }
            private class II_Empty : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    ((E_EmptyItem) resItem).Reset (de);
                }
            }
            private class II_Material : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    ((E_MaterialItem) resItem).Reset (de, num);
                }
            }
            private class II_Consumable : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    var conDe = dem.GetConsumableById (de.m_id);
                    ((E_ConsumableItem) resItem).Reset (de, conDe, num);
                }
            }
            private class II_Equipment : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    var eqDe = dem.GetEquipmentById (de.m_id);
                    ((E_EquipmentItem) resItem).Reset (de, eqDe);
                }
            }
            private class II_Gem : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    var gemDe = dem.GetGemById (de.m_id);
                    ((E_GemItem) resItem).Reset (de, gemDe);
                }
            }
            private class II_Enchantment : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.ENCHANTMENT; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    ((E_EnchantmentItem) resItem).Reset (de);
                }
            }
            #endregion
            private DEM_Item m_dem;
            private Dictionary<ItemType, ObjectPool> m_poolDict = new Dictionary<ItemType, ObjectPool> ();
            private Dictionary<ItemType, IItemInitializer> m_itemInitializerDict = new Dictionary<ItemType, IItemInitializer> ();
            private const int c_poolSize = 2000;
            public ItemFactory (DEM_Item dem) {
                m_dem = dem;
                m_poolDict.Add (ItemType.EMPTY, new ObjectPool<E_EmptyItem> (c_poolSize));
                m_poolDict.Add (ItemType.MATERIAL, new ObjectPool<E_MaterialItem> (c_poolSize));
                m_poolDict.Add (ItemType.CONSUMABLE, new ObjectPool<E_ConsumableItem> (c_poolSize));
                m_poolDict.Add (ItemType.EQUIPMENT, new ObjectPool<E_EquipmentItem> (c_poolSize));
                m_poolDict.Add (ItemType.GEM, new ObjectPool<E_GemItem> (c_poolSize));
                m_poolDict.Add (ItemType.ENCHANTMENT, new ObjectPool<E_EnchantmentItem> (c_poolSize));
                // 实例化所有 IItemInitializer 的子类
                var baseType = typeof (IItemInitializer);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemInitializer impl = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IItemInitializer;
                    m_itemInitializerDict.Add (impl.m_ItemType, impl);
                }
            }
            public void RecycleItem (E_Item item) {
                m_poolDict[item.m_Type].RecycleInstance (item);
            }
            private E_Item GetInstance (ItemType type) {
                return m_poolDict[type].GetInstanceObj () as E_Item;
            }
            public E_EmptyItem GetEmptyItemInstance () {
                return GetAndInitInstance (-1, 0) as E_EmptyItem;
            }
            /// <summary>
            /// 获得物品实例
            /// </summary>
            public E_Item GetAndInitInstance (short itemId, short num) {
                var de = m_dem.GetItemById (itemId);
                if (de == null) return null;
                var initializer = m_itemInitializerDict[de.m_type];
                var res = GetInstance (de.m_type);
                initializer.Initialize (m_dem, res, de, num);
                return res;
            }
        }
        private class ItemDynamicDataHelper {
            #region ItemDealer
            private class ItemInfoDdoCollections {
                private Dictionary<long, DDO_EquipmentInfo> m_eqInfoDict = new Dictionary<long, DDO_EquipmentInfo> ();
                private Dictionary<long, DDO_EnchantmentInfo> m_ecmtInfoDict = new Dictionary<long, DDO_EnchantmentInfo> ();
                public void Reset (List<DDO_EquipmentInfo> eqInfoDdoList, List<DDO_EnchantmentInfo> ecmtInfoDdoList) {
                    m_eqInfoDict.Clear ();
                    for (int i = 0; i < eqInfoDdoList.Count; i++)
                        m_eqInfoDict.Add (eqInfoDdoList[i].m_realId, eqInfoDdoList[i]);
                    for (int i = 0; i < ecmtInfoDdoList.Count; i++)
                        m_ecmtInfoDict.Add (ecmtInfoDdoList[i].m_realId, ecmtInfoDdoList[i]);
                }
                public bool TryGetEquipment (long realId, out DDO_EquipmentInfo resInfo) {
                    return m_eqInfoDict.TryGetValue (realId, out resInfo);
                }
                public bool TryGetEnchantment (long realId, out DDO_EnchantmentInfo resInfo) {
                    return m_ecmtInfoDict.TryGetValue (realId, out resInfo);
                }
            }
            private interface IItemDealer {
                ItemType m_ItemType { get; }
                long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos);
                void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos);
                void Delete (IDDS_Item dds, E_Item item);
                void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem);
            }
            private class II_Empty : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Material : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Consumable : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Equipment : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertEquipmentInfo (((E_EquipmentItem) item).GetEquipmentInfoDdo (charId));
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                    dds.UpdateEquipmentInfo ((item as E_EquipmentItem).GetEquipmentInfoDdo (charId));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                    dds.DeleteEquipmentInfoByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) {
                    resItem.ResetRealId (realId);
                    DDO_EquipmentInfo eqDdo;
                    if (!collct.TryGetEquipment (realId, out eqDdo)) {
                        (resItem as E_EquipmentItem).ResetEquipmentData (0, new (ActorUnitConcreteAttributeType, int) [0], new List<short> (), new List<DE_GemData> ());
                        return;
                    }
                    var gemList = new List<DE_GemData> (eqDdo.m_inlaidGemIdList.Count);
                    for (int i = 0; i < eqDdo.m_inlaidGemIdList.Count; i++)
                        gemList.Add (dem.GetGemById (eqDdo.m_inlaidGemIdList[i]));
                    (resItem as E_EquipmentItem).ResetEquipmentData (eqDdo.m_strengthNum, eqDdo.m_enchantAttr, eqDdo.m_inlaidGemIdList, gemList);
                }
            }
            private class II_Gem : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Enchantment : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.ENCHANTMENT; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertEnchantmentInfo ((item as E_EnchantmentItem).GetEnchantmentDdoInfo (charId));
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                    dds.UpdateEnchantmentInfo ((item as E_EnchantmentItem).GetEnchantmentDdoInfo (charId));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                    dds.DeleteEnchantmentInfoByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) {
                    resItem.ResetRealId (realId);
                    DDO_EnchantmentInfo info;
                    if (!collct.TryGetEnchantment (realId, out info)) {
                        (resItem as E_EnchantmentItem).ResetEnchantmentData (new List < (ActorUnitConcreteAttributeType, int) > ());
                        return;
                    }
                    (resItem as E_EnchantmentItem).ResetEnchantmentData (new List < (ActorUnitConcreteAttributeType, int) > (info.m_attrArr));
                }
            }
            #endregion
            private DEM_Item m_dem;
            private IDDS_Item m_dds;
            private ItemFactory m_fact;
            private ItemInfoDdoCollections m_itemInfoDdoCollections = new ItemInfoDdoCollections ();
            private Dictionary<ItemType, IItemDealer> m_dealerDict = new Dictionary<ItemType, IItemDealer> ();
            public ItemDynamicDataHelper (DEM_Item dem, IDDS_Item dds, ItemFactory fact) {
                m_dem = dem;
                m_dds = dds;
                m_fact = fact;
                // 实例化所有 IItemDealer 的子类
                var baseType = typeof (IItemDealer);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemDealer impl = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IItemDealer;
                    m_dealerDict.Add (impl.m_ItemType, impl);
                }
            }
            /// <summary>
            /// 返回realId  
            /// 同时item的realId也会被更新
            /// </summary>
            public long Insert (E_Item item, int charId, ItemPlace ip, short pos) {
                var res = m_dealerDict[item.m_Type].Insert (m_dds, item, charId, ip, pos);
                item.ResetRealId (res);
                return res;
            }
            public void Save (E_Item item, int charId, ItemPlace ip, short pos) {
                m_dealerDict[item.m_Type].Save (m_dds, item, charId, ip, pos);
            }
            public void Delete (E_Item item) {
                m_dealerDict[item.m_Type].Delete (m_dds, item);
            }
            /// <summary>
            /// 在加载角色初始信息时, 获得物品实例, 有 RealId 与 ItemInfo
            /// </summary>
            public bool GetAndResetCharacterItemInstance (int charId, out E_Item[] resBag, out E_Item[] resStoreHouse, out E_Item[] resEquiped) {
                resBag = null;
                resStoreHouse = null;
                resEquiped = null;
                var eqInfoList = m_dds.GetAllEquipmentByCharacterId (charId);
                var ecmtInfoList = m_dds.GetAllEnchantmentByCharacterId (charId);
                var bagList = m_dds.GetBagByCharacterId (charId);
                var storeHouseList = m_dds.GetStoreHouseByCharacterId (charId);
                var equipedList = m_dds.GetEquipmentRegionByCharacterId (charId);
                if (eqInfoList == null || bagList == null || storeHouseList == null || equipedList == null)
                    return false;
                // 排序
                bagList.Sort ((a, b) => { return a.m_position - b.m_position; });
                storeHouseList.Sort ((a, b) => { return a.m_position - b.m_position; });
                equipedList.Sort ((a, b) => { return a.m_position - b.m_position; });
                // 获取实例
                m_itemInfoDdoCollections.Reset (eqInfoList, ecmtInfoList);
                resBag = GetAndResetInstanceArr (bagList, m_itemInfoDdoCollections);
                resStoreHouse = GetAndResetInstanceArr (storeHouseList, m_itemInfoDdoCollections);
                resEquiped = GetAndResetInstanceArr (equipedList, m_itemInfoDdoCollections);
                return true;
            }
            private E_Item[] GetAndResetInstanceArr (List<DDO_Item> itemList, ItemInfoDdoCollections iidc) {
                E_Item[] res = new E_Item[itemList.Count];
                for (int i = 0; i < itemList.Count; i++) {
                    var itemObj = m_fact.GetAndInitInstance (itemList[i].m_itemId, itemList[i].m_num);
                    if (itemObj == null)
                        itemObj = m_fact.GetEmptyItemInstance ();
                    m_dealerDict[itemObj.m_Type].ResetInfo (m_dem, iidc, itemList[i].m_realId, itemObj);
                    res[i] = itemObj;
                }
                return res;
            }
        }
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
        private float c_renewableItemRefreshRadian = 15;
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
        public E_EnchantmentItem CharacterGainEnchantmentItem (E_EmptyItem oriSlot, short itemId, List < (ActorUnitConcreteAttributeType, int) > attrList, int charId, ItemPlace ip, short pos) {
            E_EnchantmentItem item = m_itemFactory.GetAndInitInstance (itemId, 1) as E_EnchantmentItem;
            if (item == null)
                return null;
            item.ResetEnchantmentData (attrList);
            CharacterGainItem (oriSlot, item, charId, ip, pos);
            return item;
        }
        public E_Item CharacterGainItem (E_EmptyItem oriSlot, short itemId, short itemNum, int charId, ItemPlace ip, short pos) {
            // 生成实例
            E_Item item = m_itemFactory.GetAndInitInstance (itemId, itemNum);
            if (item == null)
                item = m_itemFactory.GetEmptyItemInstance ();
            CharacterGainItem (oriSlot, item, charId, ip, pos);
            return item;
        }
        public void CharacterGainItem (E_EmptyItem oriSlot, E_Item item, int charId, ItemPlace ip, short pos) {
            // 持久层
            m_ddh.Delete (oriSlot);
            m_ddh.Insert (item, charId, ip, pos);
            // 回收实例
            m_itemFactory.RecycleItem (oriSlot);
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
        public void GenerateItemOnGround (IReadOnlyList < (short, short) > itemIdAndNumList, int charId, Vector2 centerPos) {
            for (int i = 0; i < itemIdAndNumList.Count; i++) {
                // TODO: 随机掉落需要优化
                var pos = centerPos + new Vector2 (MyRandom.NextFloat (0, 0.5f)-0.25f, 0.25f-MyRandom.NextFloat (0, 0.5f));
                GenerateItemOnGround (itemIdAndNumList[i].Item1, itemIdAndNumList[i].Item2, charId, pos);
            }
        }
        /// <summary>
        /// 创建地面物品
        /// </summary>
        public void GenerateItemOnGround (short itemId, short num, int charId, Vector2 centerPos) {
            var item = m_itemFactory.GetAndInitInstance (itemId, num);
            if (item == null)
                return;
            var gndItem = s_entityPool.m_groundItemPool.GetInstance ();
            long groundItemId = m_groundItemIdManager.AssignGroundItemId ();
            gndItem.Reset (groundItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), item, charId, centerPos);
            m_groundItemList.Add (gndItem);
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
        public void ItemOnGroundAutoDisappear () {
            for (int i = m_groundItemList.Count - 1; i >= 0; i--)
                if (MyTimer.CheckTimeUp (m_groundItemList[i].m_disappearTime)) {
                    s_entityPool.m_groundItemPool.RecycleInstance (m_groundItemList[i]);
                    m_itemFactory.RecycleItem (m_groundItemList[i].m_item);
                    m_groundItemList.RemoveAt (i);
                }
        }
        public void ItemOnGroundPicked (E_GroundItem groundItem) {
            // 移除gndItem
            for (int i = 0; i < m_groundItemList.Count; i++)
                if (m_groundItemList[i] == groundItem)
                    m_groundItemList.RemoveAt (i);
            // 回收gndItem
            s_entityPool.m_groundItemPool.RecycleInstance (groundItem);
            // 回收Item
            m_itemFactory.RecycleItem (groundItem.m_item);
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
                    GenerateItemOnGround (itemId, 1, -1, pos);

                    // 准备下一次刷新
                    itemIdPosTime.Item3 = MyTimer.s_CurTime.Ticked (MyRandom.NextFloat (c_renewableItemRefreshTimeMin, c_renewableItemRefreshTimeMax));
                    m_renewableItemList[i] = itemIdPosTime;
                }
            }
        }
    }
}