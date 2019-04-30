using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_AcceptingMission : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端请求接受任务");
            short missionId = reader.GetShort();
            SM_ActorUnit.s_instance.CommandAcceptingMission(netId, missionId);
        }
    }
}