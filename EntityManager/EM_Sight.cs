using System.Collections.Generic;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 管理场景中Character的视野信息  
    /// 存放场景中所有能够显示的单位  
    /// </summary>
    class EM_Sight {
        public static EM_Sight s_instance;
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitVisibleDict = new Dictionary<int, E_ActorUnit> ();
        private Dictionary<int, List<E_ActorUnit>> m_networkIdAndActorUnitInSightDict = new Dictionary<int, List<E_ActorUnit>> ();
        /// <summary>
        /// 根据NetId获取他的视野内的单位  
        /// 可以对其视野进行修改  
        /// </summary>
        public List<E_ActorUnit> GetRawActorUnitInSightByNetworkId (int netId) {
            List<E_ActorUnit> res = null;
            m_networkIdAndActorUnitInSightDict.TryGetValue (netId, out res);
            return res;
        }
        public Dictionary<int, E_ActorUnit>.ValueCollection.Enumerator GetActorUnitVisibleEnumerator () {
            return m_networkIdAndActorUnitVisibleDict.Values.GetEnumerator ();
        }
        public void SetUnitVisible (E_ActorUnit unit) {
            m_networkIdAndActorUnitVisibleDict[unit.m_networkId] = unit;
        }
        public void SetUnitInvisible (int netId) {
            m_networkIdAndActorUnitVisibleDict.Remove (netId);
        }
    }
}