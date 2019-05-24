using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    class EM_Status : EntityManagerBase {
        public static EM_Status s_instance;
        Dictionary<int, List<E_Status>> m_networkIdAndStatusListDict = new Dictionary<int, List<E_Status>> ();
        public E_Status AssignStatus () {
            return s_entityPool.m_statusPool.GetInstance ();
        }
        public void InitCharacterStatus (int netId) {
            m_networkIdAndStatusListDict.Add (netId, new List<E_Status> ());
        }
        public void RemoveCharacterStatus (int netId) {
            m_networkIdAndStatusListDict.Remove (netId);
        }
        public void AddStatus (int netId, List<E_Status> newStatusList) {
            List<E_Status> oriStatusList = null;
            if (!m_networkIdAndStatusListDict.TryGetValue (netId, out oriStatusList))
                return; 
            for (int i=0; i<newStatusList.Count; i++)
                oriStatusList.Add (newStatusList[i]);
        }
        /// <summary>
        /// 获取原始状态列表  
        /// 可以读写
        /// </summary>
        /// <param name="netId"></param>
        /// <returns></returns>
        public List<E_Status> GetRawStatusListByNetworkId (int netId) {   
            List<E_Status> res = null;
            m_networkIdAndStatusListDict.TryGetValue (netId, out res);
            return res;
        }
    }
}