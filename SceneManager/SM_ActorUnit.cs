using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class SM_ActorUnit {
        public static SM_ActorUnit s_instance = new SM_ActorUnit ();
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        private const float c_monsterRefreshTime = 20f;
        private const float c_monsterRefreshTimerCycleTime = 10000f;
        private bool m_monsterRefreshTimerCycle = false;
        private float m_monsterRefreshTimer = 0f;
        private Dictionary<int, Vector2> m_networkIdAndMonsterPosDict = new Dictionary<int, Vector2> ();
        private Dictionary<int, short> m_networkIdAndMonsterIdDict = new Dictionary<int, short> ();
        private Dictionary<int, KeyValuePair<bool, float>> m_networkIdAndMonsterDeathTimeDict = new Dictionary<int, KeyValuePair<bool, float>> ();
        public SM_ActorUnit () {
            // TODO: 用于测试
            int monsterNetId;
            monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
            m_networkIdAndMonsterIdDict[monsterNetId] = 0;
            m_networkIdAndMonsterPosDict[monsterNetId] = new Vector2 (-1, 0);
            m_networkIdAndMonsterDeathTimeDict[monsterNetId] = new KeyValuePair<bool, float> (false, 1f);
            monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
            m_networkIdAndMonsterIdDict[monsterNetId] = 1;
            m_networkIdAndMonsterPosDict[monsterNetId] = new Vector2 (-3, 1);
            m_networkIdAndMonsterDeathTimeDict[monsterNetId] = new KeyValuePair<bool, float> (false, 10f);
        }
        private E_ActorUnit GetActorUnitByNetworkId (int networkId) {
            if (m_networkIdAndCharacterDict.ContainsKey (networkId))
                return m_networkIdAndCharacterDict[networkId];
            if (m_networkIdAndMonsterDict.ContainsKey (networkId))
                return m_networkIdAndMonsterDict[networkId];
            return null;
        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr (int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit> (networkIdArr.Length);
            foreach (var netId in networkIdArr) {
                if (m_networkIdAndCharacterDict.ContainsKey (netId))
                    res.Add (m_networkIdAndCharacterDict[netId]);
                if (m_networkIdAndMonsterDict.ContainsKey (netId))
                    res.Add (m_networkIdAndMonsterDict[netId]);
            }
            return res;
        }
        private void RemoveMonster (int netId) {
            if (m_networkIdAndMonsterDict.ContainsKey (netId))
                m_networkIdAndMonsterDict.Remove (netId);
        }
        public void Tick (float dT) {
            // 每个单位的Tick
            var playerEn = m_networkIdAndCharacterDict.GetEnumerator ();
            while (playerEn.MoveNext ())
                playerEn.Current.Value.Tick (dT);
            var monsterEn = m_networkIdAndMonsterDict.GetEnumerator ();
            while (monsterEn.MoveNext ())
                monsterEn.Current.Value.Tick (dT);

            // 处理怪物刷新
            m_monsterRefreshTimer += dT;
            while (m_monsterRefreshTimer >= c_monsterRefreshTimerCycleTime) {
                m_monsterRefreshTimer -= c_monsterRefreshTimerCycleTime;
                m_monsterRefreshTimerCycle = !m_monsterRefreshTimerCycle;
            }
            List<int> monsterIdToRefreshList = new List<int> ();
            var monsterDeathTimeEn = m_networkIdAndMonsterDeathTimeDict.GetEnumerator ();
            while (monsterDeathTimeEn.MoveNext ())
                if (monsterDeathTimeEn.Current.Value.Key == m_monsterRefreshTimerCycle && monsterDeathTimeEn.Current.Value.Value <= m_monsterRefreshTimer)
                    monsterIdToRefreshList.Add (monsterDeathTimeEn.Current.Key);
            for (int i = 0; i < monsterIdToRefreshList.Count; i++) {
                int monsterIdToRefresh = monsterIdToRefreshList[i];
                m_networkIdAndMonsterDeathTimeDict.Remove (monsterIdToRefresh);
                m_networkIdAndMonsterDict.Add (monsterIdToRefresh, new E_Monster (monsterIdToRefresh, m_networkIdAndMonsterIdDict[monsterIdToRefresh], m_networkIdAndMonsterPosDict[monsterIdToRefresh]));
            }
        }
        public void NetworkTick () {
            foreach (var selfPair in m_networkIdAndCharacterDict) {
                var selfNetId = selfPair.Key;
                var self = selfPair.Value;
                if (self.m_playerId == -1)
                    continue;

                // 发送其他玩家视野信息
                var actorUnitType = ActorUnitType.Player;
                List<int> netIdList = new List<int> ();
                foreach (var otherPair in m_networkIdAndCharacterDict) {
                    if (otherPair.Value.m_playerId == -1)
                        continue;
                    if (otherPair.Key == selfNetId) continue;
                    netIdList.Add (otherPair.Key);
                }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight (selfNetId, actorUnitType, netIdList);
                // 发送怪物视野信息
                actorUnitType = ActorUnitType.Monster;
                netIdList.Clear ();
                foreach (var monsterPair in m_networkIdAndMonsterDict) {
                    netIdList.Add (monsterPair.Key);
                }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight (selfNetId, actorUnitType, netIdList);

                // 发送所有单位的位置信息
                netIdList.Clear ();
                List<Vector2> posList = new List<Vector2> ();
                foreach (var otherPair in m_networkIdAndCharacterDict)
                    if (otherPair.Key != selfNetId) {
                        netIdList.Add (otherPair.Key);
                        posList.Add (otherPair.Value.m_Position);
                    }
                foreach (var monsterPair in m_networkIdAndMonsterDict) {
                    netIdList.Add (monsterPair.Key);
                    posList.Add (monsterPair.Value.m_Position);
                }
                NetworkService.s_instance.NetworkSetOtherPosition (selfNetId, netIdList, posList);

                // 发送所有单位的HP与MP
                netIdList.Clear ();
                List<Dictionary<ActorUnitConcreteAttributeType, int>> HPMPList = new List<Dictionary<ActorUnitConcreteAttributeType, int>> ();
                foreach (var allPair in m_networkIdAndCharacterDict) {
                    var allUnit = allPair.Value;
                    if (allUnit.m_playerId == -1)
                        continue;
                    netIdList.Add (allPair.Key);
                    HPMPList.Add (allUnit.m_concreteAttributeDict);
                }
                foreach (var monsterPair in m_networkIdAndMonsterDict) {
                    var monsterUnit = monsterPair.Value;
                    netIdList.Add (monsterPair.Key);
                    HPMPList.Add (monsterUnit.m_concreteAttributeDict);
                }
                NetworkService.s_instance.NetworkSetAllHPAndMP (selfNetId, netIdList, HPMPList);
            }
        }
        /// <summary>
        /// 在新的玩家连接到服务器后调用
        /// 为它分配并返回一个NetworkId
        /// </summary>
        /// <returns></returns>
        public int CommandAssignNetworkId () {
            return NetworkIdManager.GetNewActorUnitNetworkId ();
        }
        public void CommandRemoveCharacter (int netId) {
            NetworkIdManager.RemoveActorUnitNetworkId (netId);
            if (m_networkIdAndCharacterDict.ContainsKey (netId))
                m_networkIdAndCharacterDict.Remove (netId);
        }
        public void CommandSetCharacterPlayerId (int netId, int playerId) {
            E_Character newChar = new E_Character (netId, playerId);
            m_networkIdAndCharacterDict[netId] = newChar;
            short[] skillIdArr;
            short[] skillLvArr;
            int[] skillMasterlyArr;
            newChar.GetAllLearnedSkill (out skillIdArr, out skillLvArr, out skillMasterlyArr);

            NetworkService.s_instance.NetworkSetSelfInfo (netId, newChar.m_Level, newChar.m_Experience, skillIdArr, skillLvArr, skillMasterlyArr);
        }
        public void CommandSetPosition (int netId, Vector2 pos) {
            m_networkIdAndCharacterDict[netId].SetPosition (pos);
        }
        public void CommandApplyCastSkill (int netId, short skillId, int[] tarIdArr) {
            E_Skill skill = new E_Skill (skillId);
            KeyValuePair<int, E_Status[]>[] statusPairArr = m_networkIdAndCharacterDict[netId].ApplyCastSkill (new E_Skill (skillId), GetActorUnitArrByNetworkIdArr (tarIdArr));
            NetworkService.s_instance.NetworkSetAllEffectToAll(skill.m_skillEffect.m_animId, (byte)skill.m_skillEffect.m_StatusAttachNum, statusPairArr);
        }
        public void CommandApplyActiveEnterFSMState (int netId, FSMActiveEnterState state) {
            m_networkIdAndCharacterDict[netId].ApplyActiveEnterFSMState (state);
            NetworkService.s_instance.NetworkSetSelfFSMStateToOther (netId, state);
        }
        public void CommandAcceptingMission(int netId, short missionId) {
            E_Character character = GetPlayerByNetId(netId);
            E_Mission mission = new E_Mission();
            // TODO:根据任务id从数据库获取任务
            character.AcceptingMission(mission);
            

            NetworkService.s_instance.NetworkConfirmAcceptingMission(netId, missionId);
        }

        public void CommandDeliveringMission(int netId, short missionId) {
            E_Character character = GetPlayerByNetId(netId);
            NetworkService.s_instance.NetworkConfirmDeliveringMission(netId, missionId, character.DeliveringMission(missionId));
        }

        public E_Character GetPlayerByNetId(int netId) {
            E_ActorUnit actorUnit = GetActorUnitByNetworkId(netId);
            if(actorUnit.m_ActorUnitType == ActorUnitType.Player) {
                return (E_Character)actorUnit;
            }
            return null;
        }

        public void CommandCancelMission(int netId, short missionId) {
            E_Character character = GetPlayerByNetId(netId);
            character.CancelMission(missionId);
            NetworkService.s_instance.NetworkConfirmMissionFailed(netId, missionId);
        }
    }
}