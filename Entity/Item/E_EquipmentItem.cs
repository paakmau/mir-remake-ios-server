using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_EquipmentItem : E_Item {
        public const int c_maxStrengthenNum = 10;
        public DE_EquipmentData m_equipmentDe;
        public EquipmentPosition m_EquipmentPosition { get { return m_equipmentDe.m_equipPosition; } }
        public byte m_strengthenNum;
        public (ActorUnitConcreteAttributeType, int) [] m_enchantAttr;
        public List<short> m_inlaidGemIdList;
        public void Reset (DE_Item itemDe, DE_EquipmentData eqDe, DDO_Item itemDdo, DDO_EquipmentInfo equipDdo) {
            base.Reset (itemDe, itemDdo);
            m_equipmentDe = eqDe;
            m_strengthenNum = equipDdo.m_strengthNum;
            m_enchantAttr = equipDdo.m_enchantAttr;
            m_inlaidGemIdList = equipDdo.m_inlaidGemIdList;
        }
        public void Reset (DE_Item itemDe, DE_EquipmentData eqDe, long realId) {
            base.Reset (itemDe, realId, 1);
            m_equipmentDe = eqDe;
            m_strengthenNum = 0;
            m_enchantAttr = new (ActorUnitConcreteAttributeType, int) [0];
            m_inlaidGemIdList = new List<short> ();
        }
        public DDO_EquipmentInfo GetEquipmentInfoDdo (int charId) {
            return new DDO_EquipmentInfo (m_realId, charId, m_strengthenNum, m_enchantAttr, m_inlaidGemIdList);
        }
        public NO_EquipmentItemInfo GetEquipmentInfoNo () {
            return new NO_EquipmentItemInfo (m_realId, m_strengthenNum, m_enchantAttr, m_inlaidGemIdList);
        }
        public int CalcStrengthenedAttr (int value) {
            return (int) (value * (1 + m_strengthenNum / c_maxStrengthenNum * m_equipmentDe.m_attrWave));
        }
    }
}