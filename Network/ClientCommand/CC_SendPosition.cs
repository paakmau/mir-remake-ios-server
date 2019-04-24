using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_SendPosition : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端发送位置");
            var position = reader.GetVector2();
            SM_Character.s_instance.CommandSetPosition(netId, position);
        }
    }
}