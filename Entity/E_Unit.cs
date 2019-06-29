using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_Unit {
        protected DE_Unit m_unitDe;
        public abstract ActorUnitType m_UnitType { get; }
        public int m_networkId;
        public abstract short m_Level { get; }
        public Vector2 m_position;
        public int m_curHp;
        public int m_curMp;
        private Dictionary<ActorUnitConcreteAttributeType, int> m_concreteAttributeDict = new Dictionary<ActorUnitConcreteAttributeType, int> ();
        private Dictionary<ActorUnitSpecialAttributeType, int> m_specialAttributeDict = new Dictionary<ActorUnitSpecialAttributeType, int> ();
        // 仇恨度哈希表
        public Dictionary<int, MyTimer.Time> m_hatredRefreshDict = new Dictionary<int, MyTimer.Time> ();
        public int m_HighestHatredTargetNetId {
            get {
                int res = -1;
                MyTimer.Time resHighest = MyTimer.s_CurTime;
                var en = m_hatredRefreshDict.GetEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.Value >= resHighest) {
                        res = en.Current.Key;
                        resHighest = en.Current.Value;
                    }
                }
                return res;
            }
        }
        public int m_MaxHp {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_HP] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_HP]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_HP] = value; }
        }
        public int m_MaxMp {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_MP] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_MP]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_MP] = value; }
        }
        public int m_DeltaHpPerSecond {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND] = value; }
        }
        public int m_DeltaMpPerSecond {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND] = value; }
        }
        public int m_Attack {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.ATTACK] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.ATTACK]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.ATTACK] = value; }
        }
        public int m_Magic {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAGIC] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAGIC]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAGIC] = value; }
        }
        public int m_Defence {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DEFENCE] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.DEFENCE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DEFENCE] = value; }
        }
        public int m_Resistance {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.RESISTANCE] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.RESISTANCE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.RESISTANCE] = value; }
        }
        public int m_Tenacity {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.TENACITY] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.TENACITY]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.TENACITY] = value; }
        }
        public int m_Speed {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.SPEED] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.SPEED]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.SPEED] = value; }
        }
        public int m_CriticalRate {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_RATE] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_RATE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_RATE] = value; }
        }
        public int m_CriticalBonus {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS] = value; }
        }
        public int m_HitRate {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.HIT_RATE] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.HIT_RATE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.HIT_RATE] = value; }
        }
        public bool m_IsFaint { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] > 0; } }
        public bool m_IsSilent { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] > 0; } }
        public bool m_IsImmobile { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] > 0; } }
        public bool m_IsDead { get { return m_curHp <= 0; } }
        public virtual void Reset (DE_Unit de) {
            m_unitDe = de;
            // TODO: 具体属性使用 类似 *1000 + 1, 2, 3 的方式设计
            var attrEn = de.m_concreteAttributeDict.Keys.GetEnumerator ();
            while (attrEn.MoveNext ())
                m_concreteAttributeDict[attrEn.Current] = 0;
            Respawn ();
        }
        public void Respawn () {
            m_curHp = m_MaxHp;
            m_curMp = m_MaxMp;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] = 0;
        }
        public void AddConAttr (ActorUnitConcreteAttributeType type, int value) {
            m_concreteAttributeDict[type] += value;
        }
        public void AddSpAttr (ActorUnitSpecialAttributeType type, int value) {
            m_specialAttributeDict[type] += value;
        }
    }

    class E_Monster : E_Unit {
        public override ActorUnitType m_UnitType { get { return ActorUnitType.MONSTER; } }
        public DE_MonsterData m_monsterDe;
        public override short m_Level { get { return m_monsterDe.m_level; } }
        public short m_MonsterId { get { return m_monsterDe.m_monsterId; } }
        public Vector2 m_respawnPosition;
        public void Reset (int networkId, Vector2 pos, DE_Unit auDe, DE_MonsterData mDe) {
            base.Reset (auDe);
            m_monsterDe = mDe;
            m_networkId = networkId;
            m_position = pos;
            m_respawnPosition = pos;
        }
        public NO_Monster GetNo () {
            return new NO_Monster (m_networkId, m_position, m_MonsterId);
        }
    }

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