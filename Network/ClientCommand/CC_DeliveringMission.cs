using LiteNetLib.Utils;
using System;

namespace MirRemake {
    class CC_DeliveringMission : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端请求提交任务");
            short missionId = reader.GetShort();

            SM_ActorUnit.s_instance.CommandDeliveringMission(netId, missionId);
        }
    }
}