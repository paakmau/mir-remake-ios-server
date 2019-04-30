using System;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_CancelMission : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端请求取消任务");
            short missionId = reader.GetShort();
            SM_ActorUnit.s_instance.CommandCancelMission(netId, missionId);
        }
    }
}