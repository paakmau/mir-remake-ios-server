using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Character : E_ActorUnit {
        private DE_CharacterData m_characterDe;
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.PLAYER; } }
        public OccupationType m_occupation;
        public int m_UpgradeExperienceInNeed { get { return m_characterDe.m_upgradeExperienceInNeed; } }
        public int m_characterId;
        public int m_experience;
        public ValueTuple<CurrencyType, long>[] m_currencyArr;
        public ValueTuple<ActorUnitMainAttributeType, short>[] m_mainAttrPointArr;
        public long m_VirtualCurrency {
            get {
                foreach (var item in m_currencyArr)
                    if (item.Item1 == CurrencyType.VIRTUAL)
                        return item.Item2;
                return 0;
            }
            set {
                for (int i=0; i<m_currencyArr.Length; i++) {
                    if (m_currencyArr[i].Item1 == CurrencyType.VIRTUAL)
                        m_currencyArr[i].Item2 = value;
                }
            }
        }
        public void Reset (int netId, int charId, DE_ActorUnit auDe, DE_CharacterData charDe, DDO_Character charDdo) {
            base.Reset (auDe);
            m_characterDe = charDe;
            m_networkId = netId;
            m_characterId = charId;
            m_occupation = charDdo.m_occupation;
            m_level = charDdo.m_level;
            m_experience = charDdo.m_experience;
            m_currencyArr = charDdo.m_currencyArr;
            m_mainAttrPointArr = charDdo.m_distributedMainAttrPointArr;
        }
        public DDO_Character GetDdo () {
            return new DDO_Character (m_characterId, m_level, m_occupation, m_experience, m_currencyArr, m_mainAttrPointArr);
        }
    }
}