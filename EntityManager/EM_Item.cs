using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    class EM_Item : EntityManagerBase {
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
        public DE_GemData GetGemById (short itemId) {
            return DEM_Item.s_instance.GetGemById (itemId);
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
                        item = s_entityPool.m_consumableItemPool.GetInstance ();
                        DE_ConsumableData cDe = DEM_Item.s_instance.GetConsumableById (itemId);
                        ((E_ConsumableItem) item).Reset (itemDe, cDe, itemDdo);
                        break;
                    case ItemType.EQUIPMENT:
                        item = s_entityPool.m_equipmentItemPool.GetInstance ();
                        DE_EquipmentData eqDe = DEM_Item.s_instance.GetEquipmentById (itemId);
                        DDO_Equipment eqDdo = eqDdoDict[realId];
                        ((E_EquipmentItem) item).Reset (itemDe, eqDe, itemDdo, eqDdo);
                        break;
                    case ItemType.MATERIAL:
                        item = s_entityPool.m_materialItemPool.GetInstance ();
                        ((E_MaterialItem) item).Reset (itemDe, itemDdo);
                        break;
                    case ItemType.GEM:
                        item = s_entityPool.m_gemItemPool.GetInstance ();
                        DE_GemData gDe = DEM_Item.s_instance.GetGemById (itemId);
                        ((E_GemItem) item).Reset (itemDe, gDe, itemDdo);
                        break;

                }
                itemArr[i] = item;
            }
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
            E_Repository bag = s_entityPool.m_repositoryPool.GetInstance ();
            E_Repository storeHouse = s_entityPool.m_repositoryPool.GetInstance ();
            E_EquipmentRegion eqRegion = s_entityPool.m_equipmentRegionPool.GetInstance ();
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
        public void RemoveCharacterItem (int netId) {
            var bag = m_networkIdAndBagDict[netId];
            var storeHouse = m_networkIdAndStoreHouseDict[netId];
            var equiped = m_networkIdAndEquipmentRegionDict[netId];
            m_networkIdAndBagDict.Remove (netId);
            m_networkIdAndStoreHouseDict.Remove (netId);
            m_networkIdAndEquipmentRegionDict.Remove (netId);
            s_entityPool.m_repositoryPool.RecycleInstance (bag);
            s_entityPool.m_repositoryPool.RecycleInstance (storeHouse);
            s_entityPool.m_equipmentRegionPool.RecycleInstance (equiped);
            UnloadItemByItemList (bag.m_ItemList);
            UnloadItemByItemList (storeHouse.m_ItemList);
            UnloadItemByItemList (equiped.GetAllItemList ());
        }
        private void UnloadItem (E_Item item) {
            m_realIdAndItemDict.Remove (item.m_realId);
            s_entityPool.RecycleItem(item);
        }
        private void UnloadItemByItemList (List<E_Item> itemList) {
            for (int i = 0; i < itemList.Count; i++)
                UnloadItem (itemList[i]);
        }
        private void LoadItemArr (E_Item[] itemArr) {
            foreach (var item in itemArr) {
                m_realIdAndItemDict.Add (item.m_realId, item);
            }
        }
    }
}