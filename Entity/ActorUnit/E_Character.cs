using System;
using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.Entity {
    class E_Character : E_ActorUnit {
        private DE_CharacterData m_characterDe;
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.PLAYER; } }
        public int m_UpgradeExperienceInNeed { get { return m_characterDe.m_upgradeExperienceInNeed; } }
        public int m_characterId;
        public int m_experience;
        public ValueTuple<CurrencyType, long>[] m_currencyArr;
        public void Reset (int netId, int charId, DE_ActorUnit auDe, DE_CharacterData charDe, DDO_Character charDdo) {
            base.Reset (auDe);
            m_characterDe = charDe;
            m_networkId = netId;
            m_characterId = charId;
            m_level = charDdo.m_level;
            m_experience = charDdo.m_experience;
            m_currencyArr = charDdo.m_currencyArr;
        }
    }
}