using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_VipCard {
        public int m_charId;
        public int m_vipLv;
        public int m_chargeMoney;
        public void Reset (DDO_CharacterVipCard ddo) {
            m_charId = ddo.m_characterId;
            m_vipLv = ddo.m_vipLevel;
            m_chargeMoney = ddo.m_chargeMoney;
        }
        public DDO_CharacterVipCard GetDdo () {
            return new DDO_CharacterVipCard (m_charId, m_vipLv, m_chargeMoney);
        }
    }
}