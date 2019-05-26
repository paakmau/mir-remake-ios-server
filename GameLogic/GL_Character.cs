using System.Collections.Generic;
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
        private List<int> t_intList = new List<int> ();
        public GL_Character (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
            Messenger.AddListener ("CommandAssignNetworkId", CommandAssignNetworkId);
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, E_EquipmentItem> ("NotifyTakeOffEquipment", NotifyTakeOffEquipment);
            Messenger.AddListener<int, E_EquipmentItem> ("NotifyPutOnEquipment", NotifyPutOnEquipment);
            Messenger.AddListener<int, E_ConsumableItem> ("NotifyUseConsumable", NotifyUseConsumable);
        }
        public override void Tick (float dT) { // 移除超时的状态
            var statusEn = EM_Status.s_instance.GetStatusEn ();
            while (statusEn.MoveNext ()) {
                int netId = statusEn.Current.Key;
                E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
                t_intList.Clear ();
                for (int i = 0; i < statusEn.Current.Value.Count; i++) {
                    if (MyTimer.CheckTimeUp (statusEn.Current.Value[i].m_endTime)) {
                        t_intList.Add (i);

                    }
                }
                EM_Status.s_instance.RemoveStatus (netId, t_intList);
            }
        }
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
            EM_Status.s_instance.RemoveCharacterStatus (netId);
        }
        void NotifyTakeOffEquipment (int netId, E_EquipmentItem eqObj) {
            E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (unit == null) return;
            // 处理基础属性与强化
            var attrList = eqObj.m_equipmentDe.m_attrList;
            for (int i = 0; i < attrList.Count; i++)
                unit.AddAttr (attrList[i].Item1, eqObj.CalcStrengthenedAttr(attrList[i].Item2));
            // 处理附魔
            var enchantAttr = eqObj.m_enchantAttr;
            foreach (var attr in enchantAttr)
                unit.AddAttr (attr.Item1, attr.Item2);
            // 处理镶嵌
            var gemIdList = eqObj.m_inlaidGemIdList;
            for (int i=0; i<gemIdList.Count; i++) {
                var gemDe = EM_Item.s_instance.GetGemById (gemIdList[i]);
            }
        }
        void NotifyPutOnEquipment (int netId, E_EquipmentItem eqObj) {
            E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (unit == null) return;
            
        }
        void NotifyUseConsumable (int netId, E_ConsumableItem conObj) {
            // TODO: 对人物施加Effect
        }
    }
}