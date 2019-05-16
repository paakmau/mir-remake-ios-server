using System.Collections.Generic;

namespace MirRemakeBackend {
    static class NetworkIdManager {
        private static HashSet<int> m_actorUnitNetIdSet = new HashSet<int> ();
        private static HashSet<int> m_itemNetIdSet = new HashSet<int> ();
        private static int m_actorUnitCnt = 0;
        private static int m_itemCnt = 0;

        public static int GetNewActorUnitNetworkId() {
            // 分配NetworkId
            while(true) {
                ++m_actorUnitCnt;
                if(!m_actorUnitNetIdSet.Contains(m_actorUnitCnt))
                    break;
            }
            m_actorUnitNetIdSet.Add(m_actorUnitCnt);
            return m_actorUnitCnt;
        }
        public static void RemoveActorUnitNetworkId(int netId) {
            m_actorUnitNetIdSet.Remove(netId);
        }
    }
}