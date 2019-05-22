using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using UnityEngine;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色的行为
    /// 例如技能释放, 移动等
    /// </summary>
    class GL_CharacterAction : GameLogicBase {
        public GL_CharacterAction (INetworkService netService) : base (netService) {
            Messenger.AddListener<int, Vector2> ("CommandSetPosition", CommandSetPosition);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandSetPosition (int netId, Vector2 pos) {
            E_Character charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            charObj.m_position = pos;
        }
    }
}