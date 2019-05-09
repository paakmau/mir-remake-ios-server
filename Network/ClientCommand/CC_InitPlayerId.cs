using System;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_InitPlayerId : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.INIT_PLAYER_ID; } }
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端初始化玩家ID");
            int playerId = reader.GetInt();
            SM_ActorUnit.s_instance.CommandInitCharacterPlayerId(netId, playerId);
        }
    }
}