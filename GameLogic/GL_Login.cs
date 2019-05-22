using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    // TODO: 整合到GL_Character
    class GL_Login : GameLogicBase {
        private IDDS_Character m_characterDds;
        public GL_Login (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
            Messenger.AddListener ("CommandAssignNetworkId", CommandAssignNetworkId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandAssignNetworkId () {
            int netId = EM_ActorUnit.s_instance.AssignCharacterNetworkId ();
            m_networkService.AssignNetworkId (netId);
        }
        public void CommandInitCharacterId (int netId, int charId) {
            // 创建角色实例
            E_Character newChar = EM_ActorUnit.s_instance.InitCharacter (netId, charId, m_characterDds.GetCharacterById (charId));
            EM_Sight.s_instance.SetUnitVisible (newChar);

            // 通知视野逻辑初始化角色视野
            Messenger.Broadcast<int> ("NotifyInitCharacterSight", netId);
            // 向客户端发送角色信息 TODO: 分开发
            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_level, newChar.m_experience, skillIdArr, skillLvArr, skillMasterlyArr));
        }
        public void CommandRemoveCharacter (int netId) {
            EM_ActorUnit.s_instance.RemoveCharacterByNetworkId (netId);
            // 通知视野管理器管理视野
            Messenger.Broadcast<int> ("", netId);
        }
    }
}