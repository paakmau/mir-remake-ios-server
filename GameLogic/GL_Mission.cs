using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private IDDS_Mission m_misDds;
        private Dictionary < E_Mission, List < (MissionTargetType, Callback<short, int>) >> m_missionCallbackDict;
        public GL_Mission (IDDS_Mission mDds, INetworkService ns) : base (ns) {
            m_misDds = mDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandApplyTalkToNpc (int netId, short npcId, short missionId) {
            // TODO:
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
            // 添加监听
            AddListener (mis);
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
            // 移除监听
            RemoveListener (misObj);
            // 其他模块
            GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, misObj.m_BonusVirtualCurrency);
            GL_Property.s_instance.NotifyGainItem (charObj, misObj.m_BonusItemIdAndNumList);
            GL_CharacterLevel.s_instance.NotifyGainExperience (charObj, misObj.m_BonusExperience);
        }
        public void CommandCancelMission (int netId, short misId) {
            var charId = EM_Unit.s_instance.GetCharIdByNetworkId (netId);
            if (charId == -1) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            // 移除实例 数据 client
            EM_Mission.s_instance.CancelMission (netId, misObj);
            m_misDds.UpdateMission (misObj.GetDdo (charId));
            m_networkService.SendServerCommand (SC_ApplySelfCancelMission.Instance (netId, misId));
            // 移除监听
            RemoveListener (misObj);
        }
        public void NotifyInitMission (List<E_Mission> misList) {
            for (int i = 0; i < misList.Count; i++)
                AddListener (misList[i]);
        }
        private void AddListener (E_Mission mis) {
            if (!m_missionCallbackDict.TryAdd (mis, new List < (MissionTargetType, Callback<short, int>) > ()))
                return;
            // 对所有目标添加监听
            for (int i = 0; i < mis.m_MisTarget.Count; i++) {
                int index = i;
                Callback<short, int> callback = (id, deltaV) => {
                    if (mis.m_MisTarget[index].Item2 == id)
                        mis.m_misTargetProgressArr[index].Item1 += deltaV;
                };
                Messenger.AddListener<short, int> (mis.m_MisTarget[i].Item1.ToString (), callback);
                m_missionCallbackDict[mis].Add ((mis.m_MisTarget[i].Item1, callback));
            }
        }
        private void RemoveListener (E_Mission mis) {
            List < (MissionTargetType, Callback<short, int>) > callbacks;
            if (!m_missionCallbackDict.TryGetValue (mis, out callbacks))
                return;
            for (int i = 0; i < callbacks.Count; i++)
                Messenger.RemoveListener (callbacks[i].Item1.ToString (), callbacks[i].Item2);
        }
    }
}