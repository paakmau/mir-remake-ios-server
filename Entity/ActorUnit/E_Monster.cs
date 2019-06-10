using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
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
}