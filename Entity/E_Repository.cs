using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    // TODO: 做容量检测
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
        public E_Item RemoveItemByRealId (long realId, E_EmptyItem empty) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    var res = m_itemList[i];
                    m_itemList[i] = empty;
                    return res;
                }
            return null;
        }
        public void SetItem (E_Item item, short pos) {
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
    }
    class E_StoreHouse : E_Bag {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.STORE_HOUSE; } }
    }
    class E_EquipmentRegion : E_Bag {
        public override ItemPlace m_repositoryPlace { get { return ItemPlace.EQUIPMENT_REGION; } }
        /// <summary>
        /// 若找不到，则随机返回一个装备区的 E_EmptyItem
        /// </summary>
        public E_Item GetEquipmentByEquipPosition (EquipmentPosition eqPos) {
            for (int i = 0; i < m_itemList.Count; i++) {
                var eq = m_itemList[i] as E_EquipmentItem;
                if (eq == null) continue;
                if (eq.m_EquipmentPosition == eqPos)
                    return eq;
            }
            for (int i = 0; i < m_itemList.Count; i++)
                if ((m_itemList[i].m_Type == ItemType.EMPTY))
                    return m_itemList[i];
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