using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_Monster : E_Unit {
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.MONSTER; } }
        public DE_MonsterData m_monsterDe;
        public short m_MonsterId { get { return m_monsterDe.m_monsterId; } }
        public Vector2 m_respawnPosition;
        // 怪物仇恨度哈希表
        private Dictionary<int, MyTimer.Time> m_netIdAndHatredRefreshDict = new Dictionary<int, MyTimer.Time> ();
        public int m_HighestHatredTargetNetId {
            get {
                int res = -1;
                MyTimer.Time resHighest = MyTimer.s_CurTime;
                var en = m_netIdAndHatredRefreshDict.GetEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.Value >= resHighest) {
                        res = en.Current.Key;
                        resHighest = en.Current.Value;
                    }
                }
                return res;
            }
        }
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