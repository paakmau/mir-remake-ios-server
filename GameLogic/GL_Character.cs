using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色升级, 属性点分配, 经验值获取等
    /// 以及具体属性等
    /// </summary>
    class GL_Character : GameLogicBase {
        private IDDS_Character m_characterDds;
        public GL_Character (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
            Messenger.AddListener ("CommandAssignNetworkId", CommandAssignNetworkId);
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, E_EquipmentItem> ("NotifyUseEquipment", NotifyUseEquipment);
            Messenger.AddListener<int, E_ConsumableItem> ("NotifyUseConsumable", NotifyUseConsumable);
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

            // 向客户端发送角色信息 TODO: 分开发
            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_level, newChar.m_experience, skillIdArr, skillLvArr, skillMasterlyArr));
        }
        public void CommandRemoveCharacter (int netId) {
            EM_ActorUnit.s_instance.RemoveCharacterByNetworkId (netId);
        }
        void NotifyUseEquipment (int netId, E_EquipmentItem eqObj) {
            // TODO: 修改人物属性
        }
        void NotifyUseConsumable (int netId, E_ConsumableItem conObj) {
            // TODO: 对人物施加Effect
        }
    }
}