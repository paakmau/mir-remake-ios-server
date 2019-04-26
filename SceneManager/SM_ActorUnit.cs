using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class SM_ActorUnit {
        public static SM_ActorUnit s_instance = new SM_ActorUnit ();
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        public SM_ActorUnit () {
            // TODO: 用于测试
            AddMonster (NetworkIdManager.GetNewActorUnitNetworkId (), 0, new Vector2 (0, 0));
            AddMonster (NetworkIdManager.GetNewActorUnitNetworkId (), 0, new Vector2 (0, 1));
        }
        private E_ActorUnit GetActorUnitByNetworkId (int networkId) {
            return m_networkIdAndCharacterDict[networkId];
        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr (int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit> (networkIdArr.Length);
            foreach (var netId in networkIdArr)
                res.Add (m_networkIdAndCharacterDict[netId]);
            return res;
        }
        private void AddMonster (int netId, int monsterId, Vector2 pos) {
            m_networkIdAndMonsterDict[netId] = new E_Monster (netId, monsterId, pos);
        }
        private void RemoveMonster (int netId) {
            if (m_networkIdAndMonsterDict.ContainsKey (netId))
                m_networkIdAndMonsterDict.Remove (netId);
        }
        public void Tick (float dT) {
            foreach (var selfPair in m_networkIdAndCharacterDict) {
                var selfNetId = selfPair.Key;
                var self = selfPair.Value;
                if (self.m_playerId == -1)
                    continue;
                // 每个单位的Tick
                self.Tick (dT);

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

                // 发送其他单位的位置信息
                netIdList.Clear ();
                List<Vector2> posList = new List<Vector2> ();
                foreach (var otherPair in m_networkIdAndCharacterDict)
                    if (otherPair.Key != selfNetId) {
                        netIdList.Add (otherPair.Key);
                        posList.Add (otherPair.Value.m_Position);
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
                NetworkService.s_instance.NetworkSetAllHPAndMP (selfNetId, netIdList, HPMPList);
            }
            foreach (var monsterPair in m_networkIdAndMonsterDict) {
                var monsterNetId = monsterPair.Key;
                var monster = monsterPair.Value;
                monster.Tick (dT);
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
            newChar.GetAllLearnedSkill(out skillIdArr, out skillLvArr, out skillMasterlyArr);

            NetworkService.s_instance.NetworkSetSelfInfo (netId, newChar.m_Level, newChar.m_Experience, skillIdArr, skillLvArr, skillMasterlyArr);
        }
        public void CommandSetPosition (int netId, Vector2 pos) {
            m_networkIdAndCharacterDict[netId].SetPosition (pos);
        }
        public void CommandApplyCastSkill (int netId, short skillId, int[] tarIdArr) {
            m_networkIdAndCharacterDict[netId].ApplyCastSkill (new E_Skill (skillId), GetActorUnitArrByNetworkIdArr (tarIdArr));
        }
        public void CommandApplyActiveEnterFSMState (int netId, FSMActiveEnterState state) {
            m_networkIdAndCharacterDict[netId].ApplyActiveEnterFSMState (state);
            NetworkService.s_instance.NetworkSetSelfFSMStateToOther(netId, state);
        }
    }
}