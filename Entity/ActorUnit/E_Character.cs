using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_Character : E_Unit {
        private DE_Character m_characterDe;
        private DE_CharacterData m_characterDataDe;
        public override ActorUnitType m_UnitType { get { return ActorUnitType.PLAYER; } }
        public int m_characterId;
        public OccupationType m_Occupation { get { return m_characterDe.m_occupation; } }
        public override short m_Level { get { return m_characterDataDe.m_level; } }
        public short m_MaxLevel { get { return m_characterDe.m_characterMaxLevel; } }
        public short m_TotalMainPoint { get { return m_characterDataDe.m_mainAttributePointNum; } }
        public int m_UpgradeExperienceInNeed { get { return m_characterDataDe.m_upgradeExperienceInNeed; } }
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
            set { m_mainAttrPointDict[ActorUnitMainAttributeType.STRENGTH] = value; }
        }
        public short m_Intelligence {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.INTELLIGENCE]; }
            set { m_mainAttrPointDict[ActorUnitMainAttributeType.INTELLIGENCE] = value; }
        }
        public short m_Agility {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.AGILITY]; }
            set { m_mainAttrPointDict[ActorUnitMainAttributeType.AGILITY] = value; }
        }
        public short m_Spirit {
            get { return m_mainAttrPointDict[ActorUnitMainAttributeType.SPIRIT]; }
            set { m_mainAttrPointDict[ActorUnitMainAttributeType.SPIRIT] = value; }
        }
        public void Reset (int netId, int charId, DE_Character charDe, DE_Unit auDe, DE_CharacterData charDataDe, DDO_Character charDdo) {
            base.Reset (auDe);
            m_characterDe = charDe;
            m_characterDataDe = charDataDe;
            m_networkId = netId;
            m_characterId = charId;
            m_experience = charDdo.m_experience;
            foreach (var c in charDdo.m_currencyArr)
                m_currencyDict[c.Item1] = c.Item2;
            foreach (var mainP in charDdo.m_distributedMainAttrPointArr)
                m_mainAttrPointDict[mainP.Item1] = mainP.Item2;
        }
        /// <summary>
        /// 尝试使用经验升级, 返回提升的等级
        /// </summary>
        public int TryLevelUp () {
            if (m_Level == m_MaxLevel)
                return 0;
            int cnt = 0;
            while (m_experience >= m_UpgradeExperienceInNeed) {
                m_experience -= m_UpgradeExperienceInNeed;
                m_unitDe = m_characterDe.m_unitAllLevel[m_Level + 1];
                m_characterDataDe = m_characterDe.m_characterDataAllLevel[m_Level + 1];
                cnt++;
            }
            return cnt;
        }
        public void DistributePoints (short str, short intl, short agl, short spr) {
            if (str + intl + agl + spr > m_TotalMainPoint)
                return;
            m_Strength = str;
            m_Intelligence = intl;
            m_Agility = agl;
            m_Spirit = spr;
        }
        public DDO_Character GetDdo () {
            var currencyArr = new (CurrencyType, long) [2];
            currencyArr[0] = (CurrencyType.VIRTUAL, m_VirtualCurrency);
            currencyArr[1] = (CurrencyType.CHARGE, m_ChargeCurrency);
            var pointArr = new (ActorUnitMainAttributeType, short) [4];
            pointArr[0] = (ActorUnitMainAttributeType.STRENGTH, m_Strength);
            pointArr[1] = (ActorUnitMainAttributeType.INTELLIGENCE, m_Intelligence);
            pointArr[2] = (ActorUnitMainAttributeType.AGILITY, m_Agility);
            pointArr[3] = (ActorUnitMainAttributeType.SPIRIT, m_Spirit);
            return new DDO_Character (m_characterId, m_Level, m_Occupation, m_experience, currencyArr, pointArr);
        }
        public NO_Character GetNo () {
            return new NO_Character (m_networkId, m_position, m_Occupation, m_Level);
        }
    }
}