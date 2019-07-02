using System;
using System.Collections.Generic;
using System.Linq;
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
            #endregion
            private DEM_Item m_dem;
            private Dictionary<ItemType, ObjectPool> m_itemPoolDict = new Dictionary<ItemType, ObjectPool> ();
            private Dictionary<ItemType, IItemInitializer> m_itemInitializerDict = new Dictionary<ItemType, IItemInitializer> ();
            private const int c_emptyItemPoolSize = 100000;
            private const int c_materialItemPoolSize = 100000;
            private const int c_consumableItemPoolSize = 100000;
            private const int c_equipmentItemPoolSize = 100000;
            private const int c_gemItemPoolSize = 100000;
            public ItemFactory (DEM_Item dem) {
                m_dem = dem;
                m_itemPoolDict.Add (ItemType.EMPTY, new ObjectPool<E_EmptyItem> (c_emptyItemPoolSize));
                m_itemPoolDict.Add (ItemType.MATERIAL, new ObjectPool<E_MaterialItem> (c_materialItemPoolSize));
                m_itemPoolDict.Add (ItemType.CONSUMABLE, new ObjectPool<E_ConsumableItem> (c_consumableItemPoolSize));
                m_itemPoolDict.Add (ItemType.EQUIPMENT, new ObjectPool<E_EquipmentItem> (c_equipmentItemPoolSize));
                m_itemPoolDict.Add (ItemType.GEM, new ObjectPool<E_GemItem> (c_gemItemPoolSize));
                // 实例化所有 IItemInitializer 的子类
                var baseType = typeof (IItemInitializer);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemInitializer impl = type.GetConstructor (new Type[] { typeof (DEM_Item) }).Invoke (null) as IItemInitializer;
                    m_itemInitializerDict.Add (impl.m_ItemType, impl);
                }
            }
            public void RecycleItem (E_Item item) {
                m_itemPoolDict[item.m_Type].RecycleInstance (item);
            }
            private E_Item GetInstance (ItemType type) {
                return m_itemPoolDict[type].GetInstanceObj () as E_Item;
            }
            public E_EmptyItem GetEmptyItemInstance () {
                return GetAndInitInstance (m_dem.GetEmptyItem (), 0) as E_EmptyItem;
            }
            /// <summary>
            /// 获得物品实例
            /// </summary>
            public E_Item GetAndInitInstance (DE_Item de, short num) {
                var initializer = m_itemInitializerDict[de.m_type];
                var res = GetInstance (de.m_type);
                initializer.Initialize (m_dem, res, de, num);
                return res;
            }
        }
        private class ItemDynamicDataHelper {
            #region ItemInserter
            private interface IItemInserter {
                ItemType m_ItemType { get; }
                void Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos);
            }
            private class II_Empty : IItemInserter {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public void Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            private class II_Material : IItemInserter {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public void Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            private class II_Consumable : IItemInserter {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public void Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            private class II_Equipment : IItemInserter {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public void Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                    dds.InsertEquipmentInfo (((E_EquipmentItem) item).GetEquipmentInfoDdo (charId));
                }
            }
            private class II_Gem : IItemInserter {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public void Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            #endregion
            #region ItemSaver
            private interface IItemSaver {
                ItemType m_ItemType { get; }
                void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos);
            }
            private class IS_Empty : IItemSaver {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            private class IS_Material : IItemSaver {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            private class IS_Consumable : IItemSaver {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            private class IS_Equipment : IItemSaver {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                    dds.UpdateEquipmentInfo (((E_EquipmentItem) item).GetEquipmentInfoDdo (charId));
                }
            }
            private class IS_Gem : IItemSaver {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
            }
            #endregion
            #region ItemDeleter
            private interface IItemDeleter {
                ItemType m_ItemType { get; }
                void Delete (IDDS_Item dds, E_Item item);
            }
            private class ID_Empty : IItemDeleter {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
            }
            private class ID_Material : IItemDeleter {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
            }
            private class ID_Consumable : IItemDeleter {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
            }
            private class ID_Equipment : IItemDeleter {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                    dds.DeleteEquipmentInfoByRealId (item.m_realId);
                }
            }
            private class ID_Gem : IItemDeleter {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
            }
            #endregion
            #region ItemInfoReseter
            private class ItemInfoDdoCollections {
                private Dictionary<long, DDO_EquipmentInfo> m_eqInfoDict = new Dictionary<long, DDO_EquipmentInfo> ();
                public void Reset (List<DDO_EquipmentInfo> eqInfoDdoList) {
                    m_eqInfoDict.Clear ();
                    for (int i = 0; i < eqInfoDdoList.Count; i++)
                        m_eqInfoDict.Add (eqInfoDdoList[i].m_realId, eqInfoDdoList[i]);
                }
                public DDO_EquipmentInfo GetEquipment (long realId) {
                    DDO_EquipmentInfo res;
                    m_eqInfoDict.TryGetValue (realId, out res);
                    return res;
                }
            }
            private interface IItemInfoReseter {
                ItemType m_ItemType { get; }
                void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem);
            }
            private class IIR_Empty : IItemInfoReseter {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.m_realId = realId; }
            }
            private class IIR_Material : IItemInfoReseter {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.m_realId = realId; }
            }
            private class IIR_Consumable : IItemInfoReseter {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.m_realId = realId; }
            }
            private class IIR_Equipment : IItemInfoReseter {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) {
                    var eqDdo = collct.GetEquipment (realId);
                    var gemList = new List<DE_GemData> (eqDdo.m_inlaidGemIdList.Count);
                    for (int i = 0; i < eqDdo.m_inlaidGemIdList.Count; i++)
                        gemList.Add (dem.GetGemById (eqDdo.m_inlaidGemIdList[i]));
                    ((E_EquipmentItem) resItem).ResetEquipmentInfo (eqDdo.m_strengthNum, eqDdo.m_enchantAttr, eqDdo.m_inlaidGemIdList, gemList);
                }
            }
            private class IIR_Gem : IItemInfoReseter {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.m_realId = realId; }
            }
            #endregion
            private DEM_Item m_dem;
            private IDDS_Item m_dds;
            private Dictionary<ItemType, IItemInserter> m_inserterDict = new Dictionary<ItemType, IItemInserter> ();
            private Dictionary<ItemType, IItemSaver> m_saverDict = new Dictionary<ItemType, IItemSaver> ();
            private Dictionary<ItemType, IItemDeleter> m_deleterDict = new Dictionary<ItemType, IItemDeleter> ();
            private ItemInfoDdoCollections m_itemInfoDdoCollections = new ItemInfoDdoCollections ();
            private Dictionary<ItemType, IItemInfoReseter> m_itemInfoReseterDict = new Dictionary<ItemType, IItemInfoReseter> ();
            public ItemDynamicDataHelper (DEM_Item dem, IDDS_Item dds) {
                m_dem = dem;
                m_dds = dds;
                // 实例化所有 IItemInserter 的子类
                var baseType = typeof (IItemInserter);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemInserter impl = type.GetConstructor (new Type[] { typeof (DEM_Item) }).Invoke (null) as IItemInserter;
                    m_inserterDict.Add (impl.m_ItemType, impl);
                }
                // 实例化所有 IItemSaver 的子类
                baseType = typeof (IItemSaver);
                implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemSaver impl = type.GetConstructor (new Type[] { typeof (DEM_Item) }).Invoke (null) as IItemSaver;
                    m_saverDict.Add (impl.m_ItemType, impl);
                }
                // 实例化所有 IItemDeleter 的子类
                baseType = typeof (IItemDeleter);
                implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemDeleter impl = type.GetConstructor (new Type[] { typeof (DEM_Item) }).Invoke (null) as IItemDeleter;
                    m_deleterDict.Add (impl.m_ItemType, impl);
                }
                // 实例化所有 IItemInfoReseter 的子类
                baseType = typeof (IItemInfoReseter);
                implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemInfoReseter impl = type.GetConstructor (new Type[] { typeof (DEM_Item) }).Invoke (null) as IItemInfoReseter;
                    m_itemInfoReseterDict.Add (impl.m_ItemType, impl);
                }
            }
            public void Save (E_Item item, int charId, ItemPlace ip, short pos) {
                var saver = m_saverDict[item.m_Type];
                saver.Save (m_dds, item, charId, ip, pos);
            }
            public void Delete (E_Item item) {

            }
            /// <summary>
            /// 在加载角色初始信息时, 获得物品实例, 有 RealId 与 ItemInfo
            /// </summary>
            public bool GetAndResetInstanceArr (int charId, out E_Item[] resBag, out E_Item[] resStoreHouse, out E_Item[] resEquiped) {
                resBag = null;
                resStoreHouse = null;
                resEquiped = null;
                var eqInfoList = m_dds.GetAllEquipmentByCharacterId (charId);
                var bagList = m_dds.GetBagByCharacterId (charId);
                var storeHouseList = m_dds.GetStoreHouseByCharacterId (charId);
                var equipedList = m_dds.GetEquipmentRegionByCharacterId (charId);
                if (eqInfoList == null || bagList == null || storeHouseList == null || )
                m_itemInfoDdoCollections.Reset (eqInfoList);
                for (int i = 0; i < itemList.Count; i++) {
                    var de = m_dem.GetItemById (itemList[i].m_itemId);
                    var initlzer = m_itemInitializerDict[de.m_type];
                    var infoReseter = m_itemInfoReseterDict[de.m_type];
                    var itemObj = GetInstance (de.m_type);
                    initlzer.Initialize (m_dem, itemObj, de, itemList[i].m_num);
                    infoReseter.ResetInfo (m_dem, m_itemInfoDdoCollections, itemList[i].m_realId, itemObj);
                    res[i] = itemObj;
                }
                return true;
            }
        }
        public static EM_Item s_instance;
        private DEM_Item m_dem;
        private IDDS_Item m_dds;
        private ItemFactory m_itemFactory;
        private Dictionary<int, E_Repository> m_bagDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_Repository> m_storeHouseDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_EquipmentRegion> m_equipmentRegionDict = new Dictionary<int, E_EquipmentRegion> ();
        public EM_Item (DEM_Item dem, IDDS_Item dds) {
            m_dem = dem;
            m_dds = dds;
            m_itemFactory = new ItemFactory (dem);
        }
        /// <summary>
        /// 初始化新的角色的所有物品
        /// </summary>
        /// <param name="netId"></param>
        /// <param name="bagDdo"></param>
        /// <param name="storeHouseDdo"></param>
        /// <param name="equipedDdo"></param>
        /// <param name="allEquipmentDdoList"></param>
        public void InitCharacter (
            int netId,
            int charId,
            List<DDO_EquipmentInfo> allEquipmentDdoList,
            out E_Repository bag,
            out E_Repository storeHouse,
            out E_EquipmentRegion eqRegion
        ) {
            // 若角色已经初始化
            if (m_bagDict.ContainsKey (netId)) {
                bag = GetBag (netId);
                storeHouse = GetStoreHouse (netId);
                eqRegion = GetEquiped (netId);
                return;
            }
            var bagDdoList = m_dds.GetBagByCharacterId (charId);
            var storeHouseDdoList = m_dds.GetStoreHouseByCharacterId (charId);
            var equipedDdoList = m_dds.GetEquipmentRegionByCharacterId (charId);
            var eqInfoDdoList = m_dds.GetAllEquipmentByCharacterId (charId);

            // 初始化背包, 仓库, 装备区
            bag = s_entityPool.m_repositoryPool.GetInstance ();
            storeHouse = s_entityPool.m_repositoryPool.GetInstance ();
            eqRegion = s_entityPool.m_equipmentRegionPool.GetInstance ();

            bagDdoList.Sort ((a, b) => { return a.m_position - b.m_position; });
            storeHouseDdoList.Sort ((a, b) => { return a.m_position - b.m_position; });
            equipedDdoList.Sort ((a, b) => { return a.m_position - b.m_position; });

            E_Item[] itemInBag = m_itemFactory.GetAndResetInstanceArr (bagDdoList, eqInfoDdoList);
            E_Item[] itemInStoreHouse = m_itemFactory.GetAndResetInstanceArr (storeHouseDdoList, eqInfoDdoList);
            E_Item[] itemEquiped = m_itemFactory.GetAndResetInstanceArr (equipedDdoList, eqInfoDdoList);
            bag.Reset (ItemPlace.BAG, itemInBag);
            storeHouse.Reset (ItemPlace.STORE_HOUSE, itemInStoreHouse);
            eqRegion.Reset (ItemPlace.EQUIPMENT_REGION, itemEquiped);
            // 索引各区域
            m_bagDict[netId] = bag;
            m_storeHouseDict[netId] = storeHouse;
            m_equipmentRegionDict[netId] = eqRegion;
        }
        public void RemoveCharacter (int netId) {
            E_Repository bag;
            m_bagDict.TryGetValue (netId, out bag);
            E_Repository storeHouse;
            m_storeHouseDict.TryGetValue (netId, out storeHouse);
            E_EquipmentRegion equiped;
            m_equipmentRegionDict.TryGetValue (netId, out equiped);
            if (bag == null || storeHouse == null || equiped == null)
                return;
            m_bagDict.Remove (netId);
            m_storeHouseDict.Remove (netId);
            m_equipmentRegionDict.Remove (netId);
            s_entityPool.m_repositoryPool.RecycleInstance (bag);
            s_entityPool.m_repositoryPool.RecycleInstance (storeHouse);
            s_entityPool.m_equipmentRegionPool.RecycleInstance (equiped);
            RecycleItemList (bag.m_ItemList);
            RecycleItemList (storeHouse.m_ItemList);
            RecycleItemList (equiped.GetAllItemList ());
        }
        /// <summary>
        /// 根据itemId与num  
        /// 初始化Item  
        /// 返回Entity  
        /// 不进入索引  
        /// </summary>
        public List<E_Item> InitItemList (IReadOnlyList < (short, short) > itemIdAndNumList) {
            var res = new List<E_Item> ();
            for (int i = 0; i < itemIdAndNumList.Count; i++) {
                var idAndNum = itemIdAndNumList[i];
                DE_Item itemDe = m_dem.GetItemById (idAndNum.Item1);
                E_Item item = m_itemFactory.GetAndInitInstance (itemDe, idAndNum.Item2);
                res.Add (item);
            }
            return res;
        }
        /// <summary>
        /// 获取装备区
        /// </summary>
        public E_EquipmentRegion GetEquiped (int netId) {
            E_EquipmentRegion er = null;
            m_equipmentRegionDict.TryGetValue (netId, out er);
            return er;
        }
        public List<short> GetEquipedItemIdList (int netId) {
            var res = new List<short> ();
            res.Clear ();
            var equips = GetEquiped (netId);
            if (equips == null) return null;
            var en = equips.GetEquipedEn ();
            while (en.MoveNext ())
                res.Add (en.Current.Value.m_ItemId);
            return res;
        }
        public E_Repository GetBag (int netId) {
            E_Repository res = null;
            m_bagDict.TryGetValue (netId, out res);
            return res;
        }
        public E_Repository GetStoreHouse (int netId) {
            E_Repository res = null;
            m_storeHouseDict.TryGetValue (netId, out res);
            return res;
        }
        public void RecycleItem (E_Item item) {
            m_itemFactory.RecycleItem (item);
        }
        public void RecycleItemList (List<E_Item> itemList) {
            for (int i = 0; i < itemList.Count; i++)
                RecycleItem (itemList[i]);
        }
        public void SaveItem (E_Item item) {
            m_dds
        }
        public void DeleteItem (long realId) {

        }
    }
}