using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_EmptyItem : E_Item {
        public override ItemType m_Type { get { return ItemType.EMPTY; } }
    }
    class E_MaterialItem : E_Item {
        public override ItemType m_Type { get { return ItemType.MATERIAL; } }
    }
    class E_GemItem : E_Item {
        public DE_GemData m_gemDe;
        public override ItemType m_Type { get { return ItemType.GEM; } }
        public void Reset (DE_Item itemDe, DE_GemData gemDe, long realId, short num) {
            base.Reset (itemDe, realId, num);
            m_gemDe = gemDe;
        }
        public void Reset (DE_Item itemDe, DE_GemData gemDe, long realId) {
            base.Reset (itemDe, realId, 1);
            m_gemDe = gemDe;
        }
    }
    class E_ConsumableItem : E_Item {
        public DE_ConsumableData m_consumableDe;
        public override ItemType m_Type { get { return ItemType.CONSUMABLE; } }
        public void Reset (DE_Item itemDe, DE_ConsumableData consDe, long realId, short num) {
            base.Reset (itemDe, realId, num);
            m_consumableDe = consDe;
        }
    }
    class E_EquipmentItem : E_Item {
        public DE_EquipmentData m_equipmentDe;
        public override ItemType m_Type { get { return ItemType.EQUIPMENT; } }
        public const int c_maxStrengthenNum = 10;
        public EquipmentPosition m_EquipmentPosition { get { return m_equipmentDe.m_equipPosition; } }
        public byte m_strengthenNum;
        public List < (ActorUnitConcreteAttributeType, int) > m_enchantAttrList = new List<(ActorUnitConcreteAttributeType, int)> ();
        private List<short> m_inlaidGemIdList = new List<short> ();
        public List<DE_GemData> m_inlaidGemList = new List<DE_GemData> ();
        public void ResetEquipmentInfo (long realId, byte strNum, (ActorUnitConcreteAttributeType, int) [] enchantAttr, List<short> inlaidGemIdList, List<DE_GemData> inlaidGemList) {
            m_strengthenNum = strNum;
            m_enchantAttrList.Clear ();
            m_enchantAttrList.AddRange (enchantAttr);
            m_inlaidGemIdList.Clear ();
            m_inlaidGemIdList.AddRange (inlaidGemIdList);
            m_inlaidGemList.Clear ();
            m_inlaidGemList.AddRange (inlaidGemList);
        }
        public void Reset (DE_Item itemDe, DE_EquipmentData eqDe, long realId) {
            base.Reset (itemDe, realId, 1);
            m_equipmentDe = eqDe;
            m_strengthenNum = 0;
            m_enchantAttrList.Clear ();
            m_inlaidGemIdList.Clear ();
            m_inlaidGemList.Clear ();
        }
        public DDO_EquipmentInfo GetEquipmentInfoDdo (int charId) {
            return new DDO_EquipmentInfo (m_realId, charId, m_strengthenNum, m_enchantAttrList, m_inlaidGemIdList);
        }
        public NO_EquipmentItemInfo GetEquipmentInfoNo () {
            return new NO_EquipmentItemInfo (m_realId, m_strengthenNum, m_enchantAttrList, m_inlaidGemIdList);
        }
        public int CalcStrengthenedAttr (int value) {
            return (int) (value * (1 + m_strengthenNum / c_maxStrengthenNum * m_equipmentDe.m_attrWave));
        }
    }
    abstract class E_Item {
        public long m_realId;
        public DE_Item m_itemDe;
        public short m_num;
        public short m_ItemId { get { return m_itemDe.m_id; } }
        public abstract ItemType m_Type { get; }
        public short m_MaxNum { get { return m_itemDe.m_maxNum; } }
        public long m_Price { get { return m_itemDe.m_price; } }
        public bool m_IsEmpty { get { return m_Type == ItemType.EMPTY; } }
        public void Reset (DE_Item de, long realId, short num) {
            m_itemDe = de;
            m_realId = realId;
            m_num = num;
        }
        /// <summary>
        /// 移除一定的数量  
        /// </summary>
        /// <returns>整格用完返回true</returns>
        public bool RemoveNum (short num) {
            m_num = (short) Math.Max (0, m_num - num);
            return m_num == 0;
        }
        /// <summary>
        /// 加入一定的数量  
        /// 返回成功加入的数量
        /// </summary>
        public short AddNum (short num) {
            short rNum = (short) Math.Min (m_MaxNum - m_num, num);
            m_num += rNum;
            return rNum;
        }
        public DDO_Item GetItemDdo (int charId, ItemPlace place, short pos) {
            return new DDO_Item (m_realId, m_ItemId, charId, m_num, place, pos);
        }
        public NO_Item GetItemNo () {
            return new NO_Item (m_realId, m_ItemId, m_num);
        }
    }
}