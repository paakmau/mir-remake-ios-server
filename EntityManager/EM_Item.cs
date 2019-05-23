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
            
        }
        public void RemoveCharacterItem (int netId) {
            var bag = m_networkIdAndBagDict[netId];
            var storeHouse = m_networkIdAndStoreHouseDict[netId];
            var equiped = m_networkIdAndEquipmentRegionDict[netId];
            m_networkIdAndBagDict.Remove (netId);
            m_networkIdAndStoreHouseDict.Remove (netId);
            m_networkIdAndEquipmentRegionDict.Remove (netId);
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
    }
}