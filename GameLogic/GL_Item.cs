using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    class GL_Item : GameLogicBase {
        public static GL_Item s_instance;
        private IDDS_Item m_itemDds;
        public GL_Item (IDDS_Item itemDds, INetworkService netService) : base (netService) {
            m_itemDds = itemDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyRemoveCharacter (E_Character charObj) {
            EM_Item.s_instance.RemoveCharacter (charObj.m_networkId);
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (bag == null || charObj == null) return;
            short posInBag = -1;
            E_ConsumableItem item = bag.GetItemByRealId (realId, out posInBag) as E_ConsumableItem;
            if (item == null) return;
            GL_UnitBattleAttribute.s_instance.NotifyApplyEffect (item.m_consumableDe.m_itemEffect, -1, charObj, charObj);
            GL_Property.s_instance.NotifyLostItem (charObj, item, 1, posInBag, bag);
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_EquipmentRegion eqRegion = EM_Item.s_instance.GetEquiped (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || eqRegion == null || bag == null) return;
            short posInBag = -1;
            var eq = bag.GetItemByRealId (realId, out posInBag) as E_EquipmentItem;
            if (eq == null) return;
            // 通知角色属性逻辑
            // 该位置原有装备卸下
            var oriEq = eqRegion.GetItemByPosition ((short) eq.m_EquipmentPosition) as E_EquipmentItem;
            if (oriEq != null) {
                GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq, -1));
            }
            // 装备穿上Attr
            GL_CharacterAttribute.s_instance.NotifyConcreteAttributeChange (charObj, EquipmentToAttrList (oriEq, 1));
            // 通知Property逻辑
            GL_Property.s_instance.NotifySwapItemPlace (charObj, eqRegion, (short) eq.m_EquipmentPosition, oriEq, bag, posInBag, eq);
        }
        private List < (ActorUnitConcreteAttributeType, int) > EquipmentToAttrList (E_EquipmentItem eqObj, int k) {
            List < (ActorUnitConcreteAttributeType, int) > res = new List < (ActorUnitConcreteAttributeType, int) > ();
            // 处理强化与基础属性
            var attrList = eqObj.m_equipmentDe.m_attrList;
            for (int i = 0; i < attrList.Count; i++)
                res.Add ((attrList[i].Item1, k * eqObj.CalcStrengthenedAttr (attrList[i].Item2)));
            // 处理附魔
            var enchantAttr = eqObj.m_enchantAttr;
            foreach (var attr in enchantAttr)
                res.Add ((attr.Item1, k * attr.Item2));
            // 处理镶嵌
            var gemIdList = eqObj.m_inlaidGemIdList;
            for (int i = 0; i < gemIdList.Count; i++) {
                var gemDe = EM_Item.s_instance.GetGemById (gemIdList[i]);
                for (int j = 0; j < gemDe.m_attrList.Count; j++)
                    res.Add ((gemDe.m_attrList[j].Item1, k * gemDe.m_attrList[j].Item2));
            }
            return res;
        }
    }
}