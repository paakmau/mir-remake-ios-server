using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend {
    abstract class E_Item {
        private DE_Item m_itemDe;
        public short m_itemId;
        public virtual ItemType m_Type { get; }
        public long m_realId;
        public short m_num;
        public bool m_IsEmpty { get { return m_Type == ItemType.EMPTY; } }
        public short m_MaxNum { get { return m_itemDe.m_maxNum; } }
        public virtual void Reset (DE_Item de, DDO_Item ddo) {
            m_itemDe = de;
            m_itemId = ddo.m_itemId;
            m_realId = ddo.m_realId;
            m_num = ddo.m_num;
        }
        /// <summary>
        /// 从一个道具中移除一定的数量, 返回实际从当前道具中移除的量
        /// </summary>
        /// <param name="num">需要移除的量</param>
        /// <returns>实际移除的量</returns>
        public short RemoveNum (short num) {
            short res = Math.Min (num, m_num);
            m_num -= res;
            return res;
        }
    }
}