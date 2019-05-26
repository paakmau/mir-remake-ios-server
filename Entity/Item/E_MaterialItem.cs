using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_MaterialItem : E_Item {
        public override ItemType m_Type { get { return ItemType.MATERIAL; } }
        public new void Reset (DE_Item de, DDO_Item ddo) {
            base.Reset (de, ddo);
        }
    }
}