using System;
using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    class CC_ApplyCancelMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CANCEL_MISSION; } }
        public void Execute(NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandCancelMission (netId, missionId);
        }
    }
}