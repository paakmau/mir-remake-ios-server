using System;
using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    class CC_ApplyTalkToMissionNpc : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_TALK_TO_MISSION_NPC; } }
        public void Execute(NetDataReader reader, int netId) {
            short npcId = reader.GetShort ();
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandApplyTalkToNpc (netId, npcId, missionId);
        }
    }
}