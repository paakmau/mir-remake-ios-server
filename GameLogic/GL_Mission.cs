using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private IDDS_Mission m_missionDds;
        public GL_Mission (INetworkService ns, IDDS_Mission mDds) : base (ns) {
            m_missionDds = mDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandApplyTalkToNpc (int netId, short npcId, short missionId) {

        }
        public void CommandApplyAcceptMission (int netId, short missionId) {
            var mList = EM_Mission.s_instance.GetRawAcceptedMissionListByNetworkId (netId);
            if (mList == null) return;

        }
        public void CommandApplyDeliveryMission (int netId, short missionId) {

        }
        public void CommandCancelMission (int netId, short missionId) {

        }
    }
}