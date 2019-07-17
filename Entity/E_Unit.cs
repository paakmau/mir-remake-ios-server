using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_Unit {
        protected class ConcreteAttribute {
            private Dictionary<ActorUnitConcreteAttributeType, int> m_attrDict = new Dictionary<ActorUnitConcreteAttributeType, int> ();
            public ConcreteAttribute () {
                Reset ();
            }
            public void Reset () {
                foreach (ActorUnitConcreteAttributeType attr in Enum.GetValues (typeof (ActorUnitConcreteAttributeType)))
                    m_attrDict[attr] = 0;
            }
            public int GetAttr (ActorUnitConcreteAttributeType type) {
                return m_attrDict[type];
            }
            public void SetAttr (ActorUnitConcreteAttributeType type, int value) {
                m_attrDict[type] = value;
            }
            public void AddAttr (ActorUnitConcreteAttributeType type, int value) {
                m_attrDict[type] += value;
            }
            public int m_MaxHp {
                get { return m_attrDict[ActorUnitConcreteAttributeType.MAX_HP]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.MAX_HP] = value; }
            }
            public int m_MaxMp {
                get { return m_attrDict[ActorUnitConcreteAttributeType.MAX_MP]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.MAX_MP] = value; }
            }
            public int m_DeltaHpPerSecond {
                get { return m_attrDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND] = value; }
            }
            public int m_DeltaMpPerSecond {
                get { return m_attrDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND] = value; }
            }
            public int m_Attack {
                get { return m_attrDict[ActorUnitConcreteAttributeType.ATTACK]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.ATTACK] = value; }
            }
            public int m_Magic {
                get { return m_attrDict[ActorUnitConcreteAttributeType.MAGIC]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.MAGIC] = value; }
            }
            public int m_Defence {
                get { return m_attrDict[ActorUnitConcreteAttributeType.DEFENCE]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.DEFENCE] = value; }
            }
            public int m_Resistance {
                get { return m_attrDict[ActorUnitConcreteAttributeType.RESISTANCE]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.RESISTANCE] = value; }
            }
            public int m_Tenacity {
                get { return m_attrDict[ActorUnitConcreteAttributeType.TENACITY]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.TENACITY] = value; }
            }
            public int m_Speed {
                get { return m_attrDict[ActorUnitConcreteAttributeType.SPEED]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.SPEED] = value; }
            }
            public int m_CriticalRate {
                get { return m_attrDict[ActorUnitConcreteAttributeType.CRITICAL_RATE]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.CRITICAL_RATE] = value; }
            }
            public int m_CriticalBonus {
                get { return m_attrDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS] = value; }
            }
            public int m_HitRate {
                get { return m_attrDict[ActorUnitConcreteAttributeType.HIT_RATE]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.HIT_RATE] = value; }
            }
            public int m_DodgeRate {
                get { return m_attrDict[ActorUnitConcreteAttributeType.DODGE_RATE]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.DODGE_RATE] = value; }
            }
            public int m_LifeSteal {
                get { return m_attrDict[ActorUnitConcreteAttributeType.LIFE_STEAL]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.LIFE_STEAL] = value; }
            }
            public int m_PhysicsVulernability {
                get { return m_attrDict[ActorUnitConcreteAttributeType.PHYSICS_VULERNABILITY]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.PHYSICS_VULERNABILITY] = value; }
            }
            public int m_MagicVulernability {
                get { return m_attrDict[ActorUnitConcreteAttributeType.MAGIC_VULERNABILITY]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.MAGIC_VULERNABILITY] = value; }
            }
            public int m_DamageReduction {
                get { return m_attrDict[ActorUnitConcreteAttributeType.DAMAGE_REDUCTION]; }
                set { m_attrDict[ActorUnitConcreteAttributeType.DAMAGE_REDUCTION] = value; }
            }
        }
        protected DE_Unit m_unitDe;
        public abstract ActorUnitType m_UnitType { get; }
        public int m_networkId;
        public abstract short m_Level { get; }
        public Vector2 m_position;
        private Dictionary<ActorUnitSpecialAttributeType, int> m_specialAttributeDict = new Dictionary<ActorUnitSpecialAttributeType, int> ();
        public bool m_IsFaint { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] > 0; } }
        public bool m_IsSilent { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] > 0; } }
        public bool m_IsImmobile { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] > 0; } }
        private ConcreteAttribute m_battleConcreteAttr = new ConcreteAttribute ();
        public int m_curHp;
        public bool m_IsDead { get { return m_curHp <= 0; } }
        public int m_curMp;
        public virtual int m_MaxHp { get { return m_battleConcreteAttr.m_MaxHp + m_unitDe.m_MaxHp; } }
        public virtual int m_MaxMp { get { return m_battleConcreteAttr.m_MaxMp + m_unitDe.m_MaxMp; } }
        public virtual int m_DeltaHpPerSecond { get { return m_battleConcreteAttr.m_DeltaHpPerSecond + m_unitDe.m_DeltaHpPerSecond; } }
        public virtual int m_DeltaMpPerSecond { get { return m_battleConcreteAttr.m_DeltaMpPerSecond + m_unitDe.m_DeltaMpPerSecond; } }
        public virtual int m_Attack { get { return m_battleConcreteAttr.m_Attack + m_unitDe.m_Attack; } }
        public virtual int m_Magic { get { return m_battleConcreteAttr.m_Magic + m_unitDe.m_Magic; } }
        public virtual int m_Defence { get { return m_battleConcreteAttr.m_Defence + m_unitDe.m_Defence; } }
        public virtual int m_Resistance { get { return m_battleConcreteAttr.m_Resistance + m_unitDe.m_Resistance; } }
        public virtual int m_Tenacity { get { return m_battleConcreteAttr.m_Tenacity + m_unitDe.m_Tenacity; } }
        public virtual int m_Speed { get { return m_battleConcreteAttr.m_Speed + m_unitDe.m_Speed; } }
        public virtual int m_CriticalRate { get { return m_battleConcreteAttr.m_CriticalRate + m_unitDe.m_CriticalRate; } }
        public virtual int m_CriticalBonus { get { return m_battleConcreteAttr.m_CriticalBonus + m_unitDe.m_CriticalBonus; } }
        public virtual int m_HitRate { get { return m_battleConcreteAttr.m_HitRate + m_unitDe.m_HitRate; } }
        public virtual int m_DodgeRate { get { return m_battleConcreteAttr.m_DodgeRate + m_unitDe.m_DodgeRate; } }
        public virtual int m_LifeSteal { get { return m_battleConcreteAttr.m_LifeSteal + m_unitDe.m_LifeSteal; } }
        public virtual int m_PhysicsVulernability { get { return m_battleConcreteAttr.m_PhysicsVulernability + m_unitDe.m_PhysicsVulernability; } }
        public virtual int m_MagicVulernability { get { return m_battleConcreteAttr.m_MagicVulernability + m_unitDe.m_MagicVulernability; } }
        public virtual int m_DamageReduction { get { return m_battleConcreteAttr.m_DamageReduction + m_unitDe.m_DamageReduction; } }
        /// <summary>退出被攻击状态计时器</summary>
        public MyTimer.Time m_isAttackedTimer;
        /// <summary>伤害值统计</summary>
        public Dictionary<int, int> m_netIdAndDamageDict = new Dictionary<int, int> ();
        /// <summary>根据伤害值计算仇恨</summary>
        public int m_HighestHatredTargetNetId {
            get {
                int res = -1;
                int maxDamage = 0;
                var en = m_netIdAndDamageDict.GetEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.Value >= maxDamage) {
                        res = en.Current.Key;
                        maxDamage = en.Current.Value;
                    }
                }
                return res;
            }
        }
        public virtual void Reset (DE_Unit de) {
            m_unitDe = de;
            Respawn ();
        }
        public void Dead () {
            m_curHp = m_curMp = 0;
            m_battleConcreteAttr.Reset ();
            m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] = 0;
        }
        public void Respawn () {
            m_curHp = m_MaxHp;
            m_curMp = m_MaxMp;
            m_battleConcreteAttr.Reset ();
            m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] = 0;
        }
        public void AddBattleConAttr (ActorUnitConcreteAttributeType type, int value) {
            m_battleConcreteAttr.AddAttr (type, value);
        }
        public void AddSpAttr (ActorUnitSpecialAttributeType type, int value) {
            m_specialAttributeDict[type] += value;
        }
    }
    class E_Character : E_Unit {
        private DE_Character m_characterDe;
        private DE_CharacterData m_characterDataDe;
        public override ActorUnitType m_UnitType { get { return ActorUnitType.PLAYER; } }
        public int m_playerId;
        public int m_characterId;
        public string m_name;
        public OccupationType m_Occupation { get { return m_characterDe.m_occupation; } }
        public override short m_Level { get { return m_characterDataDe.m_level; } }
        public short m_MaxLevel { get { return m_characterDe.m_characterMaxLevel; } }
        public short m_TotalMainPoint { get { return m_characterDataDe.m_mainAttributePointNum; } }
        public int m_UpgradeExperienceInNeed { get { return m_characterDataDe.m_upgradeExperienceInNeed; } }
        public int m_experience;
        public Dictionary<CurrencyType, long> m_currencyDict = new Dictionary<CurrencyType, long> ();
        public long m_VirtualCurrency {
            get { return m_currencyDict[CurrencyType.VIRTUAL]; }
            set { m_currencyDict[CurrencyType.VIRTUAL] = value; }
        }
        public long m_ChargeCurrency {
            get { return m_currencyDict[CurrencyType.CHARGE]; }
            set { m_currencyDict[CurrencyType.CHARGE] = value; }
        }
        public Dictionary<ActorUnitMainAttributeType, short> m_mainAttrPointDict = new Dictionary<ActorUnitMainAttributeType, short> ();
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
        private ConcreteAttribute m_equipConcreteAttr = new ConcreteAttribute ();
        private ConcreteAttribute m_mainPointConcreteAttr = new ConcreteAttribute ();
        public override int m_MaxHp { get { return m_equipConcreteAttr.m_MaxHp + m_mainPointConcreteAttr.m_MaxHp + base.m_MaxHp; } }
        public override int m_MaxMp { get { return m_equipConcreteAttr.m_MaxMp + m_mainPointConcreteAttr.m_MaxMp + base.m_MaxMp; } }
        public override int m_DeltaHpPerSecond { get { return m_equipConcreteAttr.m_DeltaHpPerSecond + m_mainPointConcreteAttr.m_DeltaHpPerSecond + base.m_DeltaHpPerSecond; } }
        public override int m_DeltaMpPerSecond { get { return m_equipConcreteAttr.m_DeltaMpPerSecond + m_mainPointConcreteAttr.m_DeltaMpPerSecond + base.m_DeltaMpPerSecond; } }
        public override int m_Attack { get { return m_equipConcreteAttr.m_Attack + m_mainPointConcreteAttr.m_Attack + base.m_Attack; } }
        public override int m_Magic { get { return m_equipConcreteAttr.m_Magic + m_mainPointConcreteAttr.m_Magic + base.m_Magic; } }
        public override int m_Defence { get { return m_equipConcreteAttr.m_Defence + m_mainPointConcreteAttr.m_Defence + base.m_Defence; } }
        public override int m_Resistance { get { return m_equipConcreteAttr.m_Resistance + m_mainPointConcreteAttr.m_Resistance + base.m_Resistance; } }
        public override int m_Tenacity { get { return m_equipConcreteAttr.m_Tenacity + m_mainPointConcreteAttr.m_Tenacity + base.m_Tenacity; } }
        public override int m_Speed { get { return m_equipConcreteAttr.m_Speed + m_mainPointConcreteAttr.m_Speed + base.m_Speed; } }
        public override int m_CriticalRate { get { return m_equipConcreteAttr.m_CriticalRate + m_mainPointConcreteAttr.m_CriticalRate + base.m_CriticalRate; } }
        public override int m_CriticalBonus { get { return m_equipConcreteAttr.m_CriticalBonus + m_mainPointConcreteAttr.m_CriticalBonus + base.m_CriticalBonus; } }
        public override int m_HitRate { get { return m_equipConcreteAttr.m_HitRate + m_mainPointConcreteAttr.m_HitRate + base.m_HitRate; } }
        public override int m_DodgeRate { get { return m_equipConcreteAttr.m_DodgeRate + m_mainPointConcreteAttr.m_DodgeRate + base.m_DodgeRate; } }
        public override int m_LifeSteal { get { return m_equipConcreteAttr.m_LifeSteal + m_mainPointConcreteAttr.m_LifeSteal + base.m_LifeSteal; } }
        public override int m_PhysicsVulernability { get { return m_equipConcreteAttr.m_PhysicsVulernability + m_mainPointConcreteAttr.m_PhysicsVulernability + base.m_PhysicsVulernability; } }
        public override int m_MagicVulernability { get { return m_equipConcreteAttr.m_MagicVulernability + m_mainPointConcreteAttr.m_MagicVulernability + base.m_MagicVulernability; } }
        public override int m_DamageReduction { get { return m_equipConcreteAttr.m_DamageReduction + m_mainPointConcreteAttr.m_DamageReduction + base.m_DamageReduction; } }
        public void Reset (int netId, DE_Character charDe, DE_Unit auDe, DE_CharacterData charDataDe, DDO_Character charDdo) {
            base.Reset (auDe);
            m_characterDe = charDe;
            m_characterDataDe = charDataDe;
            m_networkId = netId;
            m_playerId = charDdo.m_playerId;
            m_characterId = charDdo.m_characterId;
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
        public void AddEquipConAttr (ActorUnitConcreteAttributeType type, int value) {
            m_equipConcreteAttr.AddAttr (type, value);
        }
        public void SetMainPointConAttr (ActorUnitConcreteAttributeType type, int value) {
            m_mainPointConcreteAttr.SetAttr (type, value);
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
            return new DDO_Character (m_playerId, m_characterId, m_Level, m_Occupation, m_experience, currencyArr, pointArr, m_name);
        }
        public DDO_CharacterPosition GetPosDdo () {
            return new DDO_CharacterPosition (m_characterId, m_position);
        }
        public NO_Character GetNo () {
            return new NO_Character (m_networkId, m_position, m_Occupation, m_Level);
        }
        // public NO_DamageRankCharacter GetDmgRnkNo () {
        //     return new NO_DamageRankCharacter (m_characterId, )
        // }
    }
    class E_Monster : E_Unit {
        public override ActorUnitType m_UnitType { get { return ActorUnitType.MONSTER; } }
        protected DE_MonsterData m_monsterDe;
        public short m_MonsterId { get { return m_monsterDe.m_monsterId; } }
        public MonsterType m_MonsterType { get { return m_monsterDe.m_monsterType; } }
        public override short m_Level { get { return m_monsterDe.m_level; } }
        public IReadOnlyList<short> m_DropItemIdList { get { return m_monsterDe.m_dropItemIdList; } }
        public Vector2 m_respawnPosition;
        public void Reset (int networkId, Vector2 pos, DE_Unit auDe, DE_MonsterData mDe) {
            base.Reset (auDe);
            m_monsterDe = mDe;
            m_networkId = networkId;
            m_position = pos;
            m_respawnPosition = pos;
        }
        public NO_Monster GetNo () {
            return new NO_Monster (m_networkId, m_position, m_MonsterId, m_MonsterType);
        }
    }
}