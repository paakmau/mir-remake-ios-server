using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class SM_Character {
        public static SM_Character s_instance = new SM_Character();
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        private E_ActorUnit GetActorUnitByNetworkId(int networkId) {
            return m_networkIdAndCharacterDict[networkId];
        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr(int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit>(networkIdArr.Length);
            foreach (var netId in networkIdArr)
                res.Add(m_networkIdAndCharacterDict[netId]);
            return res;
        }
        public void AddCharacter(int netId) {
            m_networkIdAndCharacterDict[netId] = new E_Character(netId);
        }
        public void RemoveCharacter(int netId) {
            m_networkIdAndCharacterDict.Remove(netId);
        }
        public void Tick(float dT) {
            foreach (var selfPair in m_networkIdAndCharacterDict) {
                var selfId = selfPair.Key;
                var self = selfPair.Value;
                if(self.m_ActorUnitType == ActorUnitType.Player)
                    if(((E_Character)self).m_playerId == -1)
                        continue;
                // 每个单位的Tick
                self.Tick(dT);

                // 发送视野信息
                List<int> netIdList = new List<int>();
                List<ActorUnitType> typeList = new List<ActorUnitType>();
                foreach(var otherPair in m_networkIdAndCharacterDict) {
                    var other = otherPair.Value;
                    if(other.m_ActorUnitType == ActorUnitType.Player)
                        if(((E_Character)other).m_playerId == -1)
                            continue;
                    if(otherPair.Key == selfId) continue;
                    netIdList.Add(otherPair.Key);
                    typeList.Add(otherPair.Value.m_ActorUnitType);
                }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight(selfId, netIdList, typeList);

                // 发送其他单位的位置信息
                netIdList.Clear();
                List<Vector2> posList = new List<Vector2>();
                foreach (var otherPair in m_networkIdAndCharacterDict)
                    if(otherPair.Key != selfId) {
                        netIdList.Add(otherPair.Key);
                        posList.Add(otherPair.Value.m_Position);
                    }
                NetworkService.s_instance.NetworkSetOtherPosition(selfId, netIdList, posList);

                // 发送所有单位的HP与MP
                netIdList.Clear();
                List<Dictionary<ActorUnitConcreteAttributeType, int>> HPMPList = new List<Dictionary<ActorUnitConcreteAttributeType, int>>();
                foreach (var allPair in m_networkIdAndCharacterDict) {
                    var allUnit = allPair.Value;
                    if(allUnit.m_ActorUnitType == ActorUnitType.Player && ((E_Character)allUnit).m_playerId == -1)
                        continue;
                    netIdList.Add(allPair.Key);
                    HPMPList.Add(allUnit.m_concreteAttributeDict);
                }
                NetworkService.s_instance.NetworkSetAllHPAndMP(selfId, netIdList, HPMPList);
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