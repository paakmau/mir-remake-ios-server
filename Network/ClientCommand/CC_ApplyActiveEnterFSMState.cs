
using System;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyActiveEnterFSMState : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端请求主动切换FSM状态");
            var aEState = reader.GetFSMAEState();
            SM_Character.s_instance.CommandApplyActiveEnterFSMState(netId, aEState);
        }
    }
}