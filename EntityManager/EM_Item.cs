using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    class EM_Item {
        public static EM_Item s_instance;
        private Dictionary<long, E_Item> m_realIdAndItemDict = new Dictionary<long, E_Item> ();
        private Dictionary<int, E_Repository> m_networkIdAndBagDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_Repository> m_networkIdAndStoreHouseDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_EquipmentRegion> m_networkIdAndEquipmentRegionDict = new Dictionary<int, E_EquipmentRegion> ();
        public E_Item GetItemByRealId (long realId) {
            E_Item res = null;
            m_realIdAndItemDict.TryGetValue (realId, out res);
            return res;
        }
        public E_EquipmentRegion GetEquipedByNetworkId (int netId) {
            E_EquipmentRegion er = null;
            m_networkIdAndEquipmentRegionDict.TryGetValue (netId, out er);
            return er;
        }
        public E_Repository GetBagByNetworkId (int netId) {
            E_Repository res = null;
            m_networkIdAndBagDict.TryGetValue (netId, out res);
            return res;
        }
        public E_Repository GetStoreHouseByNetworkId (int netId) {
            E_Repository res = null;
            m_networkIdAndStoreHouseDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 初始化新的角色的所有物品
        /// </summary>
        /// <param name="netId"></param>
        /// <param name="bagDdo"></param>
        /// <param name="storeHouseDdo"></param>
        /// <param name="equipedDdo"></param>
        /// <param name="allEquipmentDdoList"></param>
        public void InitCharacterItem (int netId, List<DDO_Item> bagDdo, List<DDO_Item> storeHouseDdo, List<DDO_Item> equipedDdo, List<DDO_Equipment> allEquipmentDdoList) {
            // 索引角色的所有装备
            Dictionary<long, DDO_Equipment> eqDdoDict = new Dictionary<long, DDO_Equipment> ();
            for (int i = 0; i < allEquipmentDdoList.Count; i++)
                eqDdoDict.Add (allEquipmentDdoList[i].m_realId, allEquipmentDdoList[i]);
            // 初始化背包, 仓库, 装备区
            E_Repository bag = EntityManagerPoolInstance.s_repositoryPool.GetInstance ();
            E_Repository storeHouse = EntityManagerPoolInstance.s_repositoryPool.GetInstance ();
            E_EquipmentRegion eqRegion = EntityManagerPoolInstance.s_equipmentRegionPool.GetInstance ();
            E_Item[] itemInBag = new E_Item[bagDdo.Count];
            E_Item[] itemInStoreHouse = new E_Item[storeHouseDdo.Count];
            E_Item[] itemEquiped = new E_Item[equipedDdo.Count];
            GetItemEntityArrByDdo (bagDdo, eqDdoDict, itemInBag);
            GetItemEntityArrByDdo (storeHouseDdo, eqDdoDict, itemInStoreHouse);
            GetItemEntityArrByDdo (equipedDdo, eqDdoDict, itemEquiped);
            bag.Reset (itemInBag);
            storeHouse.Reset (itemInStoreHouse);
            eqRegion.Reset (itemEquiped);
            // 索引各区域
            m_networkIdAndBagDict[netId] = bag;
            m_networkIdAndStoreHouseDict[netId] = storeHouse;
            m_networkIdAndEquipmentRegionDict[netId] = eqRegion;
            // 索引所有道具
            LoadItemArr (itemInBag);
            LoadItemArr (itemInStoreHouse);
            LoadItemArr (itemEquiped);
        }
        /// <summary>
        /// 根据ddoList写入itemArr  
        /// itemArr的空间需要足够  
        /// </summary>
        private void GetItemEntityArrByDdo (List<DDO_Item> ddoList, Dictionary<long, DDO_Equipment> eqDdoDict, E_Item[] itemArr) {
            for (int i = 0; i < ddoList.Count; i++) {
                DDO_Item itemDdo = ddoList[i];
                short itemId = itemDdo.m_itemId;
                long realId = itemDdo.m_realId;
                DE_Item itemDe = DEM_Item.s_instance.GetItemById (itemId);
                E_Item item = null;
                switch (itemDe.m_type) {
                    case ItemType.CONSUMABLE:
                        item = EntityManagerPoolInstance.s_consumableItemPool.GetInstance ();
                        ((E_ConsumableItem) item).Reset (itemDe, itemDdo);
                        break;
                    case ItemType.EQUIPMENT:
                        item = EntityManagerPoolInstance.s_equipmentItemPool.GetInstance ();
                        DE_Equipment eqDe = DEM_Item.s_instance.GetEquipmentById (itemId);
                        DDO_Equipment eqDdo = eqDdoDict[realId];
                        ((E_EquipmentItem) item).Reset (itemDe, eqDe, itemDdo, eqDdo);
                        break;
                    case ItemType.MATERIAL:
                        item = EntityManagerPoolInstance.s_materialItemPool.GetInstance ();
                        ((E_MaterialItem) item).Reset (itemDe, itemDdo);
                        break;
                }
                itemArr[i] = item;
            }
        }
        public void RemoveCharacterItem (int netId) {
            var bag = m_networkIdAndBagDict[netId];
            var storeHouse = m_networkIdAndStoreHouseDict[netId];
            var equiped = m_networkIdAndEquipmentRegionDict[netId];
            m_networkIdAndBagDict.Remove (netId);
            m_networkIdAndStoreHouseDict.Remove (netId);
            m_networkIdAndEquipmentRegionDict.Remove (netId);
            EntityManagerPoolInstance.s_repositoryPool.RecycleInstance (bag);
            EntityManagerPoolInstance.s_repositoryPool.RecycleInstance (storeHouse);
            EntityManagerPoolInstance.s_equipmentRegionPool.RecycleInstance (equiped);
            UnloadItemByItemList (bag.m_ItemList);
            UnloadItemByItemList (storeHouse.m_ItemList);
            UnloadItemByItemList (equiped.GetAllItemList ());
        }
        public void UnloadItemByRealId (long realId) {
            m_realIdAndItemDict.Remove (realId);
        }
        private void UnloadItemByItemList (List<E_Item> itemList) {
            for (int i = 0; i < itemList.Count; i++)
                m_realIdAndItemDict.Remove (itemList[i].m_realId);
        }
        public void LoadItem (E_Item item) {
            m_realIdAndItemDict.Add (item.m_realId, item);
        }
        public void LoadItemList (List<E_Item> itemList) {
            for (int i = 0; i < itemList.Count; i++)
                m_realIdAndItemDict.Add (itemList[i].m_realId, itemList[i]);
        }
        public void LoadItemArr (E_Item[] itemArr) {
            foreach (var item in itemArr) {
                m_realIdAndItemDict.Add (item.m_realId, item);
            }
        }
    }
}