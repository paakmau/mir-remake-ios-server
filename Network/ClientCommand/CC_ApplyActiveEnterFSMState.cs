

using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyActiveEnterFSMState : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            var aEState = reader.GetFSMAEState();
            NetworkEntityManager.s_instance.CommandApplyActiveEnterFSMState(netId, aEState);
        }
    }
}