using System;
using System.Collections.Generic;

namespace MirRemakeBackend {
    class E_Repository {
        private List<E_Item> m_itemList;
        public List<E_Item> m_ItemList { get { return m_itemList; } }
        public E_Repository (List<E_Item> m_itemList) {
            this.m_itemList = m_itemList;
        }
        public short RemoveItem (long realId, short num) {
            for (int i=0; i<m_itemList.Count; i++)
                if (m_itemList[i].m_realId == realId) {
                    short res = Math.Min (num, m_itemList[i].m_num);
                    m_itemList[i].m_num -= res;
                    return res;
                }
            return 0;
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