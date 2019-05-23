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
        private Dictionary<short, DE_Consumable> m_consumableDict = new Dictionary<short, DE_Consumable> ();
        private Dictionary<short, DE_Equipment> m_equipmentDict = new Dictionary<short, DE_Equipment> ();
        public DEM_Item (IDS_Item itemDs) {
            var itemDoArr = itemDs.GetAllItem ();
            var consumableDoArr = itemDs.GetAllConsumable ();
            var equipmentDoArr = itemDs.GetAllEquipment ();
            foreach (var itemDo in itemDoArr)
                m_itemDict.Add (itemDo.m_itemId, new DE_Item (itemDo));
            foreach (var consumDo in consumableDoArr)
                m_consumableDict.Add (consumDo.m_itemId, new DE_Consumable(consumDo));
            foreach (var equipDo in equipmentDoArr)
                m_equipmentDict.Add (equipDo.m_itemId, new DE_Equipment(equipDo));
        }
        public DE_Item GetItemById (short itemId) {
            DE_Item res = null;
            m_itemDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_Consumable GetConsumableById (short itemId) {
            DE_Consumable res = null;
            m_consumableDict.TryGetValue (itemId, out res);
            return res;
        }
        public DE_Equipment GetEquipmentById (short itemId) {
            DE_Equipment res = null;
            m_equipmentDict.TryGetValue (itemId, out res);
            return res;
        }
    }
}