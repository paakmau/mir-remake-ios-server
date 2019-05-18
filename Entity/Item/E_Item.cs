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
        public static E_Item CreateInstance (ItemType type, long realId, short id, short num) {
            switch (type) {
                case ItemType.CONSUMABLE:
                    return new E_ConsumableItem (realId, id, num);
                case ItemType.EQUIPMENT:
                    return new E_EquipmentItem (realId, id, num);
                case ItemType.MATERIAL:
                    return new E_MaterialItem (realId, id, num);
                case ItemType.EMPTY:
                    return new E_EmptyItem ();
            }
            return null;
        }
        /// <summary>
        /// 为 -1 则为空物品
        /// </summary>
        public short m_id;
        public bool m_IsEmpty { get { return m_Type == ItemType.EMPTY; } }
        public long m_realId;
        public string m_name;
        public virtual ItemType m_Type { get; }
        public string m_details;
        public short m_maxNum;
        public short m_num;
        public ItemQuality m_quality;
        public long m_price;
        public short m_bindCharacterId = -1;
        public E_Item(long realId, short id, short num) {
            m_id = id;
            m_realId = realId;
            m_num = num;
            // TODO:测试用
            switch(id) {
                case 1:
                    m_name = "圣剑";
                    m_details = "传说中的圣骑士hbm曾经使用这把剑斩杀了恶魔首领67";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.HEIRLOOM;
                    break;
                case 2:
                    m_name = "战神斧";
                    m_details = "传说中的战神yzj在封神之战中使用这把斧头击败了所有的挑战者";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.HEIRLOOM;
                    break;
                case 3:
                    m_name = "fa*杖";
                    m_details = "相传某位王使用这根法杖捅穿了67的后花园";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.LEGENDARY;
                    break;
                case 4:
                    m_name = "香港记者的鞋子";
                    m_details = "这双鞋子曾是世界上跑得最快的记者所使用，代表了当时最先进的科技，其舒适的脚感能够让使用者健步如飞";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.EPIC;
                    break;
                case 5:
                    m_name = "fa*袍";
                    m_details = "传说中某位王的上衣";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.RARE;
                    break;
                case 6:
                    m_name = "战神头盔";
                    m_details = "传说中的战神yzj在封神之战中使用的头盔";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.HEIRLOOM;
                    break;
                case 7:
                    m_name = "冠军戒指";
                    m_details = "篮球之神cxk在WNBA中获得的总冠军戒指";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.POOR;
                    break;    
                case 8:
                    m_name = "战神护腕";
                    m_details = "传说中的战神yzj在封神之战中使用的护腕";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.HEIRLOOM;
                    break;
                case 9:
                    m_name = "圣碗";
                    m_details = "传说中的圣骑士hbm在斩杀67时所佩戴的护腕";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.UNCOMMON;
                    break;
                case 10:
                    m_name = "MVP戒指";
                    m_details = "篮球之神cxk在WNBA季后赛中获得的MVP戒指";
                    m_num = 1;
                    m_maxNum = 1;
                    m_quality = ItemQuality.COMMON;
                    break;
                default:
                    break;
            }
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