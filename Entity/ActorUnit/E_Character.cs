using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_Character : E_Unit {
        private DE_CharacterData m_characterDe;
        public override ActorUnitType m_UnitType { get { return ActorUnitType.PLAYER; } }
        public OccupationType m_occupation;
        public int m_UpgradeExperienceInNeed { get { return m_characterDe.m_upgradeExperienceInNeed; } }
        public int m_characterId;
        public int m_experience;
        public Dictionary<CurrencyType, long> m_currencyDict = new Dictionary<CurrencyType, long> ();
        public Dictionary<ActorUnitMainAttributeType, short> m_mainAttrPointDict = new Dictionary<ActorUnitMainAttributeType, short> ();
        public long m_VirtualCurrency {
            get { return m_currencyDict[CurrencyType.VIRTUAL]; }
            set { m_currencyDict[CurrencyType.VIRTUAL] = value; }
        }
        public long m_ChargeCurrency {
            get { return m_currencyDict[CurrencyType.CHARGE]; }
            set { m_currencyDict[CurrencyType.CHARGE] = value; }
        }
        public short m_Strength {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.STRENGTH]; }
        }
        public short m_Intelligence {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.INTELLIGENCE]; }
        }
        public short m_Agility {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.AGILITY]; }
        }
        public short m_Spirit {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.SPIRIT]; }
        }
        public void Reset (int netId, int charId, DE_Unit auDe, DE_CharacterData charDe, DDO_Character charDdo) {
            base.Reset (auDe);
            m_characterDe = charDe;
            m_networkId = netId;
            m_characterId = charId;
            m_occupation = charDdo.m_occupation;
            m_level = charDdo.m_level;
            m_experience = charDdo.m_experience;
            foreach (var c in charDdo.m_currencyArr)
                m_currencyDict[c.Item1] = c.Item2;
            foreach (var mainP in charDdo.m_distributedMainAttrPointArr)
                m_mainAttrPointDict[mainP.Item1] = mainP.Item2;
        }
        public DDO_Character GetDdo () {
            var currencyArr = new (CurrencyType, long)[2];
            currencyArr[0] = (CurrencyType.VIRTUAL, m_VirtualCurrency);
            currencyArr[1] = (CurrencyType.CHARGE, m_ChargeCurrency);
            var pointArr = new (ActorUnitMainAttributeType, short)[4];
            pointArr[0] = (ActorUnitMainAttributeType.STRENGTH, m_Strength);
            pointArr[1] = (ActorUnitMainAttributeType.INTELLIGENCE, m_Intelligence);
            pointArr[2] = (ActorUnitMainAttributeType.AGILITY, m_Agility);
            pointArr[3] = (ActorUnitMainAttributeType.SPIRIT, m_Spirit);
            return new DDO_Character (m_characterId, m_level, m_occupation, m_experience, currencyArr, pointArr);
        }
        public NO_Character GetNo () {
            return new NO_Character (m_networkId, m_position, m_occupation, m_level);
        }
    }
}