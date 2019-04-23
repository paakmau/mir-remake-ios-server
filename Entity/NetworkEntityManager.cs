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