using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    class EM_Item : EntityManagerBase {
        public static EM_Item s_instance;
        private DEM_Item m_dem;
        private Dictionary<int, E_Repository> m_networkIdAndBagDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_Repository> m_networkIdAndStoreHouseDict = new Dictionary<int, E_Repository> ();
        private Dictionary<int, E_EquipmentRegion> m_networkIdAndEquipmentRegionDict = new Dictionary<int, E_EquipmentRegion> ();
        public EM_Item (DEM_Item dem) { m_dem = dem; }
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
            m_networkIdAndBagDict[netId] = bag;
            m_networkIdAndStoreHouseDict[netId] = storeHouse;
            m_networkIdAndEquipmentRegionDict[netId] = eqRegion;
        }
        public void RemoveCharacter (int netId) {
            E_Repository bag;
            m_networkIdAndBagDict.TryGetValue (netId, out bag);
            E_Repository storeHouse;
            m_networkIdAndStoreHouseDict.TryGetValue (netId, out storeHouse);
            E_EquipmentRegion equiped;
            m_networkIdAndEquipmentRegionDict.TryGetValue (netId, out equiped);
            if (bag == null || storeHouse == null || equiped == null)
                return;
            m_networkIdAndBagDict.Remove (netId);
            m_networkIdAndStoreHouseDict.Remove (netId);
            m_networkIdAndEquipmentRegionDict.Remove (netId);
            s_entityPool.m_repositoryPool.RecycleInstance (bag);
            s_entityPool.m_repositoryPool.RecycleInstance (storeHouse);
            s_entityPool.m_equipmentRegionPool.RecycleInstance (equiped);
            RecycleItemList (bag.m_ItemList);
            RecycleItemList (storeHouse.m_ItemList);
            RecycleItemList (equiped.GetAllItemList ());
        }
        /// <summary>
        /// 实例化Item列表  
        /// </summary>
        public List<E_Item> InitItemList (IReadOnlyList < (short, short) > itemIdAndNumList, IReadOnlyList<long> realIdList) {
            var res = new List<E_Item> ();
            for (int i = 0; i < itemIdAndNumList.Count; i++) {
                var idAndNum = itemIdAndNumList[i];
                DE_Item itemDe = m_dem.GetItemById (idAndNum.Item1);
                E_Item item = GenerateItemEntity (itemDe, realIdList[i], idAndNum.Item2);
                res.Add (item);
            }
            return res;
        }
        public DE_GemData GetGemById (short itemId) {
            return m_dem.GetGemById (itemId);
        }
        public E_EquipmentRegion GetEquiped (int netId) {
            E_EquipmentRegion er = null;
            m_networkIdAndEquipmentRegionDict.TryGetValue (netId, out er);
            return er;
        }
        public List<short> GetEquipedItemIdList (int netId) {
            var res = new List<short> ();
            res.Clear ();
            var equips = GetEquiped (netId);
            if (equips == null) return null;
            var en = equips.GetEquipedEn ();
            while (en.MoveNext ())
                res.Add (en.Current.Value.m_itemId);
            return res;
        }
        public E_Repository GetBag (int netId) {
            E_Repository res = null;
            m_networkIdAndBagDict.TryGetValue (netId, out res);
            return res;
        }
        public E_Repository GetStoreHouse (int netId) {
            E_Repository res = null;
            m_networkIdAndStoreHouseDict.TryGetValue (netId, out res);
            return res;
        }
        public void RecycleItem (E_Item item) {
            s_entityPool.RecycleItem (item);
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
            for (int i = 0; i < ddoList.Count; i++) {
                DDO_Item itemDdo = ddoList[i];
                short itemId = itemDdo.m_itemId;
                long realId = itemDdo.m_realId;
                DE_Item itemDe = m_dem.GetItemById (itemId);
                E_Item item = E_Item.s_emptyItem;
                switch (itemDe.m_type) {
                    case ItemType.CONSUMABLE:
                        item = s_entityPool.m_consumableItemPool.GetInstance ();
                        DE_ConsumableData cDe = m_dem.GetConsumableById (itemId);
                        ((E_ConsumableItem) item).Reset (itemDe, cDe, itemDdo);
                        break;
                    case ItemType.EQUIPMENT:
                        item = s_entityPool.m_equipmentItemPool.GetInstance ();
                        DE_EquipmentData eqDe = m_dem.GetEquipmentById (itemId);
                        DDO_EquipmentInfo eqDdo = eqDdoDict[realId];
                        ((E_EquipmentItem) item).Reset (itemDe, eqDe, itemDdo, eqDdo);
                        break;
                    case ItemType.MATERIAL:
                        item = s_entityPool.m_materialItemPool.GetInstance ();
                        ((E_MaterialItem) item).Reset (itemDe, itemDdo);
                        break;
                    case ItemType.GEM:
                        item = s_entityPool.m_gemItemPool.GetInstance ();
                        DE_GemData gDe = m_dem.GetGemById (itemId);
                        ((E_GemItem) item).Reset (itemDe, gDe, itemDdo);
                        break;
                    case ItemType.EMPTY:
                        item = E_Item.s_emptyItem;
                        break;
                }
                resItemArr[i] = item;
            }
        }
        /// <summary>
        /// 产生一件物品
        /// </summary>
        private E_Item GenerateItemEntity (DE_Item itemDe, long realId, short num) {
            E_Item res = E_Item.s_emptyItem;
            switch (itemDe.m_type) {
                case ItemType.CONSUMABLE:
                    res = s_entityPool.m_consumableItemPool.GetInstance ();
                    DE_ConsumableData conDe = m_dem.GetConsumableById (itemDe.m_id);
                    ((E_ConsumableItem) res).Reset (itemDe, conDe, realId, num);
                    break;
                case ItemType.MATERIAL:
                    res = s_entityPool.m_materialItemPool.GetInstance ();
                    ((E_MaterialItem) res).Reset (itemDe, realId, num);
                    break;
                case ItemType.GEM:
                    res = s_entityPool.m_gemItemPool.GetInstance ();
                    DE_GemData gemDe = m_dem.GetGemById (itemDe.m_id);
                    ((E_GemItem) res).Reset (itemDe, gemDe, realId, num);
                    break;
                case ItemType.EQUIPMENT:
                    res = s_entityPool.m_equipmentItemPool.GetInstance ();
                    DE_EquipmentData eqDe = m_dem.GetEquipmentById (itemDe.m_id);
                    ((E_EquipmentItem) res).Reset (itemDe, eqDe, realId);
                    break;
            }
            return res;
        }
    }
}