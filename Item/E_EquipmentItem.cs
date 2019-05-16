using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class E_EquipmentItem : E_Item {
        private Dictionary<ActorUnitConcreteAttributeType, int> m_equipmentAttributeDict;
        public Dictionary<ActorUnitConcreteAttributeType, int> m_EquipmentAttributeDict { get { return m_equipmentAttributeDict; } }
        public E_EquipmentItem (long realId, short itemId, short num) : base (realId, itemId, num) { }
    }
}