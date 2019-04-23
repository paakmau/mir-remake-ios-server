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

                // 发送其他单位的位置信息
                List<Tuple<int, Vector2>> netIdAndPos = new List<Tuple<int, Vector2>>(m_networkIdAndActorUnitDict.Count-1);
                foreach (var otherPair in m_networkIdAndActorUnitDict)
                    if(otherPair.Key != selfId)
                        netIdAndPos.Add(new Tuple<int, Vector2>(otherPair.Key, otherPair.Value.m_Position));
                NetworkService.s_instance.NetworkSetOtherPosition(selfId, netIdAndPos);

                // 发送所有单位的HP与MP
                List<Tuple<int, Dictionary<ActorUnitConcreteAttributeType, int>>> netIdAndHPMP = new List<Tuple<int, Dictionary<ActorUnitConcreteAttributeType, int>>>(m_networkIdAndActorUnitDict.Count);
                foreach (var allPair in m_networkIdAndActorUnitDict)
                    netIdAndHPMP.Add(new Tuple<int, Dictionary<ActorUnitConcreteAttributeType, int>>(allPair.Key, allPair.Value.m_concreteAttributeDict));
                NetworkService.s_instance.NetworkSetAllHPAndMP(selfId, netIdAndHPMP);
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