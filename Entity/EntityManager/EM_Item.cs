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
            private abstract class ItemInitializerBase {
                public abstract ItemType m_ItemType { get; }
                protected DEM_Item m_dem;
                public ItemInitializerBase (DEM_Item dem) {
                    m_dem = dem;
                }
                public virtual void Initialize (E_Item resItem, DE_Item de, long realId, short num) {
                    resItem.Reset (de, realId, num);
                }
            }
            private class II_Empty : ItemInitializerBase {
                public override ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public II_Empty (DEM_Item dem) : base (dem) { }
            }
            private class II_Material : ItemInitializerBase {
                public override ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public II_Material (DEM_Item dem) : base (dem) { }
            }
            private class II_Consumable : ItemInitializerBase {
                public override ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public II_Consumable (DEM_Item dem) : base (dem) { }
                public override void Initialize (E_Item resItem, DE_Item de, long realId, short num) {
                    var conDe = m_dem.GetConsumableById (de.m_id);
                    ((E_ConsumableItem) resItem).Reset (de, conDe, realId, num);
                }
            }
            private class II_Equipment : ItemInitializerBase {
                public override ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public II_Equipment (DEM_Item dem) : base (dem) { }
                public override void Initialize (E_Item resItem, DE_Item de, long realId, short num) {
                    var eqDe = m_dem.GetEquipmentById (de.m_id);
                    ((E_EquipmentItem) resItem).Reset (de, eqDe, realId);
                }
            }
            private class II_Gem : ItemInitializerBase {
                public override ItemType m_ItemType { get { return ItemType.GEM; } }
                public II_Gem (DEM_Item dem) : base (dem) { }
                public override void Initialize (E_Item resItem, DE_Item de, long realId, short num) {
                    var gemDe = m_dem.GetGemById (de.m_id);
                    ((E_GemItem) resItem).Reset (de, gemDe, realId);
                }
            }
            private Dictionary<ItemType, ObjectPool> m_itemPoolDict = new Dictionary<ItemType, ObjectPool> ();
            private Dictionary<ItemType, ItemInitializerBase> m_itemInitializer = new Dictionary<ItemType, ItemInitializerBase> ();
            private const int c_emptyItemPoolSize = 100000;
            private const int c_materialItemPoolSize = 100000;
            private const int c_consumableItemPoolSize = 100000;
            private const int c_equipmentItemPoolSize = 100000;
            private const int c_gemItemPoolSize = 100000;
            public ItemFactory (DEM_Item dem) {
                m_itemPoolDict.Add (ItemType.EMPTY, new ObjectPool<E_EmptyItem> (c_emptyItemPoolSize));
                m_itemPoolDict.Add (ItemType.MATERIAL, new ObjectPool<E_MaterialItem> (c_materialItemPoolSize));
                m_itemPoolDict.Add (ItemType.CONSUMABLE, new ObjectPool<E_ConsumableItem> (c_consumableItemPoolSize));
                m_itemPoolDict.Add (ItemType.EQUIPMENT, new ObjectPool<E_EquipmentItem> (c_equipmentItemPoolSize));
                m_itemPoolDict.Add (ItemType.GEM, new ObjectPool<E_GemItem> (c_gemItemPoolSize));
                // 实例化所有 ItemInitializerBase 的子类
                var ccType = typeof (ItemInitializerBase);
                var ccImplTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && ccType.IsAssignableFrom (p));
                foreach (var type in ccImplTypes) {
                    ItemInitializerBase ii = type.GetConstructor (new Type[] { typeof (DEM_Item) }).Invoke (new Object[] { dem }) as ItemInitializerBase;
                    m_itemInitializer.Add (ii.m_ItemType, ii);
                }
            }
            public void RecycleItem (E_Item item) {
                if (item.m_Type == ItemType.EMPTY) return;
                m_itemPoolDict[item.m_Type].RecycleInstance (item);
            }
            public E_Item GetInstance (ItemType type) {
                return m_itemPoolDict[type].GetInstanceObj () as E_Item;
            }
            public E_Item GetAndResetInitInstance (DE_Item de, long realId, short num) {
                var initializer = m_itemInitializer[de.m_type];
                var res = GetInstance (de.m_type);
                initializer.Initialize (res, de, realId, num);
                return res;
            }
        }
        public static EM_Item s_instance;
        private DEM_Item m_dem;
        private ItemFactory m_itemFactory;
        private Dictionary<int, E_Repository> m_bagDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_Repository> m_storeHouseDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_EquipmentRegion> m_equipmentRegionDict = new Dictionary<int, E_EquipmentRegion> ();
        public EM_Item (DEM_Item dem) {
            m_dem = dem;
            m_itemFactory = new ItemFactory (m_dem);
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
            List<DDO_Item> bagDdo,
            List<DDO_Item> storeHouseDdo,
            List<DDO_Item> equipedDdo,
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

            // 索引角色的所有装备
            Dictionary<long, DDO_EquipmentInfo> eqDdoDict = new Dictionary<long, DDO_EquipmentInfo> ();
            for (int i = 0; i < allEquipmentDdoList.Count; i++)
                eqDdoDict.Add (allEquipmentDdoList[i].m_realId, allEquipmentDdoList[i]);
            // 初始化背包, 仓库, 装备区
            bag = s_entityPool.m_repositoryPool.GetInstance ();
            storeHouse = s_entityPool.m_repositoryPool.GetInstance ();
            eqRegion = s_entityPool.m_equipmentRegionPool.GetInstance ();
            E_Item[] itemInBag = new E_Item[bagDdo.Count];
            E_Item[] itemInStoreHouse = new E_Item[storeHouseDdo.Count];
            E_Item[] itemEquiped = new E_Item[equipedDdo.Count];
            GetItemEntityArrByDdo (bagDdo, eqDdoDict, itemInBag);
            GetItemEntityArrByDdo (storeHouseDdo, eqDdoDict, itemInStoreHouse);
            GetItemEntityArrByDdo (equipedDdo, eqDdoDict, itemEquiped);
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
                E_Item item = m_itemFactory.GetAndResetInitInstance (itemDe, -1, idAndNum.Item2);
                res.Add (item);
            }
            return res;
        }
        public E_EmptyItem GetEmptyInstance (long realId) {
            return (E_EmptyItem) m_itemFactory.GetAndResetInitInstance (m_dem.GetEmptyItem (), realId, 0);
        }
        public DE_GemData GetGemById (short itemId) {
            return m_dem.GetGemById (itemId);
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
        /// <summary>
        /// 根据ddoList写入itemArr  
        /// itemArr的空间需要足够  
        /// </summary>
        private void GetItemEntityArrByDdo (List<DDO_Item> ddoList, Dictionary<long, DDO_EquipmentInfo> eqDdoDict, E_Item[] resItemArr) {
            ddoList.Sort ((a, b) => { return a.m_position - b.m_position; });
            for (int i = 0; i < ddoList.Count; i++) {
                DDO_Item itemDdo = ddoList[i];
                short itemId = itemDdo.m_itemId;
                long realId = itemDdo.m_realId;
                DE_Item itemDe = m_dem.GetItemById (itemId);
                E_Item item = m_itemFactory.GetInstance (itemDe.m_type);
                switch (itemDe.m_type) {
                    case ItemType.CONSUMABLE:
                        DE_ConsumableData cDe = m_dem.GetConsumableById (itemId);
                        ((E_ConsumableItem) item).Reset (itemDe, cDe, itemDdo);
                        break;
                    case ItemType.EQUIPMENT:
                        DE_EquipmentData eqDe = m_dem.GetEquipmentById (itemId);
                        DDO_EquipmentInfo eqDdo = eqDdoDict[realId];
                        ((E_EquipmentItem) item).Reset (itemDe, eqDe, itemDdo, eqDdo);
                        break;
                    case ItemType.MATERIAL:
                        ((E_MaterialItem) item).Reset (itemDe, itemDdo);
                        break;
                    case ItemType.GEM:
                        DE_GemData gDe = m_dem.GetGemById (itemId);
                        ((E_GemItem) item).Reset (itemDe, gDe, itemDdo);
                        break;
                    case ItemType.EMPTY:
                        ((E_EmptyItem) item).Reset (itemDe, itemDdo);
                        break;
                }
                resItemArr[i] = item;
            }
        }
    }
}