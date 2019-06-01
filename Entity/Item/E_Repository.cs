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
        /// 从背包移除整格物品  
        /// </summary>
        public bool RemoveItem (long realId) {
            for (int i=0; i<m_itemList.Count; i++)
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
            for (int i=0; i<m_itemList.Count; i++) {
                if (m_itemList[i].m_itemId != item.m_itemId) continue;
                int canStoreNum = m_itemList[i].m_MaxNum - m_itemList[i].m_num;
                if (canStoreNum == 0) continue;
                
            }
            return res;
        }
    }
}