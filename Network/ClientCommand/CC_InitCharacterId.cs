using System;
using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    class CC_InitCharacterId : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.INIT_CHARACTER_ID; } }
        public void Execute(NetDataReader reader, int netId) {
            int charId = reader.GetInt();
            GL_Character.s_instance.CommandInitCharacterId (netId, charId);
            GL_Skill.s_instance.CommandInitCharacterId (netId, charId);
            GL_Item.s_instance.CommandInitCharacterId (netId, charId);
            GL_Status.s_instance.CommandInitCharacterId (netId, charId);
            GL_Mission.s_instance.CommandInitCharacterId (netId, charId);
        }
    }
}