using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class NetworkEntityManager {
        public static NetworkEntityManager s_instance = new NetworkEntityManager();
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitDict = new Dictionary<int, E_ActorUnit> ();
        private E_ActorUnit GetActorUnitByNetworkId(int networkId) {
            return m_networkIdAndActorUnitDict[networkId];
        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr(int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit>(networkIdArr.Length);
            foreach (var netId in networkIdArr)
                res.Add(m_networkIdAndActorUnitDict[netId]);
            return res;
        }
        public void AddCharacter(int netId) {
            m_networkIdAndActorUnitDict.Add(netId, new E_Character(netId));
        }
        public void RemoveCharacter(int netId) {
            m_networkIdAndActorUnitDict.Remove(netId);
        }
        public void Tick(float dT) {
            foreach (var selfPair in m_networkIdAndActorUnitDict) {
                var selfId = selfPair.Key;
                var self = selfPair.Value;
                // 每个单位的Tick
                self.Tick(dT);

                // 发送视野信息
                List<int> netIdList = new List<int>();
                List<ActorUnitType> typeList = new List<ActorUnitType>();
                foreach(var otherPair in m_networkIdAndActorUnitDict)
                    if(otherPair.Key != selfId) {
                        netIdList.Add(otherPair.Key);
                        typeList.Add(otherPair.Value.m_ActorUnitType);
                    }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight(selfId, netIdList, typeList);

                // 发送其他单位的位置信息
                netIdList.Clear();
                List<Vector2> posList = new List<Vector2>();
                foreach (var otherPair in m_networkIdAndActorUnitDict)
                    if(otherPair.Key != selfId) {
                        netIdList.Add(otherPair.Key);
                        posList.Add(otherPair.Value.m_Position);
                    }
                NetworkService.s_instance.NetworkSetOtherPosition(selfId, netIdList, posList);

                // 发送所有单位的HP与MP
                netIdList.Clear();
                List<Dictionary<ActorUnitConcreteAttributeType, int>> HPMPList = new List<Dictionary<ActorUnitConcreteAttributeType, int>>();
                foreach (var allPair in m_networkIdAndActorUnitDict) {
                    netIdList.Add(allPair.Key);
                    HPMPList.Add(allPair.Value.m_concreteAttributeDict);
                }
                NetworkService.s_instance.NetworkSetAllHPAndMP(selfId, netIdList, HPMPList);
            }
        }
        public void CommandSetPosition(int netId, Vector2 pos) {
            m_networkIdAndActorUnitDict[netId].SetPosition(pos);
        }
        public void CommandApplyCastSkill(int netId, short skillId, int[] tarIdArr) {
            m_networkIdAndActorUnitDict[netId].ApplyCastSkill(new E_Skill(skillId), GetActorUnitArrByNetworkIdArr(tarIdArr));
        }
        public void CommandApplyActiveEnterFSMState(int netId, FSMActiveEnterState state) {
            m_networkIdAndActorUnitDict[netId].ApplyActiveEnterFSMState(state);
        }
    }
}