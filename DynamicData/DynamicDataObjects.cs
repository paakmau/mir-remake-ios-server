using System;
using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    struct DDO_Character {
        public int m_characterId;
        public short m_level;
        public OccupationType m_occupation;
        public int m_experience;
        public ValueTuple<CurrencyType, long>[] m_currencyArr;
        /// <summary>
        /// 已分配的主属性点情况
        /// </summary>
        public ValueTuple<ActorUnitMainAttributeType, short>[] m_distributedMainAttrPointArr;
        public DDO_Character (int charId, short level, OccupationType oc, int exp, ValueTuple<CurrencyType, long>[] currencyArr, ValueTuple<ActorUnitMainAttributeType, short>[] attrPointArr) {
            m_characterId = charId;
            m_level = level;
            m_occupation = oc;
            m_experience = exp;
            m_currencyArr = currencyArr;
            m_distributedMainAttrPointArr = attrPointArr;
        }
    }
    struct DDO_EquipmentInfo {
        public long m_realId;
        public int m_characterId;
        /// <summary>
        /// 强化次数
        /// </summary>
        public byte m_strengthNum;
        /// <summary>
        /// 装备附魔的属性
        /// 可以为null
        /// </summary>
        public (ActorUnitConcreteAttributeType, int) [] m_enchantAttr;
        /// <summary>
        /// 镶嵌的宝石Id列表
        /// </summary>
        public List<short> m_inlaidGemIdList;
        /// <summary>
        /// 目前已经打的孔的数量
        /// </summary>
        [Obsolete ("已经弃用")]
        public short m_holeNum;
        public DDO_EquipmentInfo (long realId, int charId, byte strNum, (ActorUnitConcreteAttributeType, int) [] enchantAttr, List<short> inlaidGemIds) {
            m_realId = realId;
            m_characterId = charId;
            m_strengthNum = strNum;
            m_enchantAttr = enchantAttr;
            m_inlaidGemIdList = inlaidGemIds;
            m_holeNum = 0;
        }
    }
    struct DDO_Item {
        public long m_realId;
        public short m_itemId;
        public int m_characterId;
        public short m_num;
        /// <summary>
        /// 该物品所在的地点 (背包, 仓库, 装备区, 地面)
        /// </summary>
        public ItemPlace m_place;
        /// <summary>
        /// 该物品在地点中的位置 (第几格)
        /// </summary>
        public short m_position;
        public DDO_Item (long realId, short itemId, int charId, short num, ItemPlace place, short pos) {
            m_realId = realId;
            m_itemId = itemId;
            m_characterId = charId;
            m_num = num;
            m_place = place;
            m_position = pos;
        }
    }
    struct DDO_Skill {
        public short m_skillId;
        public int m_characterId;
        public short m_skillLevel;
        public int m_masterly;
        public DDO_Skill (short skillId, int charId, short skillLv, int masterly) {
            m_skillId = skillId;
            m_characterId = charId;
            m_skillLevel = skillLv;
            m_masterly = masterly;
        }
    }
    struct DDO_Mission {
        public short m_missionId;
        public int m_characterId;
        public MissionStatus m_isAccepted;
        public List<int> m_missionTargetProgressList;
        public DDO_Mission (short misId, int charId, MissionStatus isAccepted, List<int> misProgList) {
            m_missionId = misId;
            m_characterId = charId;
            m_isAccepted = isAccepted;
            m_missionTargetProgressList = misProgList;
        }
    }
}