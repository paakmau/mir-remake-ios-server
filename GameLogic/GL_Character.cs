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
        private IDDS_Character m_characterDds;
        public GL_Character (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
            Messenger.AddListener ("CommandAssignNetworkId", CommandAssignNetworkId);
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, E_EquipmentItem> ("NotifyTakeOffEquipment", NotifyTakeOffEquipment);
            Messenger.AddListener<int, E_EquipmentItem> ("NotifyPutOnEquipment", NotifyPutOnEquipment);
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
            EM_Status.s_instance.InitCharacterStatus (netId);

            // 向客户端发送角色信息 TODO: 分开发
            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_level, newChar.m_experience, null, null, null));
        }
        public void CommandRemoveCharacter (int netId) {
            EM_ActorUnit.s_instance.RemoveCharacterByNetworkId (netId);
        }
        public void NotifyTakeOffEquipment (int netId, E_EquipmentItem eqObj) {
            EquipmentToAttr (netId, eqObj, -1);
        }
        public void NotifyPutOnEquipment (int netId, E_EquipmentItem eqObj) {
            EquipmentToAttr (netId, eqObj, 1);
        }
        private void EquipmentToAttr (int netId, E_EquipmentItem eqObj, int k) {
            E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (unit == null) return;
            // 处理基础属性与强化
            var attrList = eqObj.m_equipmentDe.m_attrList;
            for (int i = 0; i < attrList.Count; i++)
                unit.AddConAttr (attrList[i].Item1, k * eqObj.CalcStrengthenedAttr (attrList[i].Item2));
            // 处理附魔
            var enchantAttr = eqObj.m_enchantAttr;
            foreach (var attr in enchantAttr)
                unit.AddConAttr (attr.Item1, k * attr.Item2);
            // 处理镶嵌
            var gemIdList = eqObj.m_inlaidGemIdList;
            for (int i = 0; i < gemIdList.Count; i++) {
                var gemDe = EM_Item.s_instance.GetGemById (gemIdList[i]);
                for (int j = 0; j < gemDe.m_attrList.Count; j++)
                    unit.AddConAttr (gemDe.m_attrList[j].Item1, k * gemDe.m_attrList[j].Item2);
            }
        }
        void NotifyUseConsumable (int netId, E_ConsumableItem conObj) {
            E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (unit == null)
                return;
            Messenger.Broadcast<DE_Effect, E_ActorUnit, E_ActorUnit> ("NotifyApplyEffect", conObj.m_consumableDe.m_itemEffect, unit, unit);
        }
    }
}