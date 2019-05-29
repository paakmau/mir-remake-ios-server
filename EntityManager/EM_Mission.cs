using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 索引所有Character的任务
    /// </summary>
    class EM_Mission : EntityManagerBase {
        public static EM_Mission s_instance;
        private Dictionary<int, List<E_Mission>> m_networkIdAndMissionDict = new Dictionary<int, List<E_Mission>> ();
        public List<E_Mission> InitCharacterMission (int netId, int charId, List<DDO_Mission> ddoList) {
            List<E_Mission> mList = new List<E_Mission> (ddoList.Count);
            for (int i = 0; i < ddoList.Count; i++) {
                E_Mission mis = s_entityPool.m_missionPool.GetInstance ();
                DE_Mission de = DEM_Mission.s_instance.GetMissionById (ddoList[i].m_missionId);
                mis.Reset (de, ddoList[i]);
                mList[i] = mis;
            }
            m_networkIdAndMissionDict.Add (netId, mList);
            return mList;
        }
        public void RemoveCharacter (int netId) {
            List<E_Mission> mList = null;
            m_networkIdAndMissionDict.TryGetValue (netId, out mList);
            if (mList == null) return;
            m_networkIdAndMissionDict.Remove (netId);
            for (int i=0; i<mList.Count; i++)
                s_entityPool.m_missionPool.RecycleInstance (mList[i]);
        }
        public List<E_Mission> GetMissionListByNetworkId (int netId) {
            List<E_Mission> res = null;
            m_networkIdAndMissionDict.TryGetValue (netId, out res);
            return res;
        }
    }
}