using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    abstract class E_RepositoryBase {
        public abstract ItemPlace m_repositoryPlace { get; }
        public abstract List<E_Item> m_ItemList { get; }
        public abstract void Reset (E_Item[] itemArr);
        public abstract void SetItem (E_Item item, short pos);
        public abstract NO_Repository GetNo ();
        public abstract E_Item GetItemByRealId (long realId);
        public abstract E_Item GetItemByPosition (short pos);
        /// <summary>
        /// 从背包移除整格物品  
        /// 成功返回true
        /// </summary>
        public abstract E_Item RemoveItemByRealId (long realId, E_EmptyItem empty);
    }
    class E_Bag : E_RepositoryBase {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.BAG; } }
        protected List<E_Item> m_itemList = new List<E_Item> ();
        public override List<E_Item> m_ItemList { get { return m_itemList; } }
        public override void Reset (E_Item[] itemArr) {
            m_itemList.Clear ();
            foreach (var item in itemArr)
                m_itemList.Add (item);
        }
        public override NO_Repository GetNo () {
            var itemNoList = new List<NO_Item> (m_itemList.Count);
            var equipInfoNoList = new List<NO_EquipmentItemInfo> ();
            for (int i = 0; i < m_itemList.Count; i++) {
                itemNoList.Add (m_itemList[i].GetItemNo (m_repositoryPlace, (short) i));
                if (m_itemList[i].m_Type == ItemType.EQUIPMENT)
                    equipInfoNoList.Add (((E_EquipmentItem) m_itemList[i]).GetEquipmentInfoNo ());
            }
            return new NO_Repository (itemNoList, equipInfoNoList);
        }
        public override E_Item GetItemByRealId (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId)
                    return m_itemList[i];
            return null;
        }
        public override E_Item GetItemByPosition (short pos) {
            if (m_itemList.Count <= pos)
                return null;
            return m_itemList[pos];
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
        public override E_Item RemoveItemByRealId (long realId, E_EmptyItem empty) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    var res = m_itemList[i];
                    m_itemList[i] = empty;
                    return res;
                }
            return null;
        }
        public override void SetItem (E_Item item, short pos) {
            if (m_itemList.Count <= pos)
                return;
            m_itemList[pos] = item;
        }
        public bool RemoveItemByPosition (int pos, E_EmptyItem empty) {
            if (m_itemList.Count <= pos)
                return false;
            if (m_itemList[pos].m_IsEmpty)
                return false;
            m_itemList[pos] = empty;
            return true;
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
        public short AutoPileItemAndGetOccupiedPos (short itemId, short itemNum, out List < (short, E_Item) > posAndChangedItemList, out short piledNum, out short realStoredNum, out E_EmptyItem oriEmptySlot) {
            posAndChangedItemList = new List < (short, E_Item) > ();
            piledNum = 0;
            realStoredNum = 0;
            oriEmptySlot = null;
            // 堆叠
            for (int i = 0; i < m_itemList.Count; i++) {
                var itemInRepo = m_itemList[i];
                if (itemInRepo.m_ItemId == itemId && itemInRepo.m_num != itemInRepo.m_MaxNum) {
                    posAndChangedItemList.Add (((short) i, itemInRepo));
                    short added = itemInRepo.AddNum (itemNum);
                    piledNum += added;
                    realStoredNum = piledNum;
                    itemNum -= added;
                    if (itemNum == 0)
                        return -1;
                }
            }
            // 寻找空插槽
            for (short i = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) {
                    oriEmptySlot = m_itemList[i] as E_EmptyItem;
                    realStoredNum += itemNum;
                    return i;
                }
            }
            return -2;
        }
    }
    class E_StoreHouse : E_Bag {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.STORE_HOUSE; } }
    }
    class E_EquipmentRegion : E_Bag {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.EQUIPMENT_REGION; } }
        public E_EquipmentItem GetEquipmentByEquipPosition (EquipmentPosition eqPos) {
            for (int i = 0; i < m_itemList.Count; i++)
                if ((m_itemList[i] as E_EquipmentItem).m_EquipmentPosition == eqPos)
                    return m_itemList[i] as E_EquipmentItem;
            return null;
        }
        /// <summary>
        /// 返回卸下的原装备
        /// </summary>
        public E_EquipmentItem PutOnEquipment (E_EquipmentItem eq) {
            int oriPos = -1;
            E_Item res = null;
            for (int i = 0; i < m_itemList.Count; i++)
                if ((m_itemList[i] as E_EquipmentItem).m_EquipmentPosition == eq.m_EquipmentPosition) {
                    oriPos = i;
                    res = m_itemList[i];
                }
            if (oriPos != -1)
                m_itemList[oriPos] = eq;
            else
                m_itemList.Add (eq);
            return res as E_EquipmentItem;
        }
    }
}