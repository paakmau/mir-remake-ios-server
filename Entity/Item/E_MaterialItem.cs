using UnityEngine;

namespace MirRemakeBackend {
    class E_MaterialItem : E_Item {
        public override ItemType m_Type { get { return ItemType.MATERIAL; } }
        public E_MaterialItem (long realId, short itemId, short num) : base (realId, itemId, num) { }
    }
}