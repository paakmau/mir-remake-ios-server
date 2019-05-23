using System;
using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    struct DDO_Character {
        public int m_characterId;
        public short m_level;
        public OccupationType m_occupation;
        public int m_experience;
    }
    struct DDO_Equipment {
        public long m_realId;
        /// <summary>
        /// 强化次数
        /// </summary>
        public byte m_strengthNum;
        /// <summary>
        /// 装备附魔的属性
        /// 可以为null
        /// </summary>
        public ValueTuple<ActorUnitConcreteAttributeType, int>[] m_enchantAttr;
        /// <summary>
        /// 镶嵌的宝石Id列表
        /// </summary>
        public List<short> m_inlaidGemIdList;
        /// <summary>
        /// 目前已经打的孔的数量
        /// </summary>
        public short m_holeNum;
    }
    struct DDO_Item {
        public long m_realId;
        public short m_itemId;
        public short m_num;
    }
    struct DDO_Skill {
        public short m_skillId;
        public short m_skillLevel;
        public int m_masterly;
    }
}