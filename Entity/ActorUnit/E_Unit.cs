using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_Unit {
        protected DE_Unit m_unitDe;
        public abstract ActorUnitType m_UnitType { get; }
        public int m_networkId;
        public abstract short m_Level { get; }
        public Vector2 m_position;
        private Dictionary<ActorUnitConcreteAttributeType, int> m_concreteAttributeDict = new Dictionary<ActorUnitConcreteAttributeType, int> ();
        private Dictionary<ActorUnitSpecialAttributeType, int> m_specialAttributeDict = new Dictionary<ActorUnitSpecialAttributeType, int> ();
        public int m_finalAttackerNetId;
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
        public int m_CurHp {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_HP]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_HP] = value; }
        }
        public int m_CurMp {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_MP]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_MP] = value; }
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
        public int m_DodgeRate {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DODGE_RATE] + m_unitDe.m_concreteAttributeDict[ActorUnitConcreteAttributeType.DODGE_RATE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DODGE_RATE] = value; }
        }
        public bool m_IsFaint { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] > 0; } }
        public bool m_IsSilent { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] > 0; } }
        public bool m_IsImmobile { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] > 0; } }
        public bool m_IsDead { get { return m_CurHp <= 0; } }
        public virtual void Reset (DE_Unit de) {
            m_unitDe = de;
            // TODO: 具体属性使用 类似 *1000 + 1, 2, 3 的方式设计
            var attrEn = de.m_concreteAttributeDict.Keys.GetEnumerator ();
            while(attrEn.MoveNext ())
                m_concreteAttributeDict[attrEn.Current] = 0;
            m_CurHp = m_MaxHp;
            m_CurMp = m_MaxMp;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] = 0;
            m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] = 0;
        }
        public void Respawn () {
            m_CurHp = m_MaxHp;
            m_CurMp = m_MaxMp;
        }
        public void AddConAttr (ActorUnitConcreteAttributeType type, int value) {
            m_concreteAttributeDict[type] += value;
        }
        public void AddSpAttr (ActorUnitSpecialAttributeType type, int value) {
            m_specialAttributeDict[type] += value;
        }
    }
}