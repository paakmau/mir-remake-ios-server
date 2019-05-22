using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class E_Repository {
        //该repository中的所有物品
        private List<E_Item> m_itemList;
        public List<E_Item> m_ItemList { get { return m_itemList; } }
        public E_Repository (List<E_Item> m_itemList) {
            this.m_itemList = m_itemList;
        }
    }
}