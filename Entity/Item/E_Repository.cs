using System;
using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    class E_Repository {
        private List<E_Item> m_itemList = new List<E_Item> ();
        public List<E_Item> m_ItemList { get { return m_itemList; } }
        private List<ValueTuple<int, E_Item>> t_intItemList = new List < (int, E_Item) > ();
        public void Reset (E_Item[] itemArr) {
            m_itemList.Clear ();
            foreach (var item in itemArr)
                m_itemList.Add (item);
        }
        /// <summary>
        /// 根据RealId获取该物品所在的位置  
        /// 若找不到返回-1
        /// </summary>
        public int GetItemPosition (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId)
                    return i;
            return -1;
        }
        /// <summary>
        /// 从背包移除整格物品  
        /// 成功返回true
        /// </summary>
        public bool RemoveItem (long realId) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    m_itemList[i] = E_Item.s_emptyItem;
                    return true;
                }
            return false;
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
        public bool StorePiledItem (E_Item item, out List<ValueTuple<int, E_Item>> posAndChangedItemList) {
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
        public int StoreSingleItem (E_Item item) {
            for (int i = 0; i < m_itemList.Count; i++)
                if (m_itemList[i].m_IsEmpty) {
                    m_itemList[i] = item;
                    return i;
                }
            return -1;
        }
    }
}