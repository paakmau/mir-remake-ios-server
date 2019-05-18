using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class Repository {
        //该repository中的所有物品
        private List<E_Item> m_itemList;
        public Repository () {
            m_itemList = new List<E_Item> () {
                E_Item.CreateInstance (ItemType.EQUIPMENT, 2, 1, 1),
                E_Item.CreateInstance (ItemType.EQUIPMENT, 2, 2, 1),
                E_Item.CreateInstance (ItemType.EQUIPMENT, 2, 3, 1),
            };
        }
        public Repository (List<E_Item> m_itemList) {
            this.m_itemList = m_itemList;
        }
        public List<E_Item> GetAllItem () {
            return m_itemList;
        }
        /// <summary>
        /// 更新背包内的物品
        /// </summary>
        /// <param name="itemRemoveArr">需要在背包里移除的物品(按背包中的顺序) RealId</param>
        /// <param name="itemUpdateArr">需要在背包里更新数量的物品(按背包中的顺序) RealId, num</param>
        /// <param name="itemGainArr">需要新增的物品(按背包中的顺序) RealId, 物品种类, itemId, num</param>
        public void UpdateItemByRealId (long[] itemRemoveArr, KeyValuePair<long, short>[] itemUpdateArr, E_Item[] itemGainArr) {
            for (int i = 0, j = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) continue;
                if (m_itemList[i].m_realId == itemRemoveArr[j]) {
                    m_itemList[i] = new E_EmptyItem ();
                    j++;
                }
            }
            for (int i = 0, j = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) continue;
                if (m_itemList[i].m_realId == itemUpdateArr[j].Key) {
                    m_itemList[i].m_num = itemUpdateArr[j].Value;
                    j++;
                }
            }
            int itemGainIndex = 0;
            for (int i = 0; i < m_itemList.Count; i++) {
                if (m_itemList[i].m_IsEmpty) {
                    m_itemList[i] = itemGainArr[itemGainIndex];
                    itemGainIndex++;
                }
            }
            for (; itemGainIndex < itemGainArr.Length; itemGainIndex++)
                m_itemList.Add (itemGainArr[itemGainIndex]);
        }
    }
}