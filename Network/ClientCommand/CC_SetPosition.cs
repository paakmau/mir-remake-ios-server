using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_SetPosition : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.SET_POSITION; } }
        public void Execute(NetDataReader reader, int netId) {
            var position = reader.GetVector2();
            SM_ActorUnit.s_instance.CommandSetPosition(netId, position);
        }
    }
}