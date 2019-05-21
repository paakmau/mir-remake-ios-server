using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_ApplyCancelMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CANCEL_MISSION; } }
        public void Execute(NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            SM_ActorUnit.s_instance.CommandCancelMission (netId, missionId);
        }
    }
}