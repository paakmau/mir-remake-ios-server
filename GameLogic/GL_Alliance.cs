using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Alliance : GameLogicBase {
        public static GL_Alliance s_instance;
        public GL_Alliance (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandCreateAlliance (int netId, string name) { }
        public void CommandApplyToJoin (int netId) { }
        public void CommandRefuseToJoin (int netId) { }
        public void CommandApproveToJoin (int netId, int charId) { }
        public void CommandChangeJob (int netId, string tarCharId, AllianceJob job) { }
        public void NotifyInitCharacterAlliance (int netId) { }
    }
}