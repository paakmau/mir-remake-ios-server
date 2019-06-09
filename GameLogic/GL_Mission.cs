using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private IDDS_Mission m_misDds;
        public GL_Mission (INetworkService ns, IDDS_Mission mDds) : base (ns) {
            m_misDds = mDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandApplyTalkToNpc (int netId, short npcId, short missionId) {

        }
        public void CommandApplyAcceptMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            var mis = EM_Mission.s_instance.AcceptMission (netId, misId);
            if (charObj == null || mis == null) return;
            m_misDds.InsertMission (mis.GetDdo (charObj.m_characterId), charObj.m_characterId);
            m_networkService.SendServerCommand (SC_ApplySelfAcceptMission.Instance (new List<int> (netId), misId));
        }
        public void CommandApplyDeliveryMission (int netId, short misId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var misObj = EM_Mission.s_instance.GetAcceptedMission (netId, misId);
            if (misObj == null) return;
            if (misObj.m_IsFinished)
                EM_Mission.s_instance.DeliveryMission (netId, misObj, charObj.m_occupation, charObj.m_level);
            // m_mis
            GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, misObj.m_BonusVirtualCurrency);
            GL_Property.s_instance.NotifyGainItem (charObj, misObj.m_BonusItemIdAndNumList);
            GL_CharacterLevel.s_instance.NotifyGainExperience (charObj, misObj.m_BonusExperience);
        }
        public void CommandCancelMission (int netId, short missionId) {

        }
    }
}