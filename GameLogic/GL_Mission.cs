using System.Collections.Generic;
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

        }
        public void CommandApplyAcceptMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            // 实例化
            var mis = EM_Mission.s_instance.AcceptMission (netId, misId);
            if (mis == null) return;
            // 数据与client
            m_misDds.InsertMission (mis.GetDdo (charObj.m_characterId));
            m_networkService.SendServerCommand (SC_ApplySelfAcceptMission.Instance (new List<int> (netId), misId));
            // TODO: 处理任务条件监听
        }
        public void CommandApplyDeliveryMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            // 移除实例
            if (misObj.m_IsFinished)
                EM_Mission.s_instance.DeliveryMission (netId, misObj, charObj.m_occupation, charObj.m_level);
            // 数据与client
            m_misDds.DeleteMission (misId, charObj.m_characterId);
            m_networkService.SendServerCommand (SC_ApplySelfDeliverMission.Instance (new List<int> { netId }, misId));
            // TODO: 移除监听
            // 其他模块
            GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, misObj.m_BonusVirtualCurrency);
            GL_Property.s_instance.NotifyGainItem (charObj, misObj.m_BonusItemIdAndNumList);
            GL_CharacterLevel.s_instance.NotifyGainExperience (charObj, misObj.m_BonusExperience);
        }
        public void CommandCancelMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            // 移除实例 数据 client
            EM_Mission.s_instance.CancelMission (netId, misObj);
            m_misDds.DeleteMission (misId, charObj.m_characterId);
            m_networkService.SendServerCommand (SC_ApplySelfCancelMission.Instance (new List<int> { netId }, misId));
            // TODO: 移除监听
        }
    }
}