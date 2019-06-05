using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private IDDS_Mission m_missionDds;
        private Dictionary<int, HashSet<short>> m_acceptableMissionDict = new Dictionary<int, HashSet<short>> ();
        public GL_Mission (INetworkService ns, IDDS_Mission mDds) : base (ns) {
            m_missionDds = mDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            // 读取任务信息
            var ddsList = m_missionDds.GetMissionListByCharacterId (charId);
            // 载入EM
            var mList = EM_Mission.s_instance.InitCharacterMission (netId, charId, ddsList);
            // 得到角色可接的任务
            var acceptableMissionSet = new HashSet<short> ();
            m_acceptableMissionDict.Add (netId, acceptableMissionSet);
            for (int i = 0; i < mList.Count; i++)
                for (int j=0; j<mList[i].m_ChildrenIdList.Count; j++)
                    acceptableMissionSet.Add (mList[i].m_ChildrenIdList[i]);

        }
        public void CommandRemoveCharacter (int netId) {
            EM_Mission.s_instance.RemoveCharacter (netId);
        }
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