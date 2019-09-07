using System;
using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_Bag {
        public virtual ItemPlace m_repositoryPlace { get { return ItemPlace.BAG; } }
        public List<E_Item> m_itemList = new List<E_Item> ();
        public void Reset (E_Item[] itemArr) {
            m_itemList.Clear ();
            foreach (var item in itemArr)
                m_itemList.Add (item);
        }
        public NO_Repository GetNo () {
            var itemNoList = new List<NO_Item> (m_itemList.Count);
            var equipInfoNoList = new List<NO_EquipmentItemInfo> ();
            for (int i = 0; i < m_itemList.Count; i++) {
                itemNoList.Add (m_itemList[i].GetItemNo (m_repositoryPlace, (short) i));
                if (m_itemList[i].m_Type == ItemType.EQUIPMENT)
                    equipInfoNoList.Add (((E_EquipmentItem) m_itemList[i]).GetEquipmentInfoNo ());
            }
            return new NO_Repository (itemNoList, equipInfoNoList);
        }
        public E_Item GetItemByRealId (long realId, out short resPos) {
            for (short i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    resPos = i;
                    return m_itemList[i];
                }
            resPos = -1;
            return null;
        }
        public void SetItem (E_Item item, short pos) {
            if (m_itemList.Count <= pos)
                return;
            m_itemList[pos] = item;
        }
        public short GetEmptySlot (out E_EmptyItem resSlot) {
            for (short i = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) {
                    resSlot = m_itemList[i] as E_EmptyItem;
                    return i;
                }
            }
            resSlot = null;
            return -1;
        }
        /// <summary>
        /// 存储一个Item  
        /// 背包内的因为插入而被修改的物品 (pos, obj) 列表  
        /// </summary>
        /// <return>
        /// 若item占用了一个槽位返回 pos  
        /// 若完全堆叠返回 -1  
        /// 未能完全存入返回 -2  
        /// </return>
        public short AutoPileAndStoreItem (E_Item item, out List < (short, E_Item) > posAndChangedItemList, out short piledNum, out short realStoredNum, out E_EmptyItem resOriSlot) {
            posAndChangedItemList = new List < (short, E_Item) > ();
            piledNum = 0;
            realStoredNum = 0;
            resOriSlot = null;
            // 堆叠
            for (int i = 0; i < m_itemList.Count; i++) {
                var itemInRepo = m_itemList[i];
                if (itemInRepo.m_ItemId == item.m_ItemId && itemInRepo.m_num != itemInRepo.m_MaxNum) {
                    posAndChangedItemList.Add (((short) i, itemInRepo));
                    short added = itemInRepo.AddNum (item.m_num);
                    piledNum += added;
                    realStoredNum = piledNum;
                    item.m_num -= added;
                    if (item.m_num == 0)
                        return -1;
                }
            }
            // 寻找空插槽
            for (short i = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) {
                    resOriSlot = m_itemList[i] as E_EmptyItem;
                    realStoredNum += item.m_num;
                    m_itemList[i] = item;
                    return i;
                }
            }
            return -2;
        }
        /// <summary> 判断能否放入物品, 其中 snum 必须小于该物品的 MaxNum </summary>
        public bool CanPutItem (short itemId, short snum) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_Type == ItemType.EMPTY)
                    return true;
            int num = snum;
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_ItemId == itemId)
                    num -= (m_itemList[i].m_MaxNum - m_itemList[i].m_num);
            return num <= 0;
        }
        /// <summary> 判断能否放入一组物品, 其中每一个物品的 num 必须小于该物品的 MaxNum </summary>
        public bool CanPutItems (IReadOnlyList < (short, short) > itemIdAndNumList) {
            int slotNum = 0;
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_Type == ItemType.EMPTY)
                    slotNum++;
            if (slotNum >= itemIdAndNumList.Count) return true;
            var idNumMxNumArr = new (short, short, short) [m_itemList.Count];
            for (int i = 0; i < m_itemList.Count; i++)
                idNumMxNumArr[i] = (m_itemList[i].m_ItemId, m_itemList[i].m_num, m_itemList[i].m_MaxNum);
            for (int k = 0; k < itemIdAndNumList.Count; k++) {
                short num = itemIdAndNumList[k].Item2;
                for (int i = 0; i < idNumMxNumArr.Length; i++) {
                    if (idNumMxNumArr[i].Item1 != itemIdAndNumList[k].Item1) continue;
                    short toPut = Math.Min ((short) (idNumMxNumArr[i].Item3 - idNumMxNumArr[i].Item2), num);
                    idNumMxNumArr[i].Item2 += toPut;
                    num -= toPut;
                    if (num == 0) break;
                }
                slotNum = (num == 0) ? slotNum : slotNum - 1;
                if (slotNum < 0) return false;
            }
            return true;
        }
    }
    class E_StoreHouse : E_Bag {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.STORE_HOUSE; } }
    }
    class E_EquipmentRegion : E_Bag {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.EQUIPMENT_REGION; } }
        /// <summary>
        /// 返回位置
        /// 若找不到，则为随机一个装备区的 E_EmptyItem
        /// </summary>
        public short GetEquipmentByEquipPosition (EquipmentPosition eqPos, out E_Item resItem) {
            for (int i = 0; i < m_itemList.Count; i++) {
                var eq = m_itemList[i] as E_EquipmentItem;
                if (eq == null) continue;
                if (eq.m_EquipmentPosition == eqPos) {
                    resItem = eq;
                    return (short) i;
                }
            }
            for (int i = 0; i < m_itemList.Count; i++)
                if ((m_itemList[i].m_Type == ItemType.EMPTY)) {
                    resItem = m_itemList[i];
                    return (short) i;
                }
            resItem = null;
            return -3;
        }
    }
}