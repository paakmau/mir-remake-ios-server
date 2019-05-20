/// <summary>
/// Enity，物品基类
/// 创建者 fn
/// 时间 2019/4/1
/// 最后修改者 fn
/// 时间 2019/4/4
/// </summary>


using UnityEngine;
using System.Collections.Generic;
using System;
namespace MirRemakeBackend {
    abstract class E_Item {
        /// <summary>
        /// 为 -1 则为空物品
        /// </summary>
        public short m_id;
        public bool m_IsEmpty { get { return m_Type == ItemType.EMPTY; } }
        public long m_realId;
        public virtual ItemType m_Type { get; }
        public short m_maxNum;
        public short m_num;
        public ItemQuality m_quality;
        public long m_price;
        public short m_bindCharacterId = -1;
        public E_Item(long realId, short id, short num) {
            m_id = id;
            m_realId = realId;
            m_num = num;
        }
        /// <summary>
        /// 从一个道具中移除一定的数量, 返回从当前道具中移除的量
        /// </summary>
        /// <param name="num">需要移除的量</param>
        /// <returns>仍未移除的量</returns>
        public short RemoveNum (short num) {
            short res = Math.Min (num, m_num);
            m_num = (short)Math.Max (m_num - num, 0);
            return res;
        }
    }
}