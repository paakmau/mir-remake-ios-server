using MirRemakeBackend.DynamicData;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Mission : GameLogicBase {
        public static GL_Mission s_instance;
        private IDDS_Mission m_missionDds;
        public GL_Mission (INetworkService ns, IDDS_Mission mDds) : base (ns) {
            m_missionDds = mDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            var ddsList = m_missionDds.GetMissionListByCharacterId (charId);
            EM_Mission.s_instance.InitCharacterMission (netId, charId, ddsList);
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Mission.s_instance.RemoveCharacter (netId);
        }
        public void CommandApplyTalkToNpc (int netId, short npcId, short missionId) {

        }
    }
}