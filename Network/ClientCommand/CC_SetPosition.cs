using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_SetPosition : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            var position = reader.GetVector2();
            SM_ActorUnit.s_instance.CommandSetPosition(netId, position);
        }
    }
}