using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_SendPosition : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            var position = reader.GetVector2();
            NetworkEntityManager.s_instance.CommandSetPosition(netId, position);
        }
    }
}