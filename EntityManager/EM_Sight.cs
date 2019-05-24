using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 管理场景中Character的视野信息  
    /// 存放场景中所有能够显示的单位  
    /// </summary>
    class EM_Sight : EntityManagerBase {
        public static EM_Sight s_instance;
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitVisibleDict = new Dictionary<int, E_ActorUnit> ();
        /// <summary>
        /// 每个角色的视野信息, (包括自身)
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, List<E_ActorUnit>> m_networkIdAndActorUnitListInSightDict = new Dictionary<int, List<E_ActorUnit>> ();
        /// <summary>
        /// 循环链表  
        /// 每一帧获取若干个角色为其计算视野  
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <returns></returns>
        private LinkedList<int> m_unitNetworkIdLinkedList = new LinkedList<int> ();
        /// <summary>
        /// 根据NetId获取他的视野内的单位 (包括自身)  
        /// 可以对其视野进行读写  
        /// </summary>
        public List<E_ActorUnit> GetRawActorUnitsInSightByNetworkId (int netId) {
            List<E_ActorUnit> res = null;
            m_networkIdAndActorUnitListInSightDict.TryGetValue (netId, out res);
            return res;
        }
        public IReadOnlyList<int> GetActorUnitsInSightNetworkIdByNetworkId (int netId, bool includeSelf) {
            var rawList = GetRawActorUnitsInSightByNetworkId (netId);
            if (rawList == null) return null;
            List<int> res = new List<int> ();
            for (int i = 0; i < rawList.Count; i++)
                if (includeSelf || rawList[i].m_networkId != netId)
                    res.Add (rawList[i].m_networkId);
            return res;
        }
        public E_ActorUnit GetActorUnitVisibleByNetworkId (int netId) {
            E_ActorUnit res = null;
            m_networkIdAndActorUnitVisibleDict.TryGetValue (netId, out res);
            return res;
        }
        public Dictionary<int, E_ActorUnit>.ValueCollection.Enumerator GetActorUnitVisibleEnumerator () {
            return m_networkIdAndActorUnitVisibleDict.Values.GetEnumerator ();
        }
        public bool TryGetNextCharacterNetworkIdToGetSight (out int netId) {
            if (m_unitNetworkIdLinkedList.Count == 0) {
                netId = 0;
                return false;
            }
            netId = m_unitNetworkIdLinkedList.First.Value;
            m_unitNetworkIdLinkedList.RemoveFirst ();
            m_unitNetworkIdLinkedList.AddLast (netId);
            return true;
        }
        public void InitCharacterSight (int netId) {
            m_unitNetworkIdLinkedList.AddFirst (netId);
            m_networkIdAndActorUnitListInSightDict.Add (netId, new List<E_ActorUnit> ());
        }
        public void RemoveCharacterSight (int netId) {
            m_networkIdAndActorUnitListInSightDict.Remove (netId);
        }
        public void SetUnitVisible (E_ActorUnit unit) {
            m_networkIdAndActorUnitVisibleDict[unit.m_networkId] = unit;
        }
        public void SetUnitInvisible (int netId) {
            m_networkIdAndActorUnitVisibleDict.Remove (netId);
        }
    }
}