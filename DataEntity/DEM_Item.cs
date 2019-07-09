using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 道具  
    /// </summary>
    class DEM_Item {
        private Dictionary<short, DE_Item> m_itemDict = new Dictionary<short, DE_Item> ();
        private Dictionary<short, DE_ConsumableData> m_consumableDict = new Dictionary<short, DE_ConsumableData> ();
        private Dictionary<short, DE_EquipmentData> m_equipmentDict = new Dictionary<short, DE_EquipmentData> ();
        private Dictionary<short, DE_GemData> m_gemDict = new Dictionary<short, DE_GemData> ();
        private List < (short, Vector2) > m_renewableItemList = new List < (short, Vector2) > ();
        public DEM_Item (IDS_Item itemDs, IDS_GroundItemMap gndItemDs) {
            // 获取所有的Item
            var consumableDoArr = itemDs.GetAllConsumable ();
            var equipmentDoArr = itemDs.GetAllEquipment ();
            var gemDoArr = itemDs.GetAllGem ();
            var materialDoArr = itemDs.GetAllMaterial ();
            foreach (var itemDo in consumableDoArr) {
                m_itemDict.Add (itemDo.Item1.m_itemId, new DE_Item (itemDo.Item1));
                m_consumableDict.Add (itemDo.Item1.m_itemId, new DE_ConsumableData (itemDo.Item2));
            }
            foreach (var itemDo in equipmentDoArr) {
                m_itemDict.Add (itemDo.Item1.m_itemId, new DE_Item (itemDo.Item1));
                m_equipmentDict.Add (itemDo.Item1.m_itemId, new DE_EquipmentData (itemDo.Item2));
            }
            foreach (var itemDo in gemDoArr) {
                m_itemDict.Add (itemDo.Item1.m_itemId, new DE_Item (itemDo.Item1));
                m_gemDict.Add (itemDo.Item1.m_itemId, new DE_GemData (itemDo.Item2));
            }
            m_itemDict.Add (-1, new DE_Item ());

            // 获取可再生Item
            m_renewableItemList = new List < (short, Vector2) > (gndItemDs.GetAllGroundItemRespawnPosition ());
        }
        public DE_Item GetItemById (short itemId) {
            DE_Item res;
            m_itemDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_ConsumableData GetConsumableById (short itemId) {
            DE_ConsumableData res;
            m_consumableDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_EquipmentData GetEquipmentById (short itemId) {
            DE_EquipmentData res;
            m_equipmentDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_GemData GetGemById (short itemId) {
            DE_GemData res;
            m_gemDict.TryGetValue (itemId, out res);
            return res;
        }
        public IReadOnlyList < (short, Vector2) > GetAllRenewableItemList () {
            return m_renewableItemList;
        }
    }
}