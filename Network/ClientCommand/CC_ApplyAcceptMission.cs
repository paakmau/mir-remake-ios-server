using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyAcceptMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_ACCEPT_MISSION; } }
        public void Execute(NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            SM_ActorUnit.s_instance.CommandAcceptMission (netId, missionId);
        }
    }
}