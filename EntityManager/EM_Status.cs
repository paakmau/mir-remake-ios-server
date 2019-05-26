using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    class EM_Status : EntityManagerBase {
        public static EM_Status s_instance;
        Dictionary<int, List<E_Status>> m_networkIdAndStatusListDict = new Dictionary<int, List<E_Status>> ();
        public void InitCharacterStatus (int netId) {
            m_networkIdAndStatusListDict.Add (netId, new List<E_Status> ());
        }
        public void RemoveCharacterStatus (int netId) {
            m_networkIdAndStatusListDict.Remove (netId);
        }
        public void AddStatus (int netId, ValueTuple<short, float, float, int>[] statusIdAndValueAndTimeAndCasterNetIdArr) {
            List<E_Status> oriStatusList = null;
            if (!m_networkIdAndStatusListDict.TryGetValue (netId, out oriStatusList))
                return;
            foreach (var statusInfo in statusIdAndValueAndTimeAndCasterNetIdArr) {
                var statusObj = s_entityPool.m_statusPool.GetInstance ();
                var de = DEM_Status.s_instance.GetStatusById (statusInfo.Item1);
                statusObj.Reset (de, statusInfo.Item1, statusInfo.Item2, statusInfo.Item3, statusInfo.Item4);
                oriStatusList.Add (statusObj);
            }
        }
        public void RemoveStatus (int netId, List<int> orderedIndexList) {
            List<E_Status> statusList = null;
            m_networkIdAndStatusListDict.TryGetValue (netId, out statusList);
            if (statusList == null) return;
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