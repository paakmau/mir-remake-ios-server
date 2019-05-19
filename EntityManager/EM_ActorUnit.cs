using System.Collections.Generic;


namespace MirRemakeBackend
{
    /// <summary>
    /// 管理游戏场景中出现的所有单位(包括尸体)
    /// </summary>
    static class EM_ActorUnit {
        private static Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitDict = new Dictionary<int, E_ActorUnit> ();
        public static E_ActorUnit GetActorUnitByNetworkId (int networkId) {
            E_ActorUnit res = null;
            m_networkIdAndActorUnitDict.TryGetValue (networkId, out res);
            return res;
        }
        public static Dictionary<int, E_ActorUnit>.Enumerator GetActorUnitEnumerator () {
            return m_networkIdAndActorUnitDict.GetEnumerator ();
        }
        public static Dictionary<int, E_ActorUnit>.ValueCollection.Enumerator GetActorUnitValueEnumerator () {
            return m_networkIdAndActorUnitDict.Values.GetEnumerator ();
        }
        public static void LoadActorUnit (E_ActorUnit unit) {
            m_networkIdAndActorUnitDict.Add (unit.m_networkId, unit);
        }
        public static void UnloadActorUnitByNetworkId (int netId) {
            m_networkIdAndActorUnitDict.Remove (netId);
        }
    }
}