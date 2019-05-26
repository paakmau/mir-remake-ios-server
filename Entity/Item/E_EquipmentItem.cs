using System;
using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.Entity {
    class E_EquipmentItem : E_Item {
        public const int c_maxStrengthenNum = 10;
        public DE_Equipment m_equipmentDe;
        public EquipmentPosition m_EquipmentPosition { get { return m_equipmentDe.m_equipPosition; } }
        public short m_strengthenNum;
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_enchantAttr;
        public List<short> m_inlaidGemIdList;
        public short m_holeNum;
        public void Reset (DE_Item itemDe, DE_Equipment equipmentDe, DDO_Item itemDdo, DDO_Equipment equipDdo) {
            base.Reset (itemDe, itemDdo);
            m_equipmentDe = equipmentDe;
            m_strengthenNum = equipDdo.m_strengthNum;
            m_enchantAttr = equipDdo.m_enchantAttr;
            m_inlaidGemIdList = equipDdo.m_inlaidGemIdList;
            m_holeNum = equipDdo.m_holeNum;
        }
        public int CalcStrengthenedAttr (int value) {
            return (int) (value * (1 + m_strengthenNum / c_maxStrengthenNum * m_equipmentDe.m_attrWave));
        }
    }
}