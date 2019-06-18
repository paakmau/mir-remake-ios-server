using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理任务的接受, 放弃, 完成, 进度
    /// </summary>
    class GL_Mission : GameLogicBase {
        private interface IMissionTargetProgressChecker {
            MissionTargetType m_Type { get; }
            int GetNewProgress (int netId, int misId, (MissionTargetType, short, int) target, int curProgs, E_Log logBase);
        }
        private class MTPC_KillMonster : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.KILL_MONSTER; } }
            public int GetNewProgress (int netId, int misId, (MissionTargetType, short, int) target, int curProgs, E_Log logBase) {
                var log = logBase as E_KillMonsterLog;
                if (log == null) return curProgs;
                if (netId != log.m_killerNetId)
                    return curProgs;
                if (target.Item2 != log.m_monId)
                    return curProgs;
                return Math.Min (target.Item3, curProgs + 1);
            }
        }
        private class MTPC_LevelUpSkill : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.LEVEL_UP_SKILL; } }
            public int GetNewProgress (int netId, int misId, (MissionTargetType, short, int) target, int curProgs, E_Log logBase) {
                var log = logBase as E_LevelUpSkillLog;
                if (log == null) return curProgs;
                if (netId != log.m_netId)
                    return curProgs;
                if (target.Item2 != log.m_skillId)
                    return curProgs;
                return log.m_skillLv;
            }
        }
        private class MTPC_GainItem : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.GAIN_ITEM; } }
            public int GetNewProgress (int netId, int misId, (MissionTargetType, short, int) target, int curProgs, E_Log logBase) {
                var log = logBase as E_GainItemLog;
                if (log == null) return curProgs;
                if (netId != log.m_netId)
                    return curProgs;
                if (target.Item2 != log.m_itemId)
                    return curProgs;
                return curProgs + log.m_deltaNum;
            }
        }
        private class MTPC_TalkToNpc : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.TALK_TO_NPC; } }
            public int GetNewProgress (int netId, int misId, (MissionTargetType, short, int) target, int curProgs, E_Log logBase) {
                var log = logBase as E_TalkToNpcLog;
                if (log == null) return curProgs;
                if (netId != log.m_netId)
                    return curProgs;
                if (misId != log.m_misId)
                    return curProgs;
                if (target.Item2 != log.m_npcId)
                    return curProgs;
                return 1;
            }
        }
        public static GL_Mission s_instance;
        private Dictionary<MissionTargetType, IMissionTargetProgressChecker> m_progressCheckerDict = new Dictionary<MissionTargetType, IMissionTargetProgressChecker> ();
        private IDDS_Mission m_misDds;
        public GL_Mission (IDDS_Mission mDds, INetworkService ns) : base (ns) {
            m_misDds = mDds;
            // 实例化所有 IMissionTargetProgressChecker 接口的实现类
            var iType = typeof (IMissionTargetProgressChecker);
            var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => p.IsClass && iType.IsAssignableFrom (p));
            foreach (var type in implTypes) {
                IMissionTargetProgressChecker checker = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IMissionTargetProgressChecker;
                m_progressCheckerDict.Add (checker.m_Type, checker);
            }
        }
        public override void Tick (float dT) {
            // 根据之前产生的 logs 更新所有角色的任务进度
            var logs = EM_Log.s_instance.GetLogsSecondTick ();
            var charMisEn = EM_Mission.s_instance.GetAllCharMisEn ();
            for (int i = 0; i < logs.Count; i++) {
                var logObj = logs[i];
                while (charMisEn.MoveNext ()) {
                    var charNetId = charMisEn.Current.Key;
                    var charId = EM_Unit.s_instance.GetCharIdByNetworkId (charNetId);
                    if (charId == -1) continue;
                    var misEn = charMisEn.Current.Value.GetEnumerator ();
                    while (misEn.MoveNext ()) {
                        var misId = misEn.Current.Key;
                        var misObj = misEn.Current.Value;
                        var misTars = misObj.m_MisTarget;
                        var misProgs = misObj.m_misTargetProgressArr;
                        bool dirty = false;
                        for (int j = 0; j < misTars.Count; j++) {
                            var checker = m_progressCheckerDict[misTars[j].Item1];
                            var curProgs = misProgs[j].Item1;
                            var newProgs = checker.GetNewProgress (charNetId, misId, misTars[j], curProgs, logObj);
                            // 任务进度更新
                            if (curProgs != newProgs) {
                                // client
                                misProgs[j].Item1 = newProgs;
                                dirty = true;
                                m_networkService.SendServerCommand(SC_ApplySelfMissionProgress.Instance (charNetId, misId, (byte)j, newProgs));
                            }
                        }
                        if (dirty)
                            m_misDds.UpdateMission (misObj.GetDdo (charId));
                    }
                }
            }
        }
        public override void NetworkTick () { }
        public void CommandApplyTalkToNpc (int netId, short npcId, short missionId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            GL_Log.s_instance.NotifyLog (GameLogType.TALK_TO_NPC, netId, npcId, missionId);
        }
        public void CommandApplyAcceptMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            // 实例化
            var mis = EM_Mission.s_instance.AcceptMission (netId, misId);
            if (mis == null) return;
            // 数据与client
            m_misDds.UpdateMission (mis.GetDdo (charObj.m_characterId));
            m_networkService.SendServerCommand (SC_ApplySelfAcceptMission.Instance (netId, misId));
        }
        public void CommandApplyDeliveryMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            if (!misObj.m_IsFinished)
                return;
            // 移除实例
            List<short> acableMis, unaMis;
            EM_Mission.s_instance.DeliveryMission (netId, misObj, charObj.m_Occupation, charObj.m_Level, out acableMis, out unaMis);
            // dds 与 client
            m_misDds.DeleteMission (misId, charObj.m_characterId);
            for (int i = 0; i < acableMis.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (acableMis[i], charObj.m_characterId, false, new List<int> ()));
            for (int i = 0; i < unaMis.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (unaMis[i], charObj.m_characterId, false, new List<int> ()));
            m_networkService.SendServerCommand (SC_ApplySelfDeliverMission.Instance (netId, misId));
            m_networkService.SendServerCommand (SC_ApplySelfMissionUnlock.Instance (netId, acableMis, unaMis));
            // 其他模块
            GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, misObj.m_BonusVirtualCurrency);
            GL_Property.s_instance.NotifyGainItem (charObj, misObj.m_BonusItemIdAndNumList);
            GL_CharacterAttribute.s_instance.NotifyGainExperience (charObj, misObj.m_BonusExperience);
        }
        public void CommandCancelMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            // 移除实例 数据 client
            EM_Mission.s_instance.CancelMission (netId, misObj);
            m_misDds.UpdateMission (misObj.GetDdo (charObj.m_networkId));
            m_networkService.SendServerCommand (SC_ApplySelfCancelMission.Instance (netId, misId));
        }
        public void NotifyInitCharacter (E_Character charObj) {
            // 实例化任务
            var ddsList = m_misDds.GetMissionListByCharacterId (charObj.m_characterId);
            List<short> acceptedMis, acceptableMis, unacceptableMis;
            List<E_Mission> acceptedMisObjList;
            EM_Mission.s_instance.InitCharacter (charObj.m_networkId, charObj.m_characterId, charObj.m_Occupation, charObj.m_Level, ddsList, out acceptedMis, out acceptedMisObjList, out acceptableMis, out unacceptableMis);
            // client
            m_networkService.SendServerCommand (SC_InitSelfMission.Instance (charObj.m_networkId, acceptedMis, acceptableMis, unacceptableMis));
        }
        public void NotifyRemoveCharacter (E_Character charObj) {
            EM_Mission.s_instance.RemoveCharacter (charObj.m_networkId);
        }
        private void UpdateMissionProgress (E_Mission mis, int i, int deltaV, int netId) {
            mis.m_misTargetProgressArr[i].Item1++;
            // client
            m_networkService.SendServerCommand (SC_ApplySelfMissionProgress.Instance (
                netId, mis.m_MissionId, (byte) i, mis.m_misTargetProgressArr[i].Item1));
        }
    }
}