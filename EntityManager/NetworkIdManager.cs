using System.Collections.Generic;

namespace MirRemakeBackend {
    class NetworkIdManager {
        public static NetworkIdManager s_instance = new NetworkIdManager ();
        private HashSet<int> m_actorUnitNetIdSet = new HashSet<int> ();
        private HashSet<int> m_itemNetIdSet = new HashSet<int> ();
        private int m_actorUnitCnt = 0;
        private int m_itemCnt = 0;
        public int GetNewActorUnitNetworkId () {
            // 分配NetworkId
            while (true) {
                ++m_actorUnitCnt;
                if (!m_actorUnitNetIdSet.Contains (m_actorUnitCnt))
                    break;
            }
            m_actorUnitNetIdSet.Add (m_actorUnitCnt);
            return m_actorUnitCnt;
        }
        public void RemoveActorUnitNetworkId (int netId) {
            m_actorUnitNetIdSet.Remove (netId);
        }
    }
}