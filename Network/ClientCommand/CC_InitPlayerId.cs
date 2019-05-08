using System;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_InitPlayerId : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端初始化玩家ID");
            int playerId = reader.GetInt();
            SM_ActorUnit.s_instance.CommandSetCharacterPlayerId(netId, playerId);
        }
    }
}