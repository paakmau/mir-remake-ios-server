using UnityEngine;

namespace MirRemakeBackend {
    class E_ConsumableItem : E_Item {
        public override ItemType m_Type { get { return ItemType.CONSUMABLE; } }
        private E_Effect m_itemEffect;
        public E_ConsumableItem (long realId, short itemId, short num) : base (realId, itemId, num) { }
    }
}