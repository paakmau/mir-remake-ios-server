using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引所有Character的任务
    /// </summary>
    class EM_Mission : EntityManagerBase {
        public static EM_Mission s_instance;
        private DEM_Mission m_dem;
        private Dictionary<int, List<E_Mission>> m_acceptedMissionDict = new Dictionary<int, List<E_Mission>> ();
        private Dictionary<int, HashSet<short>> m_unlockedMissionDict = new Dictionary<int, HashSet<short>> ();
        public EM_Mission (DEM_Mission dem) { m_dem = dem; }
        public void InitCharacterMission (int netId, int charId, List<DDO_Mission> ddoList) {
            List<E_Mission> acceptedMissionList = new List<E_Mission> (ddoList.Count);
            for (int i = 0; i < ddoList.Count; i++) {
                E_Mission mis = s_entityPool.m_missionPool.GetInstance ();
                DE_Mission de = m_dem.GetMissionById (ddoList[i].m_missionId);
                mis.Reset (de, ddoList[i]);
                acceptedMissionList[i] = mis;
            }
            m_acceptedMissionDict.Add (netId, acceptedMissionList);

            var unlockedMissionSet = new HashSet<short> ();
            m_unlockedMissionDict.Add (netId, unlockedMissionSet);
            for (int i = 0; i < acceptedMissionList.Count; i++)
                for (int j=0; j<acceptedMissionList[i].m_ChildrenIdList.Count; j++)
                    unlockedMissionSet.Add (acceptedMissionList[i].m_ChildrenIdList[i]);
        }
        public void RemoveCharacter (int netId) {
            List<E_Mission> mList = null;
            m_acceptedMissionDict.TryGetValue (netId, out mList);
            if (mList == null) return;
            m_acceptedMissionDict.Remove (netId);
            for (int i=0; i<mList.Count; i++)
                s_entityPool.m_missionPool.RecycleInstance (mList[i]);
        }
        public List<E_Mission> GetRawAcceptedMissionListByNetworkId (int netId) {
            List<E_Mission> res = null;
            m_acceptedMissionDict.TryGetValue (netId, out res);
            return res;
        }
    }
}