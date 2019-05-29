using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 道具  
    /// </summary>
    class DEM_Item {
        public static DEM_Item s_instance;
        private Dictionary<short, DE_Item> m_itemDict = new Dictionary<short, DE_Item> ();
        private Dictionary<short, DE_ConsumableData> m_consumableDict = new Dictionary<short, DE_ConsumableData> ();
        private Dictionary<short, DE_EquipmentData> m_equipmentDict = new Dictionary<short, DE_EquipmentData> ();
        private Dictionary<short, DE_GemData> m_gemDict = new Dictionary<short, DE_GemData> ();
        public DEM_Item (IDS_Item itemDs) {
            var itemDoArr = itemDs.GetAllItem ();
            var consumableDoArr = itemDs.GetAllConsumable ();
            var equipmentDoArr = itemDs.GetAllEquipment ();
            var gemDoArr = itemDs.GetAllGem ();
            foreach (var itemDo in itemDoArr)
                m_itemDict.Add (itemDo.m_itemId, new DE_Item (itemDo));
            foreach (var consumDo in consumableDoArr)
                m_consumableDict.Add (consumDo.m_itemId, new DE_ConsumableData(consumDo));
            foreach (var equipDo in equipmentDoArr)
                m_equipmentDict.Add (equipDo.m_itemId, new DE_EquipmentData(equipDo));
            foreach (var gemDo in gemDoArr)
                m_gemDict.Add (gemDo.m_itemId, new DE_GemData(gemDo));
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