using System.Collections.Generic;


namespace MirRemake {
    class E_Equipment : E_Item {
        private int m_realityId;
        public int m_RealityId { get { return m_realityId; } }
        private Dictionary<ActorUnitConcreteAttributeType, int> m_equipmentAttributeDict;
        public Dictionary<ActorUnitConcreteAttributeType, int> m_EquipmentAttributeDict { get { return m_equipmentAttributeDict; } }
        

        public E_Equipment(BuildingEquipmentFortune fortune) {
            // TODO:xjb算法，数据库请求
            m_type = ItemType.EQUIPMENT;
        }
    }
}