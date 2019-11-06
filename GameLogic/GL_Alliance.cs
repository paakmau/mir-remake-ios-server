using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Alliance : GameLogicBase {
        public static GL_Alliance s_instance;
        private Dictionary<int, AllianceJob> m_jobDict;
        public GL_Alliance (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandCreateAlliance (int netId, string name) { }
        public void CommandDissolveAlliance (int netId, int allianceId) { }
        public void CommandTransferAlliance (int netId, int allianceId, int tarCharId) { }
        public void CommandApplyToJoin (int netId, int allianceId) { }
        public void CommandRefuseToJoin (int netId, int applyId) { }
        public void CommandApproveToJoin (int netId, int applyId) { }
        public void CommandChangeJob (int netId, int tarCharId, AllianceJob job) { }
        public void NotifyInitCharacterAlliance (int netId) { }
    }
}