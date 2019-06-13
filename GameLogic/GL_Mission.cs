using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private IDDS_Mission m_misDds;
        public GL_Mission (IDDS_Mission mDds, INetworkService ns) : base (ns) {
            m_misDds = mDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandApplyTalkToNpc (int netId, short npcId, short missionId) {
            // TODO: nmsl 没有用到 missionId
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            ListenMissionTarget (charObj, MissionTargetType.TALK_TO_NPC, npcId, 1);
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
            // TODO: 添加监听
            // AddListener (mis, charObj);
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
            // TODO: 移除监听
            // RemoveListener (misObj, charObj);
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
            // TODO: 移除监听
            // RemoveListener (misObj, charObj);
        }
        public void NotifyInitCharacter (E_Character charObj) {
            // 实例化任务
            var ddsList = m_misDds.GetMissionListByCharacterId (charObj.m_characterId);
            List<short> acceptedMis, acceptableMis, unacceptableMis;
            List<E_Mission> acceptedMisObjList;
            EM_Mission.s_instance.InitCharacter (charObj.m_networkId, charObj.m_characterId, charObj.m_Occupation, charObj.m_Level, ddsList, out acceptedMis, out acceptedMisObjList, out acceptableMis, out unacceptableMis);
            // TODO: 添加监听
            // for (int i = 0; i < acceptedMisObjList.Count; i++)
            //     AddListener (acceptedMisObjList[i], charObj);
            // client
            m_networkService.SendServerCommand (SC_InitSelfMission.Instance (charObj.m_networkId, acceptedMis, acceptableMis, unacceptableMis));
        }
        public void NotifyRemoveCharacter (E_Character charObj) {
            // TODO: 移除监听
            // var misDict = EM_Mission.s_instance.GetAllAcceptedMission (charObj.m_networkId);
            // var en = misDict.Values.GetEnumerator ();
            // while (en.MoveNext ())
            //     RemoveListener (en.Current, charObj);
            EM_Mission.s_instance.RemoveCharacter (charObj.m_networkId);
        }
        // TODO: 监听事件 双向依赖很恶心
        public void ListenMissionTarget (E_Character charObj, MissionTargetType tarType, short id, int deltaV) {
            var allMis = EM_Mission.s_instance.GetAllAcceptedMission (charObj.m_networkId);
            if (allMis == null) return;
            var en = allMis.Values.GetEnumerator ();
            while (en.MoveNext ()) {
                bool dirty = false;
                var tarList = en.Current.m_MisTarget;
                for (int i = 0; i < tarList.Count; i++) {
                    if (tarList[i].Item1 != tarType)
                        continue;
                    if (tarList[i].Item2 != id)
                        continue;
                    dirty = true;
                    UpdateMissionProgress (en.Current, i, deltaV, charObj.m_networkId);
                }
                // dds
                if (dirty) {
                    m_misDds.UpdateMission (en.Current.GetDdo (charObj.m_characterId));
                }
            }
        }
        private void UpdateMissionProgress (E_Mission mis, int i, int deltaV, int netId) {
            mis.m_misTargetProgressArr[i].Item1 ++;
            // client
            m_networkService.SendServerCommand (SC_ApplySelfMissionProgress.Instance (
                netId, mis.m_MissionId, (byte)i, mis.m_misTargetProgressArr[i].Item1));
        }
    }
}