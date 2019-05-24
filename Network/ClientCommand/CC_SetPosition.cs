using System;
using System.Numerics;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_SetPosition : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.SET_POSITION; } }
        public void Execute (NetDataReader reader, int netId) {
            var position = reader.GetVector2 ();
            Messenger.Broadcast<int, Vector2> ("CommandSetPosition", netId, position);
        }
    }
}