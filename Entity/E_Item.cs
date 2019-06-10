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
        public new void Reset (DE_Item de, DDO_Item ddo) {
            base.Reset (de, ddo);
        }
        public new void Reset (DE_Item de, long realId, short num) {
            base.Reset (de, realId, num);
        }
    }
    class E_GemItem : E_Item {
        public DE_GemData m_gemDe;
        public void Reset (DE_Item itemDe, DE_GemData gemDe, DDO_Item ddo) {
            base.Reset (itemDe, ddo);
            m_gemDe = gemDe;
        }
        public void Reset (DE_Item itemDe, DE_GemData gemDe, long realId, short num) {
            base.Reset (itemDe, realId, num);
            m_gemDe = gemDe;
        }
    }
    class E_ConsumableItem : E_Item {
        public override ItemType m_Type { get { return ItemType.CONSUMABLE; } }
        public DE_ConsumableData m_consumableDe;
        public void Reset (DE_Item itemDe, DE_ConsumableData consDe, DDO_Item itemDdo) {
            base.Reset (itemDe, itemDdo);
            m_consumableDe = consDe;
        }
        public void Reset (DE_Item itemDe, DE_ConsumableData consDe, long realId, short num) {
            base.Reset (itemDe, realId, num);
            m_consumableDe = consDe;
        }
    }
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
    abstract class E_Item {
        public static E_Item s_emptyItem = new E_EmptyItem ();
        public long m_realId;
        public short m_itemId;
        public DE_Item m_itemDe;
        public virtual ItemType m_Type { get; }
        public short m_num;
        public bool m_IsEmpty { get { return m_Type == ItemType.EMPTY; } }
        public short m_MaxNum { get { return m_itemDe.m_maxNum; } }
        protected void Reset (DE_Item de, long realId, short num) {
            m_itemDe = de;
            m_itemId = de.m_id;
            m_realId = realId;
            m_num = num;
        }
        protected void Reset (DE_Item de, DDO_Item ddo) {
            Reset (de, ddo.m_realId, ddo.m_num);
        }
        /// <summary>
        /// 移除一定的数量  
        /// </summary>
        /// <returns>整格用完返回true</returns>
        public bool RemoveNum (short num) {
            m_num = (short)Math.Max (0, m_num - num);
            return m_num == 0;
        }
        /// <summary>
        /// 加入一定的数量  
        /// 返回成功加入的数量
        /// </summary>
        public short AddNum (short num) {
            short rNum = (short)Math.Min (m_MaxNum - m_num, num);
            m_num += rNum;
            return rNum;
        }
        public DDO_Item GetItemDdo (int charId, ItemPlace place, short pos) {
            return new DDO_Item (m_realId, m_itemId, charId, m_num, place, pos);
        }
        public NO_Item GetItemNo () {
            return new NO_Item (m_realId, m_itemId, m_num);
        }
    }
}