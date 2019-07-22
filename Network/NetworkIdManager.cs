using System.Collections.Generic;
namespace MirRemakeBackend.Network {
    class NetworkIdManager {
        public static NetworkIdManager s_instance = new NetworkIdManager ();
        private HashSet<int> m_unitNetIdSet = new HashSet<int> () {-1 };
        private int m_unitCnt = 0;
        public int AssignNetworkId () {
            // 分配NetworkId
            while (true) {
                ++m_unitCnt;
                if (!m_unitNetIdSet.Contains (m_unitCnt))
                    break;
            }
            m_unitNetIdSet.Add (m_unitCnt);
            return m_unitCnt;
        }
        public int[] AssignNetworkId (int num) {
            int[] res = new int[num];
            for (int i = 0; i < num; i++)
                res[i] = AssignNetworkId ();
            return res;
        }
        public void RecycleNetworkId (int netId) {
            m_unitNetIdSet.Remove (netId);
        }
    }
}