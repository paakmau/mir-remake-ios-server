using System.Collections.Generic;

namespace MirRemakeBackend {
    struct DDO_EquipmentInfo {
        /// <summary>
        /// 强化次数
        /// </summary>
        public byte m_strengthNum;
        /// <summary>
        /// 装备附魔的属性
        /// 可以为null
        /// </summary>
        public Dictionary<ActorUnitConcreteAttributeType, int> m_enchantAttr;
        /// <summary>
        /// 镶嵌的宝石Id列表
        /// </summary>
        public List<short> m_inlaidGemIdList;
        /// <summary>
        /// 目前已经打的孔的数量
        /// </summary>
        public short m_inlaidNum;
    }
}