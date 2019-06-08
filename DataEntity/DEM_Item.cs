using System.Collections.Generic;
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
        public DEM_Item (IDS_Item itemDs) {
            var consumableDoArr = itemDs.GetAllConsumable ();
            var equipmentDoArr = itemDs.GetAllEquipment ();
            var gemDoArr = itemDs.GetAllGem ();
            foreach (var itemDo in consumableDoArr) {
                m_itemDict.Add (itemDo.Item1.m_itemId, new DE_Item (itemDo.Item1));
                m_consumableDict.Add (itemDo.Item1.m_itemId, new DE_ConsumableData(itemDo.Item2));
            }
            foreach (var itemDo in equipmentDoArr) {
                m_itemDict.Add (itemDo.Item1.m_itemId, new DE_Item (itemDo.Item1));
                m_equipmentDict.Add (itemDo.Item1.m_itemId, new DE_EquipmentData(itemDo.Item2));
            }
            foreach (var itemDo in gemDoArr) {
                m_itemDict.Add (itemDo.Item1.m_itemId, new DE_Item (itemDo.Item1));
                m_gemDict.Add (itemDo.Item1.m_itemId, new DE_GemData(itemDo.Item2));
            }
        }
        public DE_Item GetItemById (short itemId) {
            DE_Item res = null;
            m_itemDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_ConsumableData GetConsumableById (short itemId) {
            DE_ConsumableData res = null;
            m_consumableDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_EquipmentData GetEquipmentById (short itemId) {
            DE_EquipmentData res = null;
            m_equipmentDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_GemData GetGemById (short itemId) {
            DE_GemData res = null;
            m_gemDict.TryGetValue (itemId, out res);
            return res;
        }
    }
}