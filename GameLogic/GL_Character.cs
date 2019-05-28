using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色升级, 属性点分配, 经验值获取等
    /// 以及具体属性等
    /// </summary>
    class GL_Character : GameLogicBase {
        public static GL_Character s_instance;
        private IDDS_Character m_characterDds;
        public GL_Character (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
            Messenger.AddListener ("CommandAssignNetworkId", CommandAssignNetworkId);
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
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
            EM_Status.s_instance.InitCharacterStatus (netId);

            // 向客户端发送角色信息 TODO: 分开发
            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_level, newChar.m_experience, null, null, null));
        }
        public void CommandRemoveCharacter (int netId) {
            EM_ActorUnit.s_instance.RemoveCharacterByNetworkId (netId);
        }
    }
}
