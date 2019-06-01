using System;
using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    class E_Repository {
        private List<E_Item> m_itemList = new List<E_Item> ();
        public List<E_Item> m_ItemList { get { return m_itemList; } }
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
        /// 存储一个Item
        /// 返回未能存入的量
        /// </summary>
        public short StoreItem (E_Item item) {
            short res = item.m_num;
            for (int i = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty || ) {
                    
                }
            }
            return res;
        }
    }
}