using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    class EM_Status : EntityManagerBase {
        public static EM_Status s_instance;
        private DEM_Status m_dem;
        private Dictionary<int, List<E_Status>> m_networkIdAndStatusListDict = new Dictionary<int, List<E_Status>> ();
        private List<E_Status> t_statusList = new List<E_Status> ();
        public EM_Status (DEM_Status dem) { m_dem = dem; }
        public void InitCharacterStatus (int netId) {
            m_networkIdAndStatusListDict.Add (netId, new List<E_Status> ());
        }
        public void RemoveCharacterStatus (int netId) {
            List<E_Status> statusList = null;
            m_networkIdAndStatusListDict.TryGetValue (netId, out statusList);
            if (statusList == null) return;
            m_networkIdAndStatusListDict.Remove (netId);
            for (int i=0; i<statusList.Count; i++)
                s_entityPool.m_statusPool.RecycleInstance (statusList[i]);
        }
        public List<E_Status> AttachStatus (int netId, ValueTuple<short, float, float, int>[] statusIdAndValueAndTimeAndCasterNetIdArr) {
            List<E_Status> oriStatusList = null;
            if (!m_networkIdAndStatusListDict.TryGetValue (netId, out oriStatusList))
                return null;
            t_statusList.Clear ();
            foreach (var statusInfo in statusIdAndValueAndTimeAndCasterNetIdArr) {
                var de = m_dem.GetStatusById (statusInfo.Item1);
                var statusObj = s_entityPool.m_statusPool.GetInstance ();
                statusObj.Reset (de, statusInfo.Item1, statusInfo.Item2, statusInfo.Item3, statusInfo.Item4);
                oriStatusList.Add (statusObj);
            }
            return t_statusList;
        }
        public void RemoveOrderedStatus (int netId, List<int> orderedIndexList) {
            List<E_Status> statusList = null;
            m_networkIdAndStatusListDict.TryGetValue (netId, out statusList);
            if (statusList == null)
                return;
            for (int i = orderedIndexList.Count - 1; i >= 0; i--) {
                E_Status obj = statusList[orderedIndexList[i]];
                s_entityPool.m_statusPool.RecycleInstance (obj);
                statusList.RemoveAt (orderedIndexList[i]);
            }
        }
        public Dictionary<int, List<E_Status>>.Enumerator GetStatusEn () {
            return m_networkIdAndStatusListDict.GetEnumerator ();
        }
    }
}