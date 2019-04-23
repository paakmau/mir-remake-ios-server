using System;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_SendPlayerId : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端发送玩家ID");
            int playerId = reader.GetInt();
            NetworkEntityManager.s_instance.CommandSetPlayerId(netId, playerId);
        }
    }
}