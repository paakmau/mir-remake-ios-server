using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_ConsumableItem : E_Item {
        public override ItemType m_Type { get { return ItemType.CONSUMABLE; } }
        public DE_Consumable m_consumableDe;
        public void Reset (DE_Item itemDe, DE_Consumable consumDe, DDO_Item itemDdo) {
            base.Reset (itemDe, itemDdo);
            m_consumableDe = consumDe;
        }
    }
}