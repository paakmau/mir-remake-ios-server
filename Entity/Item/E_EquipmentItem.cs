using System.Collections.Generic;
using UnityEngine;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend {
    class E_EquipmentItem : E_Item {
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_equipmentAttributeDict;
        public float m_wave;
        public short m_strengthenNum;
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_enchantAttr;
        public void Reset (DE_Item itemDe, DE_Equipment equipmentDe, long realId, short itemId, DDO_Item itemDdo, DDO_Equipment equipDdo) {
            base.Reset (itemDe, itemDdo);
            m_strengthenNum = equipDdo.m_strengthNum;
            m_enchantAttr = equipDdo.m_enchantAttr;
        }
    }
}