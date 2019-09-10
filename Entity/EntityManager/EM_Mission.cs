using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引所有Character的可接, 不可接, 已接任务
    /// 称号
    /// </summary>
    class EM_Mission {
        private class MissionFactory {
            private interface IMissionTargetReseter {
                MissionTargetType m_Type { get; }
                void Reset (IMissionTarget resMt, short tarId, int progress, DEM_Mission dem);
            }
            private class MTR_TalkToNpc : IMissionTargetReseter {
                public MissionTargetType m_Type { get { return MissionTargetType.TALK_TO_NPC; } }
                public void Reset (IMissionTarget resMt, short tarId, int progress, DEM_Mission dem) {
                    (resMt as E_MissionTargetTalkToNpc).Reset (tarId, progress);
                }
            }
            private class MTR_KillMonster : IMissionTargetReseter {
                public MissionTargetType m_Type { get { return MissionTargetType.KILL_MONSTER; } }
                public void Reset (IMissionTarget resMt, short tarId, int progress, DEM_Mission dem) {
                    var de = dem.GetMissionTargetKillMonster (tarId);
                    (resMt as E_MissionTargetKillMonster).Reset (de, progress);
                }
            }
            private class MTR_GainItem : IMissionTargetReseter {
                public MissionTargetType m_Type { get { return MissionTargetType.GAIN_ITEM; } }
                public void Reset (IMissionTarget resMt, short tarId, int progress, DEM_Mission dem) {
                    var de = dem.GetMissionTargetGainItem (tarId);
                    (resMt as E_MissionTargetGainItem).Reset (de, progress);
                }
            }
            private class MTR_LevelUpSkill : IMissionTargetReseter {
                public MissionTargetType m_Type { get { return MissionTargetType.LEVEL_UP_SKILL; } }
                public void Reset (IMissionTarget resMt, short tarId, int progress, DEM_Mission dem) {
                    var de = dem.GetMissionTargetLevelUpSkill (tarId);
                    (resMt as E_MissionTargetLevelUpSkill).Reset (de, progress);
                }
            }
            private const int c_missionPoolSize = 400;
            private const int c_misTarPoolSize = 400;
            private DEM_Mission m_dem;
            private ObjectPool<E_Mission> m_misPool = new ObjectPool<E_Mission> (c_missionPoolSize);
            private Dictionary<MissionTargetType, ObjectPool> m_misTarPoolDict = new Dictionary<MissionTargetType, ObjectPool> () { { MissionTargetType.GAIN_ITEM, new ObjectPool<E_MissionTargetGainItem> (c_misTarPoolSize) }, { MissionTargetType.KILL_MONSTER, new ObjectPool<E_MissionTargetKillMonster> (c_misTarPoolSize) }, { MissionTargetType.LEVEL_UP_SKILL, new ObjectPool<E_MissionTargetLevelUpSkill> (c_misTarPoolSize) }, { MissionTargetType.TALK_TO_NPC, new ObjectPool<E_MissionTargetTalkToNpc> (c_misTarPoolSize) } };
            private Dictionary<MissionTargetType, IMissionTargetReseter> m_misTarReseterDict = new Dictionary<MissionTargetType, IMissionTargetReseter> ();
            public MissionFactory (DEM_Mission dem) {
                m_dem = dem;
                // 实例化所有 MTR 接口的实现类
                var type = typeof (IMissionTargetReseter);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => p.IsClass && type.IsAssignableFrom (p));
                foreach (var implType in implTypes) {
                    IMissionTargetReseter implObj = implType.GetConstructor (Type.EmptyTypes).Invoke (null) as IMissionTargetReseter;
                    m_misTarReseterDict.Add (implObj.m_Type, implObj);
                }
            }
            public E_Mission GetInstance (short misId, List<int> misTarProgressList) {
                var res = m_misPool.GetInstance ();
                var de = m_dem.GetMissionById (misId);
                var misTarList = new List<IMissionTarget> (de.m_targetList.Count);
                for (int i = 0; i < de.m_targetList.Count; i++) {
                    var type = de.m_targetList[i].Item1;
                    var tarId = de.m_targetList[i].Item2;
                    var progress = (misTarProgressList == null || misTarProgressList.Count <= i) ? 0 : misTarProgressList[i];
                    var tar = m_misTarPoolDict[type].GetInstanceObj () as IMissionTarget;
                    m_misTarReseterDict[type].Reset (tar, tarId, progress, m_dem);
                    misTarList.Add (tar);
                }
                res.Reset (de, misTarList);
                return res;
            }
            public void RecycleInstance (E_Mission mis) {
                for (int i = 0; i < mis.m_tarList.Count; i++)
                    m_misTarPoolDict[mis.m_tarList[i].m_Type].RecycleInstance (mis.m_tarList[i]);
                m_misPool.RecycleInstance (mis);
            }
        }
        public static EM_Mission s_instance;
        private MissionFactory m_fact;
        private DEM_Mission m_dem;
        private IDDS_Mission m_misDds;
        private IDDS_Title m_titleDds;
        /// <summary>已接任务</summary>
        private Dictionary<int, Dictionary<short, E_Mission>> m_acceptedMissionDict = new Dictionary<int, Dictionary<short, E_Mission>> ();
        /// <summary>可接任务</summary>
        private Dictionary<int, HashSet<short>> m_acceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        /// <summary>已解锁但不可接</summary>
        private Dictionary<int, HashSet<short>> m_unacceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        /// <summary>称号任务</summary>
        private Dictionary<int, Dictionary<short, E_Mission>> m_titleMissionDict = new Dictionary<int, Dictionary<short, E_Mission>> ();
        /// <summary>佩戴的称号, 若角色无佩戴, 该字典不会存储</summary>
        private Dictionary<int, short> m_attachedTitleDict = new Dictionary<int, short> ();
        public EM_Mission (DEM_Mission dem, IDDS_Mission misDds, IDDS_Title titleDds) { m_dem = dem; m_fact = new MissionFactory (dem); m_misDds = misDds; m_titleDds = titleDds; }
        public void InitCharacter (int netId, int charId, out List<E_Mission> resAcceptedMisIdList, out List<short> resAcceptableMisIdList, out List<short> resUnacceptableMisIdList, out List<E_Mission> resTitleMissionList, out short resAttachedTitleMisId, out IReadOnlyList < (ActorUnitConcreteAttributeType, int) > resTitleAttr) {
            Dictionary<short, E_Mission> oriAcceptedMisDict;
            HashSet<short> oriAcceptableMisSet;
            HashSet<short> oriUnacceptableMisSet;
            Dictionary<short, E_Mission> oriTitleMisDict;
            // 若角色已经加载
            if (m_acceptedMissionDict.TryGetValue (netId, out oriAcceptedMisDict) && m_acceptableMissionDict.TryGetValue (netId, out oriAcceptableMisSet) && m_unacceptableMissionDict.TryGetValue (netId, out oriUnacceptableMisSet) && m_titleMissionDict.TryGetValue (netId, out oriTitleMisDict)) {
                resAcceptedMisIdList = CollectionUtils.GetDictValueList (oriAcceptedMisDict);
                resAcceptableMisIdList = CollectionUtils.GetSetList (oriAcceptableMisSet);
                resUnacceptableMisIdList = CollectionUtils.GetSetList (oriUnacceptableMisSet);
                resTitleMissionList = CollectionUtils.GetDictValueList (oriTitleMisDict);
                if (!m_attachedTitleDict.TryGetValue (netId, out resAttachedTitleMisId))
                    resAttachedTitleMisId = -1;
                var oriTitleDe = m_dem.GetTitleById (resAttachedTitleMisId);
                if (oriTitleDe != null)
                    resTitleAttr = oriTitleDe.m_attr;
                else resTitleAttr = null;
                return;
            }

            // 读取普通任务ddo
            var ddoList = m_misDds.GetMissionListByCharacterId (charId);

            // 读取已接任务
            Dictionary<short, E_Mission> acceptedMissionDict = new Dictionary<short, E_Mission> (ddoList.Count);
            for (int i = 0; i < ddoList.Count; i++) {
                if (ddoList[i].m_status != MissionStatus.ACCEPTED) continue;
                E_Mission mis = m_fact.GetInstance (ddoList[i].m_missionId, ddoList[i].m_missionTargetProgressList);
                acceptedMissionDict[ddoList[i].m_missionId] = mis;
            }
            m_acceptedMissionDict.Add (netId, acceptedMissionDict);

            // 获取可接与不可接任务
            var acceptableMissionSet = new HashSet<short> ();
            var unacceptableMissionSet = new HashSet<short> ();
            m_acceptableMissionDict.Add (netId, acceptableMissionSet);
            m_unacceptableMissionDict.Add (netId, unacceptableMissionSet);
            for (int i = 0; i < ddoList.Count; i++) {
                if (ddoList[i].m_status == MissionStatus.ACCEPTED) continue;
                if (ddoList[i].m_status == MissionStatus.ACCEPTABLE)
                    acceptableMissionSet.Add (ddoList[i].m_missionId);
                else
                    unacceptableMissionSet.Add (ddoList[i].m_missionId);
            }

            // 读取称号任务
            var titleDdoList = m_misDds.GetTitleMissionListByCharacterId (charId);
            Dictionary<short, E_Mission> titleMisDict = new Dictionary<short, E_Mission> (titleDdoList.Count);
            for (int i = 0; i < titleDdoList.Count; i++) {
                E_Mission mis = m_fact.GetInstance (titleDdoList[i].m_missionId, ddoList[i].m_missionTargetProgressList);
                titleMisDict[titleDdoList[i].m_missionId] = mis;
            }
            m_titleMissionDict.Add (netId, titleMisDict);

            // 读取角色已佩戴的称号
            var attachedTitleMisId = m_titleDds.GetAttachedTitle (charId);
            DE_Title titleDe = null;
            if (attachedTitleMisId != -1) {
                titleDe = m_dem.GetTitleById (attachedTitleMisId);
                m_attachedTitleDict[netId] = attachedTitleMisId;
            }

            // 返回
            resAcceptedMisIdList = CollectionUtils.GetDictValueList (acceptedMissionDict);
            resAcceptableMisIdList = CollectionUtils.GetSetList (acceptableMissionSet);
            resUnacceptableMisIdList = CollectionUtils.GetSetList (unacceptableMissionSet);
            resTitleMissionList = CollectionUtils.GetDictValueList (titleMisDict);
            resAttachedTitleMisId = attachedTitleMisId;
            resTitleAttr = titleDe == null ? null : titleDe.m_attr;
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
                m_fact.RecycleInstance (mEn.Current);
        }
        public Dictionary<short, E_Mission> GetCharAllMisDict (int netId) {
            Dictionary<short, E_Mission> res;
            m_acceptedMissionDict.TryGetValue (netId, out res);
            return res;
        }
        public Dictionary<short, E_Mission> GetCharAllTitleMisDict (int netId) {
            Dictionary<short, E_Mission> res;
            m_titleMissionDict.TryGetValue (netId, out res);
            return res;
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
        public short GetAttachedTitleMisId (int netId) {
            short res;
            if (!m_attachedTitleDict.TryGetValue (netId, out res))
                res = -1;
            return res;
        }
        public E_Mission AcceptMission (int netId, int charId, short misId) {
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
            E_Mission mis = m_fact.GetInstance (misId, null);
            // 处理可接 已接
            acceptableSet.Remove (misId);
            acceptedDict[misId] = mis;
            // 持久化
            m_misDds.UpdateMission (mis.GetDdo (charId, MissionStatus.ACCEPTED));
            return mis;
        }
        /// <summary>
        /// 交付一个任务  
        /// 返回解锁任务的修改信息
        /// </summary>
        public void DeliveryMission (int netId, int charId, E_Mission mis, OccupationType ocp, short lv, out List<short> resNewAcceptableMis, out List<short> resNewUnacceptableMis) {
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
            m_fact.RecycleInstance (mis);
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
            // 持久化
            m_misDds.DeleteMission (mis.m_MissionId, charId);
            for (int i = 0; i < resNewAcceptableMis.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (resNewAcceptableMis[i], charId, MissionStatus.ACCEPTABLE, new List<int> ()));
            for (int i = 0; i < resNewUnacceptableMis.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (resNewUnacceptableMis[i], charId, MissionStatus.UNLOCKED_BUT_UNACCEPTABLE, new List<int> ()));
        }
        public void CancelMission (int netId, int charId, E_Mission mis) {
            HashSet<short> acceptableSet = null;
            if (!m_acceptableMissionDict.TryGetValue (netId, out acceptableSet))
                return;
            Dictionary<short, E_Mission> acceptedDict = null;
            if (!m_acceptedMissionDict.TryGetValue (netId, out acceptedDict))
                return;
            // 放弃任务 并 回收实例
            acceptedDict.Remove (mis.m_MissionId);
            m_fact.RecycleInstance (mis);
            acceptableSet.Add (mis.m_MissionId);

            // 持久化
            m_misDds.UpdateMission (mis.GetDdo (charId, MissionStatus.ACCEPTABLE));
        }
        public void UpdateTitleMission (int charId, E_Mission mission) {
            m_misDds.UpdateTitleMission (mission.GetDdo (charId, MissionStatus.ACCEPTED));
        }
        public void UpdateMission (int charId, E_Mission mis) {
            m_misDds.UpdateMission (mis.GetDdo (charId, MissionStatus.ACCEPTED));
        }
        /// <summary>
        /// 刷新已解锁任务中的可接任务
        /// </summary>
        public void RefreshUnlockedMission (int netId, short lv, out IReadOnlyList<short> acceptableMissionList) {
            acceptableMissionList = null;
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
            acceptableMissionList = changedList;
        }
        public bool AttachTitle (int netId, short misId, out IReadOnlyList < (ActorUnitConcreteAttributeType, int) > resAttr) {
            int charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            resAttr = null;
            if (charId == -1) return false;
            Dictionary<short, E_Mission> titleMisDict;
            if (!m_titleMissionDict.TryGetValue (netId, out titleMisDict)) return false;
            E_Mission titleMis;
            if (!titleMisDict.TryGetValue (misId, out titleMis)) return false;
            if (!titleMis.m_IsFinished) return false;
            m_attachedTitleDict[netId] = misId;
            m_titleDds.UpdateAttachedTitle (charId, misId);
            var titleDe = m_dem.GetTitleById (misId);
            resAttr = titleDe.m_attr;
            return true;
        }
        public bool DetachTitle (int netId, out IReadOnlyList < (ActorUnitConcreteAttributeType, int) > resAttr) {
            int charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            resAttr = null;
            if (charId == -1) return false;
            short misId;
            if (!m_attachedTitleDict.TryGetValue (netId, out misId)) return false;
            m_attachedTitleDict.Remove (netId);
            m_titleDds.UpdateAttachedTitle (charId, -1);
            var titleDe = m_dem.GetTitleById (misId);
            resAttr = titleDe.m_attr;
            return true;
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