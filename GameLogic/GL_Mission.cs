using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理任务的接受, 放弃, 完成, 进度
    /// TODO: 紧急优化: 需要Log具有针对性
    /// </summary>
    partial class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private Dictionary<MissionTargetType, IMissionTargetProgressChecker> m_progressCheckerDict = new Dictionary<MissionTargetType, IMissionTargetProgressChecker> ();
        public GL_Mission (INetworkService ns) : base (ns) {
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
            for (int i = 0; i < logs.Count; i++) {
                var logObj = logs[i];
                var netId = logObj.m_netId;
                var charId = EM_Unit.s_instance.GetCharIdByNetworkId (netId);
                if (charId == -1) continue;
                var misDict = EM_Mission.s_instance.GetCharAllMisDict (netId);
                if (misDict == null) continue;
                var misEn = misDict.GetEnumerator ();
                while (misEn.MoveNext ()) {
                    var misId = misEn.Current.Key;
                    var misObj = misEn.Current.Value;
                    var misTars = misObj.m_tarList;
                    bool dirty = false;
                    for (int j = 0; j < misTars.Count; j++) {
                        var checker = m_progressCheckerDict[misTars[j].m_Type];
                        var curProgs = misTars[j].m_Progress;
                        var newProgs = checker.GetNewProgress (misId, misTars[j], curProgs, logObj);
                        // 任务进度更新
                        if (curProgs != newProgs) {
                            // client
                            misTars[j].m_Progress = newProgs;
                            dirty = true;
                            m_networkService.SendServerCommand (SC_ApplySelfMissionProgress.Instance (netId, misId, (byte) j, newProgs));
                        }
                    }
                    if (dirty)
                        EM_Mission.s_instance.UpdateMission (charId, misObj);
                }
            }
        }
        public override void NetworkTick () { }
        public void CommandApplyTalkToNpc (int netId, short misId, short misTarId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            GL_MissionLog.s_instance.NotifyLog (GameLogType.TALK_TO_NPC, netId, misId, misTarId);
        }
        public void CommandApplyAcceptMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            // 实例化
            var mis = EM_Mission.s_instance.AcceptMission (netId, charObj.m_characterId, misId);
            if (mis == null) return;
            // client
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
            EM_Mission.s_instance.DeliveryMission (netId, charObj.m_characterId, misObj, charObj.m_Occupation, charObj.m_Level, out acableMis, out unaMis);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfDeliverMission.Instance (netId, misId));
            m_networkService.SendServerCommand (SC_ApplySelfMissionUnlock.Instance (netId, acableMis, unaMis));
            // 其他模块
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, misObj.m_BonusVirtualCurrency);
            GL_Item.s_instance.NotifyCharacterGainItem (charObj, misObj.m_BonusItemIdAndNumList);
            GL_CharacterAttribute.s_instance.NotifyGainExperience (charObj, misObj.m_BonusExperience);
        }
        public void CommandCancelMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            // 移除实例 数据 client
            EM_Mission.s_instance.CancelMission (netId, charObj.m_characterId, misObj);
            m_networkService.SendServerCommand (SC_ApplySelfCancelMission.Instance (netId, misId));
        }
        public void NotifyInitCharacter (int netId, int charId) {
            // 实例化任务
            List<E_Mission> acceptedMis;
            List<short> acceptableMis, unacceptableMis;
            EM_Mission.s_instance.InitCharacter (netId, charId, out acceptedMis, out acceptableMis, out unacceptableMis);
            List<NO_Mission> acceptedMisNo = new List<NO_Mission> (acceptedMis.Count);
            for (int i = 0; i < acceptedMis.Count; i++)
                acceptedMisNo.Add (acceptedMis[i].GetNo ());
            // client
            m_networkService.SendServerCommand (SC_InitSelfMission.Instance (netId, acceptedMisNo, acceptableMis, unacceptableMis));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Mission.s_instance.RemoveCharacter (netId);
        }
    }
}