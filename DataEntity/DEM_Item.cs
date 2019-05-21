using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 数据型Entity的容器  
    /// 道具  
    /// </summary>
    class DEM_Item {
        public static DEM_Item s_instance;
        private Dictionary<short, DE_Consumable> m_consumableDict = new Dictionary<short, DE_Consumable> ();
        private Dictionary<short, DE_Equipment> m_equipmentDict = new Dictionary<short, DE_Equipment> ();
        public DEM_Item (IDS_Item itemDs) {
            var itemDoArr = itemDs.GetAllItem ();
            foreach (var itemDo in itemDoArr) {
                switch (itemDo.m_type) {
                    case ItemType.CONSUMABLE:
                        m_consumableDict.Add (itemDo.m_itemId, new DE_Consumable (itemDs.GetConsumableInfoById (itemDo.m_itemId)));
                        break;
                    case ItemType.EQUIPMENT:
                        m_equipmentDict.Add (itemDo.m_itemId, new DE_Equipment (itemDs.GetEquipmentInfoById (itemDo.m_itemId)));
                        break;
                }
            }
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