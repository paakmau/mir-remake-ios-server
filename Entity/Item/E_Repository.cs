using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    abstract class E_RepositoryBase {
        public abstract void Reset (E_Item[] itemArr);
        public abstract NO_Repository GetNo ();
        public abstract E_Item GetItemByRealId (long realId);
        public abstract E_Item GetItemByPosition (int pos);
        /// <summary>
        /// 直接在Pos位置覆盖放置item  
        /// 不考虑原有道具
        /// </summary>
        public abstract void SetItem (E_Item item, int pos);
        /// <summary>
        /// 从背包移除整格物品  
        /// 成功返回true
        /// </summary>
        public abstract bool RemoveItemByRealId (long realId);
    }
    class E_Repository : E_RepositoryBase {
        private List<E_Item> m_itemList = new List<E_Item> ();
        public List<E_Item> m_ItemList { get { return m_itemList; } }
        private List<ValueTuple<int, E_Item>> t_intItemList = new List < (int, E_Item) > ();
        public override void Reset (E_Item[] itemArr) {
            m_itemList.Clear ();
            foreach (var item in itemArr)
                m_itemList.Add (item);
        }
        public override NO_Repository GetNo () {
            var itemNoList = new List<NO_Item> (m_itemList.Count);
            var equipInfoNoList = new List<NO_EquipmentItemInfo> ();
            for (int i = 0; i < m_itemList.Count; i++) {
                itemNoList.Add (m_itemList[i].GetItemNo ());
                if (m_itemList[i].m_Type == ItemType.EQUIPMENT)
                    equipInfoNoList.Add (((E_EquipmentItem) m_itemList[i]).GetEquipmentInfoNo ());
            }
            return new NO_Repository ();
        }
        public override E_Item GetItemByRealId (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId)
                    return m_itemList[i];
            return null;
        }
        public override E_Item GetItemByPosition (int pos) {
            if (m_itemList.Count <= pos)
                return null;
            return m_itemList[pos];
        }
        public E_Item GetItemByRealId (long realId, out int resPos) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    resPos = i;
                    return m_itemList[i];
                }
            resPos = -1;
            return null;
        }
        public override bool RemoveItemByRealId (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    m_itemList[i] = E_Item.s_emptyItem;
                    return true;
                }
            return false;
        }
        public override void SetItem (E_Item item, int pos) {
            if (m_itemList.Count <= pos)
                return;
            m_itemList[pos] = item;
        }
        public bool RemoveItemByPosition (int pos) {
            if (m_itemList.Count <= pos)
                return false;
            if (m_itemList[pos].m_IsEmpty)
                return false;
            m_itemList[pos] = E_Item.s_emptyItem;
            return true;
        }
        /// <summary>
        /// 存储可堆叠Item  
        /// 若正常存储返回true  
        /// 未能完全存入返回false  
        /// </summary>
        /// <param name="posAndChangedItemList">
        /// 背包内的因为插入而被修改的物品列表
        /// </param>
        /// <returns></returns>
        public bool AutoStorePiledItem (E_Item item, out List<ValueTuple<int, E_Item>> posAndChangedItemList) {
            posAndChangedItemList = t_intItemList;
            posAndChangedItemList.Clear ();
            short res = item.m_num;
            for (int i = 0; i < m_itemList.Count; i++) {
                var itemInRepo = m_itemList[i];
                // 找到空的插槽
                if (itemInRepo.m_IsEmpty) {
                    itemInRepo = item;
                    return true;
                }
                // 可以堆叠
                if (itemInRepo.m_itemId == item.m_itemId && itemInRepo.m_num != item.m_MaxNum) {
                    posAndChangedItemList.Add (new ValueTuple<int, E_Item> (i, itemInRepo));
                    short added = itemInRepo.AddNum (item.m_num);
                    if (item.RemoveNum (added))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 储存不可堆叠Item  
        /// 成功返回位置  
        /// 失败返回-1
        /// </summary>
        public int AutoStoreSingleItem (E_Item item) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_IsEmpty) {
                    m_itemList[i] = item;
                    return i;
                }
            return -1;
        }
    }

    class E_EquipmentRegion : E_RepositoryBase {
        Dictionary<EquipmentPosition, E_EquipmentItem> m_equipPositionAndEquipmentDict = new Dictionary<EquipmentPosition, E_EquipmentItem> ();
        public override void Reset (E_Item[] itemArr) {
            m_equipPositionAndEquipmentDict.Clear ();
            foreach (var item in itemArr)
                m_equipPositionAndEquipmentDict.Add (((E_EquipmentItem) item).m_EquipmentPosition, (E_EquipmentItem) item);
        }
        public override NO_Repository GetNo () {
            List<NO_Item> itemList = new List<NO_Item> (m_equipPositionAndEquipmentDict.Count);
            List<NO_EquipmentItemInfo> eqInfoList = new List<NO_EquipmentItemInfo> (m_equipPositionAndEquipmentDict.Count);
            var en = m_equipPositionAndEquipmentDict.Values.GetEnumerator ();
            while (en.MoveNext ()) {
                itemList.Add (en.Current.GetItemNo ());
                eqInfoList.Add (en.Current.GetEquipmentInfoNo ());
            }
            return new NO_Repository (itemList, eqInfoList);
        }
        public override E_Item GetItemByRealId (long realId) {
            var en = m_equipPositionAndEquipmentDict.Values.GetEnumerator ();
            while (en.MoveNext ()) {
                if (en.Current.m_realId == realId)
                    return en.Current;
            }
            return null;
        }
        public override E_Item GetItemByPosition (int pos) {
            E_EquipmentItem res = null;
            EquipmentPosition eqPos = (EquipmentPosition) pos;
            m_equipPositionAndEquipmentDict.TryGetValue (eqPos, out res);
            return res;
        }
        public override void SetItem (E_Item item, int pos) {
            EquipmentPosition eqPos = (EquipmentPosition) pos;
            m_equipPositionAndEquipmentDict[eqPos] = item as E_EquipmentItem;
        }
        public override bool RemoveItemByRealId (long realId) {
            var en = m_equipPositionAndEquipmentDict.GetEnumerator ();
            while (en.MoveNext ()) {
                if (en.Current.Value.m_realId == realId) {
                    m_equipPositionAndEquipmentDict.Remove (en.Current.Key);
                    return true;
                }
            }
            return false;
        }
        public List<E_Item> GetAllItemList () {
            List<E_Item> res = new List<E_Item> ();
            var equipmentEn = m_equipPositionAndEquipmentDict.Values.GetEnumerator ();
            while (equipmentEn.MoveNext ())
                res.Add (equipmentEn.Current);
            return res;
        }
        public Dictionary<EquipmentPosition, E_EquipmentItem>.Enumerator GetEquipedEn () {
            return m_equipPositionAndEquipmentDict.GetEnumerator ();
        }
        public E_EquipmentItem GetEquipmentByEquipPosition (EquipmentPosition ePos) {
            E_EquipmentItem res = null;
            m_equipPositionAndEquipmentDict.TryGetValue (ePos, out res);
            return res;
        }
        public E_EquipmentItem PutOnEquipment (E_EquipmentItem eq) {
            E_EquipmentItem res = GetEquipmentByEquipPosition (eq.m_EquipmentPosition);
            m_equipPositionAndEquipmentDict[eq.m_EquipmentPosition] = res;
            return res;
        }
    }
}