using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class SM_ActorUnit {
        public static SM_ActorUnit s_instance = new SM_ActorUnit();
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster>();
        public SM_ActorUnit() {
            // TODO: 用于测试
            AddMonster(0, new Vector2(0, 0));
            AddMonster(0, new Vector2(0, 1));
        }
        private E_ActorUnit GetActorUnitByNetworkId(int networkId) {
            return m_networkIdAndCharacterDict[networkId];
        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr(int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit>(networkIdArr.Length);
            foreach (var netId in networkIdArr)
                res.Add(m_networkIdAndCharacterDict[netId]);
            return res;
        }
        public int AddCharacter() {
            int netId = NetworkIdManager.GetNewActorUnitNetworkId();
            m_networkIdAndCharacterDict[netId] = new E_Character(netId);
            return netId;
        }
        public void RemoveCharacter(int netId) {
            NetworkIdManager.RemoveActorUnitNetworkId(netId);
            m_networkIdAndCharacterDict.Remove(netId);
        }
        private int AddMonster(int monsterId, Vector2 pos) {
            int netId = NetworkIdManager.GetNewActorUnitNetworkId();
            m_networkIdAndMonsterDict[netId] = new E_Monster(netId, monsterId, pos);
            return netId;
        }
        private void RemoveMonster(int netId) {
            NetworkIdManager.RemoveActorUnitNetworkId(netId);
            m_networkIdAndMonsterDict.Remove(netId);
        }
        public void Tick(float dT) {
            foreach (var selfPair in m_networkIdAndCharacterDict) {
                var selfNetId = selfPair.Key;
                var self = selfPair.Value;
                if(self.m_playerId == -1)
                    continue;
                // 每个单位的Tick
                self.Tick(dT);

                // 发送其他玩家视野信息
                var actorUnitType = ActorUnitType.Player;
                List<int> netIdList = new List<int>();
                foreach(var otherPair in m_networkIdAndCharacterDict) {
                    if(otherPair.Value.m_playerId == -1)
                        continue;
                    if(otherPair.Key == selfNetId) continue;
                    netIdList.Add(otherPair.Key);
                }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight(selfNetId, actorUnitType, netIdList);
                // 发送怪物视野信息
                actorUnitType = ActorUnitType.Monster;
                netIdList.Clear();
                foreach (var monsterPair in m_networkIdAndMonsterDict) {
                    netIdList.Add(monsterPair.Key);
                }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight(selfNetId, actorUnitType, netIdList);

                // 发送其他单位的位置信息
                netIdList.Clear();
                List<Vector2> posList = new List<Vector2>();
                foreach (var otherPair in m_networkIdAndCharacterDict)
                    if(otherPair.Key != selfNetId) {
                        netIdList.Add(otherPair.Key);
                        posList.Add(otherPair.Value.m_Position);
                    }
                NetworkService.s_instance.NetworkSetOtherPosition(selfNetId, netIdList, posList);

                // 发送所有单位的HP与MP
                netIdList.Clear();
                List<Dictionary<ActorUnitConcreteAttributeType, int>> HPMPList = new List<Dictionary<ActorUnitConcreteAttributeType, int>>();
                foreach (var allPair in m_networkIdAndCharacterDict) {
                    var allUnit = allPair.Value;
                    if(allUnit.m_playerId == -1)
                        continue;
                    netIdList.Add(allPair.Key);
                    HPMPList.Add(allUnit.m_concreteAttributeDict);
                }
                NetworkService.s_instance.NetworkSetAllHPAndMP(selfNetId, netIdList, HPMPList);
            }
            foreach(var monsterPair in m_networkIdAndMonsterDict) {
                var monsterNetId = monsterPair.Key;
                var monster = monsterPair.Value;
                monster.Tick(dT);
            }
        }
        public void CommandSetPlayerId(int netId, int playerId) {
            ((E_Character)m_networkIdAndCharacterDict[netId]).SetPlayerInfo(playerId);
        }
        public void CommandSetPosition(int netId, Vector2 pos) {
            m_networkIdAndCharacterDict[netId].SetPosition(pos);
        }
        public void CommandApplyCastSkill(int netId, short skillId, int[] tarIdArr) {
            m_networkIdAndCharacterDict[netId].ApplyCastSkill(new E_Skill(skillId), GetActorUnitArrByNetworkIdArr(tarIdArr));
        }
        public void CommandApplyActiveEnterFSMState(int netId, FSMActiveEnterState state) {
            m_networkIdAndCharacterDict[netId].ApplyActiveEnterFSMState(state);
        }
    }
}