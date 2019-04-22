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
        private short id;
        private string name;
        private ItemType type;
        private string details;
        private short maxNum;
        private short num;
        private byte quality;
        private long price;
        private short bindCharacterId = -1;
        private E_Effect effect;

        /// <summary>
        /// 物品id   
        /// </summary>
        /// <value></value>
        public short Id {
            get {
                return id;
            }
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        /// <value></value>
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// 物品描述
        /// </summary>
        /// <value></value>
        public string Details {
            get {
                return details;
            }
        }


        /// <summary>
        /// 在repository中单物品格最大堆叠
        /// </summary>
        /// <value></value>
        public short MaxNum {
            get {
                return maxNum;
            }
        }

        /// <summary>
        /// 在repository中的数量
        /// </summary>
        /// <value></value>
        public short Num {
            set {
                num = value;
            }
            get {
                return num;
            }
        }

        /// <summary>
        /// 物品种类
        /// </summary>
        /// <value></value>
        public ItemType Type {
            get {
                return type;
            }
        }

        /// <summary>
        /// 物品品质
        /// </summary>
        /// <value></value>
        public byte Quality {
            set {
                quality = value;
            }
            get {
                return quality;
            }
        }

        /// <summary>
        /// 物品系统售价
        /// </summary>
        /// <value></value>
        public long Price {
            set {
                price = value;
            }
            get {
                return price;
            }
        }

        /// <summary>
        /// 绑定的角色id，非绑定物品为-1
        /// </summary>
        /// <value></value>
        public short BindCharacterId { 
            set {
                this.bindCharacterId = value;
            }
            get {
                return this.BindCharacterId;
            }
        }

        /// <summary>
        /// 消耗品的使用效果，其他类型物品为null
        /// </summary>
        /// <value></value>
        public E_Effect E_Effect {
            get {
                return effect;
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
            newItem.id = this.id;
            newItem.name = this.name;
            newItem.details = this.details;
            newItem.maxNum = this.maxNum;
            newItem.num = this.num;
            newItem.quality = this.quality;
            newItem.price = this.price;
            newItem.bindCharacterId = this.bindCharacterId;
            return newItem;
        }
    }
}