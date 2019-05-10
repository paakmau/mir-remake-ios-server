using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyFinishMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_FINISH_MISSION; } }
        public void Execute(NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            SM_ActorUnit.s_instance.CommandFinishMission (netId, missionId);
        }
    }
}