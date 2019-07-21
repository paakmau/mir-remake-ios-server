using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_Item {
        public abstract ItemType m_Type { get; }
        public long m_realId;
        private DE_Item m_itemDe;
        public short m_num;
        public short m_ItemId { get { return m_itemDe.m_id; } }
        public ItemQuality m_Quality { get { return m_itemDe.m_quality; } }
        public short m_MaxNum { get { return m_itemDe.m_maxNum; } }
        public long m_BuyPrice { get { return m_itemDe.m_buyPrice; } }
        public long m_SellPrice { get { return m_itemDe.m_sellPrice; } }
        public bool m_IsEmpty { get { return m_Type == ItemType.EMPTY; } }
        protected void Reset (DE_Item de, short num) {
            m_realId = -1;
            m_itemDe = de;
            m_num = num;
        }
        public void ResetRealId (long realId) {
            m_realId = realId;
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
        public NO_Item GetItemNo (ItemPlace ip, short pos) {
            return new NO_Item (ip, pos, m_realId, m_ItemId, m_num);
        }
    }
    class E_EmptyItem : E_Item {
        public override ItemType m_Type { get { return ItemType.EMPTY; } }
        public void Reset (DE_Item de) {
            base.Reset (de, 0);
        }
    }
    class E_MaterialItem : E_Item {
        public override ItemType m_Type { get { return ItemType.MATERIAL; } }
        public new void Reset (DE_Item de, short num) {
            base.Reset (de, num);
        }
    }
    class E_GemItem : E_Item {
        public override ItemType m_Type { get { return ItemType.GEM; } }
        public DE_GemData m_gemDe;
        public IReadOnlyList < (ActorUnitConcreteAttributeType, int) > m_AttrList { get { return m_gemDe.m_attrList; } }
        public void Reset (DE_Item itemDe, DE_GemData gemDe) {
            base.Reset (itemDe, 1);
            m_gemDe = gemDe;
        }
    }
    class E_EnchantmentItem : E_Item {
        public override ItemType m_Type { get { return ItemType.ENCHANTMENT; } }
        public List < (ActorUnitConcreteAttributeType, int) > m_attrList;
        public void Reset (DE_Item itemDe) {
            base.Reset (itemDe, 1);
        }
        public void ResetEnchantmentData (IReadOnlyList < (ActorUnitConcreteAttributeType, int) > attrList) {
            m_attrList.Clear ();
            for (int i = 0; i < attrList.Count; i++)
                m_attrList.Add (attrList[i]);
        }
        public DDO_EnchantmentInfo GetEnchantmentDdoInfo (int charId) {
            return new DDO_EnchantmentInfo (m_realId, charId, m_attrList);
        }
        public NO_EnchantmentItemInfo GetEnchantmentNoInfo () {
            return new NO_EnchantmentItemInfo (m_realId, m_attrList);
        }
    }
    class E_ConsumableItem : E_Item {
        public override ItemType m_Type { get { return ItemType.CONSUMABLE; } }
        public DE_ConsumableData m_consumableDe;
        public void Reset (DE_Item itemDe, DE_ConsumableData consDe, short num) {
            base.Reset (itemDe, num);
            m_consumableDe = consDe;
        }
    }
    class E_EquipmentItem : E_Item {
        public override ItemType m_Type { get { return ItemType.EQUIPMENT; } }
        private DE_EquipmentData m_eqDe;
        public const int c_maxStrengthenNum = 10;
        public EquipmentPosition m_EquipmentPosition { get { return m_eqDe.m_equipPosition; } }
        public short m_LevelInNeed { get { return m_eqDe.m_equipLevelInNeed; } }
        public byte m_strengthenNum;
        /// <summary> 装备原始属性 </summary>
        public IReadOnlyList < (ActorUnitConcreteAttributeType, int) > m_RawAttrList { get { return m_eqDe.m_attrList; } }
        public List < (ActorUnitConcreteAttributeType, int) > m_enchantAttrList = new List < (ActorUnitConcreteAttributeType, int) > ();
        private List<short> m_inlaidGemIdList = new List<short> ();
        /// <summary> 若为 null, 则为插槽 </summary>
        private List<DE_GemData> m_inlaidGemList = new List<DE_GemData> ();
        public IReadOnlyList<DE_GemData> m_InlaidGemList { get { return m_inlaidGemList; } }
        public void Reset (DE_Item itemDe, DE_EquipmentData eqDe) {
            base.Reset (itemDe, 1);
            m_eqDe = eqDe;
            m_strengthenNum = 0;
            m_enchantAttrList.Clear ();
            m_inlaidGemIdList.Clear ();
            m_inlaidGemList.Clear ();
        }
        public void ResetEquipmentData (byte strNum, (ActorUnitConcreteAttributeType, int) [] enchantAttr, List<short> inlaidGemIdList, List<DE_GemData> inlaidGemList) {
            m_strengthenNum = strNum;
            m_enchantAttrList.Clear ();
            m_enchantAttrList.AddRange (enchantAttr);
            m_inlaidGemIdList.Clear ();
            m_inlaidGemIdList.AddRange (inlaidGemIdList);
            m_inlaidGemList.Clear ();
            m_inlaidGemList.AddRange (inlaidGemList);
        }
        public DDO_EquipmentInfo GetEquipmentInfoDdo (int charId) {
            return new DDO_EquipmentInfo (m_realId, charId, m_strengthenNum, m_enchantAttrList, m_inlaidGemIdList);
        }
        public NO_EquipmentItemInfo GetEquipmentInfoNo () {
            return new NO_EquipmentItemInfo (m_realId, m_strengthenNum, m_enchantAttrList, m_inlaidGemIdList);
        }
        public int CalcStrengthenedAttr (int value) {
            return (int) (value * (1 + m_strengthenNum / c_maxStrengthenNum * m_eqDe.m_attrWave));
        }
        public void InlayGem (int pos, short gemId, DE_GemData gem) {
            if (m_inlaidGemIdList.Count <= pos)
                return;
            m_inlaidGemIdList[pos] = gemId;
            m_inlaidGemList[pos] = gem;
        }
        public void MakeHole () {
            m_inlaidGemIdList.Add (-1);
            m_inlaidGemList.Add (null);
        }
    }
    class E_GroundItem {
        public long m_groundItemId;
        public MyTimer.Time m_disappearTime;
        public E_Item m_item;
        public int m_charId;
        public Vector2 m_position;
        public void Reset (long groundItemId, MyTimer.Time disappearTime, E_Item item, int charId, Vector2 pos) {
            m_groundItemId = groundItemId;
            m_disappearTime = disappearTime;
            m_item = item;
            m_charId = charId;
            m_position = pos;
        }
        public NO_GroundItem GetNo () {
            return new NO_GroundItem (m_groundItemId, m_item.m_ItemId, m_item.m_num, m_charId, m_position);
        }
    }
    class E_MarketItem {
        public E_Item m_item;
        public long m_RealId { get { return m_item.m_realId; } }
        public short m_ItemNum { get { return m_item.m_num; } }
        public short m_onSaleNum;
        public long m_virtualCyPrice;
        public long m_chargeCyPrice;
        public short m_bagPos;
        public void Reset (E_Item item, short onSaleNum, long virtualCyPrice, long chargeCyPrice, short bagPos) {
            m_item = item;
            m_onSaleNum = onSaleNum;
            m_virtualCyPrice = virtualCyPrice;
            m_chargeCyPrice = chargeCyPrice;
            m_bagPos = bagPos;
        }
        public NO_MarketItem GetNo () {
            return new NO_MarketItem (m_item.m_realId, m_onSaleNum, m_virtualCyPrice, m_chargeCyPrice);
        }
    }
    class E_Market {
        public List<E_MarketItem> m_itemList;
        public void Reset (List<E_MarketItem> itemList) {
            m_itemList = itemList;
        }
        public E_MarketItem GetMarketItemByRealId (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_RealId == realId)
                    return m_itemList[i];
            return null;
        }
        public void Remove (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_RealId == realId) {
                    m_itemList.RemoveAt (i);
                    break;
                }
        }
    }
}