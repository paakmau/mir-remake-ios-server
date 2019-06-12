using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    abstract class E_RepositoryBase {
        public ItemPlace m_repositoryPlace;
        protected void Reset (ItemPlace place) {
            m_repositoryPlace = place;
        }
        public abstract void Reset (ItemPlace place, E_Item[] itemArr);
        public abstract NO_Repository GetNo ();
        public abstract E_Item GetItemByRealId (long realId);
        public abstract E_Item GetItemByPosition (short pos);
        /// <summary>
        /// 直接在Pos位置覆盖放置item  
        /// 不考虑原有道具
        /// </summary>
        public abstract void SetItem (E_Item item, short pos);
        /// <summary>
        /// 从背包移除整格物品  
        /// 成功返回true
        /// </summary>
        public abstract E_Item RemoveItemByRealId (E_EmptyItem empty);
    }
    class E_Repository : E_RepositoryBase {
        private List<E_Item> m_itemList = new List<E_Item> ();
        public List<E_Item> m_ItemList { get { return m_itemList; } }
        public override void Reset (ItemPlace place, E_Item[] itemArr) {
            base.Reset (place);
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
        public override E_Item RemoveItemByRealId (E_EmptyItem empty) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == empty.m_realId) {
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
        /// </summary>
        /// <param name="posAndChangedItemList">
        /// 背包内的因为插入而被修改的物品 (pos, obj) 列表
        /// </param>
        /// <returns>
        /// 若item占用了一个槽位返回 pos  
        /// 若完全堆叠返回 -1  
        /// 未能完全存入返回 -2  
        /// </returns>
        public short AutoStoreItem (E_Item item, out List < (short, E_Item) > posAndChangedItemList) {
            posAndChangedItemList = new List < (short, E_Item) > ();
            // 堆叠
            for (int i = 0; i < m_itemList.Count; i++) {
                var itemInRepo = m_itemList[i];
                if (itemInRepo.m_ItemId == item.m_ItemId && itemInRepo.m_num != item.m_MaxNum) {
                    posAndChangedItemList.Add (((short) i, itemInRepo));
                    short added = itemInRepo.AddNum (item.m_num);
                    if (item.RemoveNum (added))
                        return -1;
                }
            }
            // 寻找空插槽
            for (short i = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) {
                    item.m_realId = m_itemList[i].m_realId;
                    m_itemList[i] = item;
                    return i;
                }
            }
            return -2;
        }
    }

    class E_EquipmentRegion : E_RepositoryBase {
        Dictionary<EquipmentPosition, E_EquipmentItem> m_equipPositionAndEquipmentDict = new Dictionary<EquipmentPosition, E_EquipmentItem> ();
        public override void Reset (ItemPlace place, E_Item[] itemArr) {
            base.Reset (place);
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
        public override E_Item GetItemByPosition (short pos) {
            E_EquipmentItem res = null;
            EquipmentPosition eqPos = (EquipmentPosition) pos;
            m_equipPositionAndEquipmentDict.TryGetValue (eqPos, out res);
            return res;
        }
        public override void SetItem (E_Item item, short pos) {
            EquipmentPosition eqPos = (EquipmentPosition) pos;
            m_equipPositionAndEquipmentDict[eqPos] = item as E_EquipmentItem;
        }
        public override E_Item RemoveItemByRealId (E_EmptyItem empty) {
            var en = m_equipPositionAndEquipmentDict.GetEnumerator ();
            while (en.MoveNext ()) {
                if (en.Current.Value.m_realId == empty.m_realId) {
                    var res = en.Current.Value;
                    m_equipPositionAndEquipmentDict.Remove (en.Current.Key);
                    return res;
                }
            }
            return null;
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