using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引所有Character的可接, 不可接, 已接任务
    /// </summary>
    class EM_Mission : EntityManagerBase {
        public static EM_Mission s_instance;
        private DEM_Mission m_dem;
        private Dictionary<int, Dictionary<short, E_Mission>> m_acceptedMissionDict = new Dictionary<int, Dictionary<short, E_Mission>> ();
        /// <summary>可接任务</summary>
        private Dictionary<int, HashSet<short>> m_acceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        /// <summary>已解锁但不可接</summary>
        private Dictionary<int, HashSet<short>> m_unacceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        public EM_Mission (DEM_Mission dem) { m_dem = dem; }
        public void InitCharacter (int netId, int charId, OccupationType charOcp, short charLv, List<DDO_Mission> ddoList) {
            // 读取已接任务
            Dictionary<short, E_Mission> acceptedMissionDict = new Dictionary<short, E_Mission> (ddoList.Count);
            for (int i = 0; i < ddoList.Count; i++) {
                E_Mission mis = s_entityPool.m_missionPool.GetInstance ();
                mis.Reset (m_dem.GetMissionById (ddoList[i].m_missionId), ddoList[i]);
                acceptedMissionDict[ddoList[i].m_missionId] = mis;
            }
            m_acceptedMissionDict.Add (netId, acceptedMissionDict);

            // 获取可接与不可接任务
            var acceptableMissionSet = new HashSet<short> ();
            var unacceptableMissionSet = new HashSet<short> ();
            m_acceptableMissionDict.Add (netId, acceptableMissionSet);
            m_unacceptableMissionDict.Add (netId, unacceptableMissionSet);
            var mEn = acceptedMissionDict.Values.GetEnumerator ();
            while (mEn.MoveNext ())
                DealWithUnlockedMission (mEn.Current, charOcp, charLv, acceptableMissionSet, unacceptableMissionSet);
        }
        public void RemoveCharacter (int netId) {
            m_acceptableMissionDict.Remove (netId);
            m_unacceptableMissionDict.Remove (netId);
            Dictionary<short, E_Mission> mDict = null;
            m_acceptedMissionDict.TryGetValue (netId, out mDict);
            if (mDict == null) return;
            m_acceptedMissionDict.Remove (netId);
            var mEn = mDict.Values.GetEnumerator ();
            while (mEn.MoveNext ())
                s_entityPool.m_missionPool.RecycleInstance (mEn.Current);
        }
        public E_Mission GetAcceptedMission (int netId, short misId) {
            Dictionary<short, E_Mission> acceptedDict = null;
            if (!m_acceptedMissionDict.TryGetValue (netId, out acceptedDict))
                return null;
            E_Mission res = null;
            acceptedDict.TryGetValue (misId, out res);
            return res;
        }
        public E_Mission AcceptMission (int netId, short misId) {
            HashSet<short> acceptableSet = null;
            if (!m_acceptableMissionDict.TryGetValue (netId, out acceptableSet))
                return null;
            Dictionary<short, E_Mission> acceptedDict = null;
            if (!m_acceptedMissionDict.TryGetValue (netId, out acceptedDict))
                return null;
            // 若不可接
            if (!acceptableSet.Contains (misId))
                return null;
            // 实例化任务
            E_Mission mis = s_entityPool.m_missionPool.GetInstance ();
            mis.Reset (m_dem.GetMissionById (misId));
            // 处理可接 已接
            acceptableSet.Remove (misId);
            acceptedDict[misId] = mis;
            return mis;
        }
        public void DeliveryMission (int netId, E_Mission mis, OccupationType ocp, short lv) {
            HashSet<short> acceptableSet = null;
            if (!m_acceptableMissionDict.TryGetValue (netId, out acceptableSet))
                return;
            HashSet<short> unacceptableSet = null;
            if (!m_unacceptableMissionDict.TryGetValue (netId, out unacceptableSet))
                return;
            Dictionary<short, E_Mission> acceptedDict = null;
            if (!m_acceptedMissionDict.TryGetValue (netId, out acceptedDict))
                return;
            // 交付任务
            acceptedDict.Remove (mis.m_MissionId);
            // 回收实例
            s_entityPool.m_missionPool.RecycleInstance (mis);
            // 解锁后续任务
            DealWithUnlockedMission (mis, ocp, lv, acceptableSet, unacceptableSet);
        }
        /// <summary>
        /// 处理任务完成后的解锁
        /// </summary>
        private void DealWithUnlockedMission (E_Mission mis, OccupationType ocp, short lv, HashSet<short> resAcceptable, HashSet<short> resUnacceptable) {
            for (int i = 0; i < mis.m_ChildrenIdList.Count; i++) {
                var de = m_dem.GetMissionById (mis.m_ChildrenIdList[i]);
                // 职业不匹配
                if ((de.m_occupation & ocp) == 0)
                    continue;
                // 等级等其他条件是否满足
                if (lv >= de.m_levelInNeed)
                    resAcceptable.Add (mis.m_ChildrenIdList[i]);
                else
                    resUnacceptable.Add (mis.m_ChildrenIdList[i]);
            }
        }
    }
}