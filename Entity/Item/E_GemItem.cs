using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_GemItem : E_Item {
        public DE_Gem m_gemDe;
        public void Reset (DE_Item itemDe, DE_Gem gemDe, DDO_Item ddo) {
            base.Reset (itemDe, ddo);
            m_gemDe = gemDe;
        }
    }
}