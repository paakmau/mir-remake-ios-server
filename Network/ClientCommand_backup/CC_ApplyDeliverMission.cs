using System;
using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    class CC_ApplyDeliverMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_DELIVER_MISSION; } }
        public void Execute(NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandApplyDeliveryMission (netId, missionId);
        }
    }
}