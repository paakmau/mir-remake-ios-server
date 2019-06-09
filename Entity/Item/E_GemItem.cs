using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_GemItem : E_Item {
        public DE_GemData m_gemDe;
        public void Reset (DE_Item itemDe, DE_GemData gemDe, DDO_Item ddo) {
            base.Reset (itemDe, ddo);
            m_gemDe = gemDe;
        }
        public void Reset (DE_Item itemDe, DE_GemData gemDe, long realId, short num) {
            base.Reset (itemDe, realId, num);
            m_gemDe = gemDe;
        }
    }
}