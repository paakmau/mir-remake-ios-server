using System;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_InitCharacterId : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.INIT_CHARACTER_ID; } }
        public void Execute(NetDataReader reader, int netId) {
            int charId = reader.GetInt();
            Messenger.Broadcast<int, int> ("CommandInitCharacterId", netId, charId);
        }
    }
}