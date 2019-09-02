using System.Collections.Generic;


namespace MirRemakeBackend.Entity {
    partial class EM_Item {
        private HashSet<int> m_autoPickUpNetIdSet = new HashSet<int> ();
        public void AutoPickOn (int netId) {
            m_autoPickUpNetIdSet.Add (netId);
        }
        public void AutoPickOff (int netId) {
            m_autoPickUpNetIdSet.Remove (netId);
        }
        public bool IsAutoPickOn (int netId) {
            return m_autoPickUpNetIdSet.Contains(netId);
        }
    }
}