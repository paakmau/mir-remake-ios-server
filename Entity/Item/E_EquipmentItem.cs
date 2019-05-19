using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class E_EquipmentItem : E_Item {
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_equipmentAttributeDict;
        public float m_wave;
        public short m_strengthenNum;
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_enchantAttr;

        public E_EquipmentItem (long realId, short itemId, DO_EquipmentInfo infoDo, DDO_Equipment ddp) : base (realId, itemId, 1) {
            m_equipmentAttributeDict = infoDo.m_equipmentAttributeArr;
            m_strengthenNum = ddp.m_strengthNum;
            m_enchantAttr = ddp.m_enchantAttr;
        }
    }
}