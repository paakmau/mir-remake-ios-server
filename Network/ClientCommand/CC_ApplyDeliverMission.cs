using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_ApplyDeliverMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_DELIVER_MISSION; } }
        public void Execute(NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            SM_ActorUnit.s_instance.CommandFinishMission (netId, missionId);
        }
    }
}