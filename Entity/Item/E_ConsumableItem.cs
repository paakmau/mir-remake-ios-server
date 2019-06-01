using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_ConsumableItem : E_Item {
        public override ItemType m_Type { get { return ItemType.CONSUMABLE; } }
        public DE_ConsumableData m_consumableDe;
        public void Reset (DE_Item itemDe, DE_ConsumableData consumDe, DDO_Item itemDdo) {
            base.Reset (itemDe, itemDdo);
            m_consumableDe = consumDe;
        }
        public DDO_Item GetDdo (int charId, RepositoryType repo, int pos) {
            return new DDO_Item (m_realId, m_itemId, charId, m_num, repo, pos);
        }
    }
}