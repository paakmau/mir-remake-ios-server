using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend
{
    class SM_Monster
    {
        private const float c_monsterRespawnTimeMin = 10f;
        private const float c_monsterRespawnTimeMax = 20f;
        private Dictionary<int, KeyValuePair<short, Vector2>> m_networkIdAndMonsterIdAndPosDict = new Dictionary<int, KeyValuePair<short, Vector2>> ();
        private Dictionary<int, MyTimer.Time> m_networkIdAndMonsterRespawnTimeDict = new Dictionary<int, MyTimer.Time> ();
        private Stack<int> m_monsterToRespawnNetwordIdStack = new Stack<int> ();
        public SM_Monster () {
            var monsterIdAndPositionArr = m_monsterDataService.GetAllMonsterSpawnPosition ();
            foreach (var monsterIdAndPos in monsterIdAndPositionArr) {
                int monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
                InitMonster (monsterNetId, monsterIdAndPos);
            }
            Messenger.AddListener<int> ("NotifyMonsterDie", NotifyMonsterDie);
        }
        public void Tick (float dT) {
            var monsterRespawnTimeEn = m_networkIdAndMonsterRespawnTimeDict.GetEnumerator ();
            while (monsterRespawnTimeEn.MoveNext ())
                if (MyTimer.CheckTimeUp (monsterRespawnTimeEn.Current.Value))
                    m_monsterToRespawnNetwordIdStack.Push (monsterRespawnTimeEn.Current.Key);
            // 处理怪物刷新
            int monsterToRespawnNetId;
            while (m_monsterToRespawnNetwordIdStack.TryPop (out monsterToRespawnNetId)) {
                m_networkIdAndMonsterRespawnTimeDict.Remove (monsterToRespawnNetId);
                DO_Monster monsterDo = m_monsterDataService.GetMonsterById (monsterToRespawnNetIdAndInfo.Value.Key);
                E_Skill[] monsterSkillArr = m_skillSceneManager.InitMonsterSkill (monsterToRespawnNetIdAndInfo.Key, monsterDo.m_skillIdAndLevelArr);
                EM_ActorUnit.LoadActorUnit (new E_Monster (monsterToRespawnNetIdAndInfo.Key, monsterToRespawnNetIdAndInfo.Value.Value, monsterDo, monsterSkillArr));
            }
        }
        public void InitMonster (int netId, KeyValuePair<short, Vector2> monsterIdAndPos) {
            m_networkIdAndMonsterIdAndPosDict[netId] = monsterIdAndPos;
            m_networkIdAndMonsterRespawnTimeDict[netId] = MyTimer.s_CurTime.Ticked (MyRandom.NextFloat (c_monsterRespawnTimeMin, c_monsterRespawnTimeMax));
        }
        public void NotifyMonsterDie (int netId) {
            MyTimer.Time refreshTime = MyTimer.s_CurTime.Ticked (MyRandom.NextFloat (c_monsterRespawnTimeMin, c_monsterRespawnTimeMax));
            m_networkIdAndMonsterRespawnTimeDict[netId] = refreshTime;
        }
    }
}