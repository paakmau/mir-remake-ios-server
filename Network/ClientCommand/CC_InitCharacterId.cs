using System;
using LiteNetLib.Utils;

namespace MirRemakeBackend {
    class CC_InitCharacterId : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.INIT_CHARACTER_ID; } }
        public void Execute(NetDataReader reader, int netId) {
            int playerId = reader.GetInt();
            SM_ActorUnit.s_instance.CommandInitCharacterId(netId, playerId);
        }
    }
}