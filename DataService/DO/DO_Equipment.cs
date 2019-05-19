using System.Collections.Generic;

namespace MirRemakeBackend
{
    struct DO_Equipment {
        /// <summary>
        /// 装备部位
        /// </summary>
        public EquipmentPosition m_position;
        /// <summary>
        /// 装备的基础属性
        /// </summary>
        public Dictionary<ActorUnitConcreteAttributeType, int> m_attr;
    }
}
