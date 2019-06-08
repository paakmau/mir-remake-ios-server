using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_Monster : E_ActorUnit {
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.MONSTER; } }
        public DE_MonsterData m_monsterDe;
        public short m_MonsterId { get { return m_monsterDe.m_monsterId; } }
        public Vector2 m_respawnPosition;
        // 怪物仇恨度哈希表
        private Dictionary<int, MyTimer.Time> m_networkIdAndHatredRefreshDict = new Dictionary<int, MyTimer.Time> ();
        public E_ActorUnit m_highestHatredTarget;
        public void Reset (int networkId, Vector2 pos, DE_Unit auDe, DE_MonsterData mDe) {
            base.Reset (auDe);
            m_monsterDe = mDe;
            m_networkId = networkId;
            m_position = pos;
            m_respawnPosition = pos;
            m_level = mDe.m_level;
        }
        public NO_Monster GetNo () {
            return new NO_Monster (m_networkId, m_position, m_MonsterId);
        }
    }
}