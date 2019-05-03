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
namespace MirRemake {
    class E_Item {
        protected short m_id;
        protected string m_name;
        protected ItemType m_type;
        protected string m_details;
        protected short m_maxNum;
        protected short m_num;
        protected byte m_quality;
        protected long m_price;
        protected short m_bindCharacterId = -1;
        protected E_Effect m_effect;

        /// <summary>
        /// 物品id   
        /// </summary>
        /// <value></value>
        public short m_Id {
            get {
                return m_id;
            }
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        /// <value></value>
        public string m_Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// 物品描述
        /// </summary>
        /// <value></value>
        public string m_Details {
            get {
                return m_details;
            }
        }


        /// <summary>
        /// 在repository中单物品格最大堆叠
        /// </summary>
        /// <value></value>
        public short m_MaxNum {
            get {
                return m_maxNum;
            }
        }

        /// <summary>
        /// 在repository中的数量
        /// </summary>
        /// <value></value>
        public short m_Num {
            set {
                m_num = value;
            }
            get {
                return m_num;
            }
        }

        /// <summary>
        /// 物品种类
        /// </summary>
        /// <value></value>
        public ItemType m_Type {
            get {
                return m_type;
            }
        }

        /// <summary>
        /// 物品品质
        /// </summary>
        /// <value></value>
        public byte m_Quality {
            set {
                m_quality = value;
            }
            get {
                return m_quality;
            }
        }

        /// <summary>
        /// 物品系统售价
        /// </summary>
        /// <value></value>
        public long m_Price {
            set {
                m_price = value;
            }
            get {
                return m_price;
            }
        }

        /// <summary>
        /// 绑定的角色id，非绑定物品为-1
        /// </summary>
        /// <value></value>
        public short m_BindCharacterId { 
            set {
                this.m_bindCharacterId = value;
            }
            get {
                return this.m_BindCharacterId;
            }
        }

        /// <summary>
        /// 消耗品的使用效果，其他类型物品为null
        /// </summary>
        /// <value></value>
        public E_Effect m_Effect {
            get {
                return m_effect;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public E_Item(){
            
        }

        /// <summary>
        /// 复制一个E_Item实体
        /// </summary>
        /// <returns>新的E_Item实体</returns>
        public E_Item Clone() {
            E_Item newItem = new E_Item();
            newItem.m_id = this.m_id;
            newItem.m_name = this.m_name;
            newItem.m_details = this.m_details;
            newItem.m_maxNum = this.m_maxNum;
            newItem.m_num = this.m_num;
            newItem.m_quality = this.m_quality;
            newItem.m_price = this.m_price;
            newItem.m_bindCharacterId = this.m_bindCharacterId;
            return newItem;
        }
    }
}