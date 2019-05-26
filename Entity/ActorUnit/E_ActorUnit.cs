using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_ActorUnit {
        protected DE_ActorUnit m_actorUnitDe;
        public abstract ActorUnitType m_ActorUnitType { get; }
        public int m_networkId;
        public short m_level;
        public Vector2 m_position;
        public Dictionary<ActorUnitConcreteAttributeType, int> m_concreteAttributeDict = new Dictionary<ActorUnitConcreteAttributeType, int> ();
        public Dictionary<ActorUnitSpecialAttributeType, int> m_specialAttributeDict = new Dictionary<ActorUnitSpecialAttributeType, int> ();
        public int m_MaxHp {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_HP]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_HP] = value; }
        }
        public int m_MaxMp {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_MP]; }
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
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND] = value; }
        }
        public int m_DeltaMpPerSecond {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND] = value; }
        }
        public int m_Attack {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.ATTACK]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.ATTACK] = value; }
        }
        public int m_Magic {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAGIC]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAGIC] = value; }
        }
        public int m_Defence {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DEFENCE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DEFENCE] = value; }
        }
        public int m_Resistance {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.RESISTANCE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.RESISTANCE] = value; }
        }
        public int m_Tenacity {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.TENACITY]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.TENACITY] = value; }
        }
        public int m_Speed {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.SPEED]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.SPEED] = value; }
        }
        public int m_CriticalRate {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_RATE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_RATE] = value; }
        }
        public int m_CriticalBonus {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS] = value; }
        }
        public int m_HitRate {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.HIT_RATE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.HIT_RATE] = value; }
        }
        public int m_DodgeRate {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DODGE_RATE]; }
            set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.DODGE_RATE] = value; }
        }
        public bool m_IsFaint { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.FAINT] > 0; } }
        public bool m_IsSilent { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.SILENT] > 0; } }
        public bool m_IsImmobile { get { return m_specialAttributeDict[ActorUnitSpecialAttributeType.IMMOBILE] > 0; } }
        public bool m_IsDead { get { return m_CurHp <= 0; } }
        public virtual void Reset (DE_ActorUnit de) {
            m_actorUnitDe = de;
            for (int i = 0; i < de.m_concreteAttributeList.Count; i++)
                m_concreteAttributeDict.Add (de.m_concreteAttributeList[i].Item1, de.m_concreteAttributeList[i].Item2);
        }
        public void AddAttr (ActorUnitConcreteAttributeType type, int value) {
            m_concreteAttributeDict[type] += value;
        }
        public virtual void Tick (float dT) {
            if (m_IsDead) return;

            int maxDeltaHP = 0;
            int killerNetId = 0;
            // 移除超时的状态 与 得到状态伤害最高的攻击者
            for (int i = m_statusList.Count - 1; i >= 0; i--) {
                if (MyTimer.CheckTimeUp (m_statusList[i].m_endTime)) {
                    RemoveStatusToAttr (m_statusList[i]);
                    m_statusList.RemoveAt (i);
                } else {
                    int dHP = m_statusList[i].m_DeltaHP;
                    if (dHP > maxDeltaHP) {
                        maxDeltaHP = dHP;
                        killerNetId = m_statusList[i].m_castererNetworkId;
                    }
                }
            }

            // 根据状态处理具体属性的每秒变化
            deltaTimeAfterLastSecond += dT;
            while (deltaTimeAfterLastSecond >= 1.0f) {
                deltaTimeAfterLastSecond -= 1.0f;
                int newHP = m_CurHP + m_DeltaHPPerSecond;
                int newMP = m_CurMP + m_DeltaMPPerSecond;
                m_CurHP = Math.Max (Math.Min (newHP, m_MaxHP), 0);
                m_CurMP = Math.Max (Math.Min (newMP, m_MaxMP), 0);
            }

            if (m_IsDead)
                SM_ActorUnit.s_instance.NotifyUnitDead (killerNetId, m_networkId);
        }
    }
}