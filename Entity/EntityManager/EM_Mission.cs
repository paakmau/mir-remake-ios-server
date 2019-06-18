using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引所有Character的可接, 不可接, 已接任务
    /// </summary>
    class EM_Mission : EntityManagerBase {
        public static EM_Mission s_instance;
        private DEM_Mission m_dem;
        /// <summary>已接任务</summary>
        private Dictionary<int, Dictionary<short, E_Mission>> m_acceptedMissionDict = new Dictionary<int, Dictionary<short, E_Mission>> ();
        /// <summary>可接任务</summary>
        private Dictionary<int, HashSet<short>> m_acceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        /// <summary>已解锁但不可接</summary>
        private Dictionary<int, HashSet<short>> m_unacceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        public EM_Mission (DEM_Mission dem) { m_dem = dem; }
        public void InitCharacter (int netId, int charId, OccupationType charOcp, short charLv, List<DDO_Mission> ddoList, out List<short> resAcceptedMisIdList, out List<E_Mission> resAcceptedMisObjList, out List<short> resAcceptableMisList, out List<short> resUnacceptableMisList) {
            Dictionary<short, E_Mission> oriAcceptedMisDict;
            HashSet<short> oriAcceptableMisSet;
            HashSet<short> oriUnacceptableMisSet;
            // 若角色已经加载
            if (m_acceptedMissionDict.TryGetValue (netId, out oriAcceptedMisDict) && m_acceptableMissionDict.TryGetValue (netId, out oriAcceptableMisSet) && m_unacceptableMissionDict.TryGetValue (netId, out oriUnacceptableMisSet)) {
                resAcceptedMisIdList = CollectionUtils.GetDictKeyList (oriAcceptedMisDict);
                resAcceptedMisObjList = CollectionUtils.GetDictValueList (oriAcceptedMisDict);
                resAcceptableMisList = CollectionUtils.GetSetList (oriAcceptableMisSet);
                resUnacceptableMisList = CollectionUtils.GetSetList (oriUnacceptableMisSet);
                return;
            }

            // 读取已接任务
            Dictionary<short, E_Mission> acceptedMissionDict = new Dictionary<short, E_Mission> (ddoList.Count);
            for (int i = 0; i < ddoList.Count; i++) {
                if (!ddoList[i].m_isAccepted) continue;
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
            for (int i = 0; i < ddoList.Count; i++) {
                if (ddoList[i].m_isAccepted) continue;
                var de = m_dem.GetMissionById (ddoList[i].m_missionId);
                if (CanAccept (de, charLv))
                    acceptableMissionSet.Add (de.m_id);
                else
                    unacceptableMissionSet.Add (de.m_id);
            }

            // 返回
            resAcceptedMisIdList = CollectionUtils.GetDictKeyList (acceptedMissionDict);
            resAcceptedMisObjList = CollectionUtils.GetDictValueList (acceptedMissionDict);
            resAcceptableMisList = CollectionUtils.GetSetList (acceptableMissionSet);
            resUnacceptableMisList = CollectionUtils.GetSetList (unacceptableMissionSet);
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
        public Dictionary<int, Dictionary<short, E_Mission>>.Enumerator GetAllCharMisEn () {
            return m_acceptedMissionDict.GetEnumerator ();
        }
        public Dictionary<short, E_Mission> GetAllAcceptedMission (int netId) {
            Dictionary<short, E_Mission> res = null;
            m_acceptedMissionDict.TryGetValue (netId, out res);
            return res;
        }
        public E_Mission GetAcceptedMission (int netId, short misId) {
            Dictionary<short, E_Mission> acceptedDict = GetAllAcceptedMission (netId);
            if (acceptedDict == null)
                return null;
            E_Mission res = null;
            acceptedDict.TryGetValue (misId, out res);
            return res;
        }
        public IReadOnlyList<short> GetAllInitUnlockMisDes (OccupationType ocp) {
            // TODO: 应当写在配置里
            var res = new List<short> ();
            var deList = m_dem.GetInitUnlockMisIdList ();
            for (int i = 0; i < deList.Count; i++)
                if (CanUnlock (deList[i], ocp))
                    res.Add (deList[i].m_id);
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
        /// <summary>
        /// 交付一个任务  
        /// 返回解锁任务的修改信息
        /// </summary>
        public void DeliveryMission (int netId, E_Mission mis, OccupationType ocp, short lv, out List<short> resNewAcceptableMis, out List<short> resNewUnacceptableMis) {
            resNewAcceptableMis = null;
            resNewUnacceptableMis = null;
            HashSet<short> acceptableSet = null;
            if (!m_acceptableMissionDict.TryGetValue (netId, out acceptableSet))
                return;
            HashSet<short> unacceptableSet = null;
            if (!m_unacceptableMissionDict.TryGetValue (netId, out unacceptableSet))
                return;
            Dictionary<short, E_Mission> acceptedDict = null;
            if (!m_acceptedMissionDict.TryGetValue (netId, out acceptedDict))
                return;
            // 交付任务 并 回收实例
            acceptedDict.Remove (mis.m_MissionId);
            s_entityPool.m_missionPool.RecycleInstance (mis);
            // 后续任务解锁
            resNewAcceptableMis = new List<short> ();
            resNewUnacceptableMis = new List<short> ();
            for (int i = 0; i < mis.m_ChildrenIdList.Count; i++) {
                var de = m_dem.GetMissionById (mis.m_ChildrenIdList[i]);
                if (!CanUnlock (de, ocp))
                    continue;
                if (CanAccept (de, lv)) {
                    acceptableSet.Add (de.m_id);
                    resNewAcceptableMis.Add (de.m_id);
                } else {
                    unacceptableSet.Add (de.m_id);
                    resNewUnacceptableMis.Add (de.m_id);
                }
            }
        }
        public void CancelMission (int netId, E_Mission mis) {
            HashSet<short> acceptableSet = null;
            if (!m_acceptableMissionDict.TryGetValue (netId, out acceptableSet))
                return;
            Dictionary<short, E_Mission> acceptedDict = null;
            if (!m_acceptedMissionDict.TryGetValue (netId, out acceptedDict))
                return;
            // 放弃任务 并 回收实例
            acceptedDict.Remove (mis.m_MissionId);
            s_entityPool.m_missionPool.RecycleInstance (mis);
            acceptableSet.Add (mis.m_MissionId);
        }
        /// <summary>
        /// 刷新已解锁任务中的可接任务
        /// </summary>
        public void RefreshUnlockedMission (int netId, short lv) {
            HashSet<short> unaMisSet;
            HashSet<short> acableMisSet;
            if (!m_unacceptableMissionDict.TryGetValue (netId, out unaMisSet) ||
                !m_acceptableMissionDict.TryGetValue (netId, out acableMisSet))
                return;
            var en = unaMisSet.GetEnumerator ();
            var changedList = new List<short> (unaMisSet.Count);
            while (en.MoveNext ()) {
                var de = m_dem.GetMissionById (en.Current);
                if (CanAccept (de, lv))
                    changedList.Add (en.Current);
            }
            for (int i = 0; i < changedList.Count; i++) {
                unaMisSet.Remove (changedList[i]);
                acableMisSet.Add (changedList[i]);
            }
        }
        private bool CanUnlock (DE_Mission de, OccupationType ocp) {
            if ((de.m_occupation & ocp) == 0)
                return false;
            return true;
        }
        private bool CanAccept (DE_Mission de, short lv) {
            if (de.m_levelInNeed > lv)
                return false;
            return true;
        }
    }
}