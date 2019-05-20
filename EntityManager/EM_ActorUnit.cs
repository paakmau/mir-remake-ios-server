using System.Collections.Generic;


namespace MirRemakeBackend
{
    /// <summary>
    /// 管理场景中的实体, 充当内存池  
    /// 单位, 怪物, 玩家  
    /// </summary>
    class EM_ActorUnit {
        public static EM_ActorUnit s_instance;
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitDict = new Dictionary<int, E_ActorUnit> ();
        public E_ActorUnit GetActorUnitByNetworkId (int networkId) {
            E_ActorUnit res = null;
            m_networkIdAndActorUnitDict.TryGetValue (networkId, out res);
            return res;
        }
        public Dictionary<int, E_ActorUnit>.Enumerator GetActorUnitEnumerator () {
            return m_networkIdAndActorUnitDict.GetEnumerator ();
        }
        public Dictionary<int, E_ActorUnit>.ValueCollection.Enumerator GetActorUnitValueEnumerator () {
            return m_networkIdAndActorUnitDict.Values.GetEnumerator ();
        }
        public void LoadActorUnit (E_ActorUnit unit) {
            m_networkIdAndActorUnitDict.Add (unit.m_networkId, unit);
        }
        public void UnloadActorUnitByNetworkId (int netId) {
            m_networkIdAndActorUnitDict.Remove (netId);
        }
    }
}