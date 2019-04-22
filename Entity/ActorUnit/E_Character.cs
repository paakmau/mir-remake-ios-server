/// <summary>
/// Enity，角色类
/// 创建者 fn
/// 时间 2019/4/3
/// 最后修改者 fn
/// 时间 2019/4/3
/// </summary>

 using UnityEngine;
 using System.Collections.Generic;
 namespace MirRemake {
     class E_Character : E_ActorUnit {

        protected E_AbstractRepository m_bag;
        protected long m_virtualMoney;
        protected long m_chargeMoney;

        /// <summary>
        /// 虚拟金币，若虚拟金币为非负数，则为我的角色；若为-1，则为联网玩家，需要调用网络接口
        /// </summary>
        /// <value></value>
        public long m_VirtualMoney {
            get {
                switch(this.m_virtualMoney) {
                    case -1:
                        return -1;// TODO:联网接口
                    default:
                        return this.m_virtualMoney;
                }
            }
            set {
                switch (this.m_virtualMoney) {
                    case -1:
                        break;//TODO:联网接口
                    default:
                        this.m_virtualMoney = value;
                        break;
                }
            }
        }
        
        /// <summary>
        /// 充值金币，若充值金币为非负数，则为我的角色；若为-1，则为联网玩家，需要调用网络接口；若为-2，则为NPC，且该NPC的金钱值无限，不可更改
        /// </summary>
        /// <value></value>
        public long m_ChargeMoney {
            get {
                switch(this.m_chargeMoney) {
                    case -1:
                        return -1;// TODO:联网接口
                    default:
                        return this.m_chargeMoney;
                }
            }
            set {
                switch(this.m_chargeMoney) {
                    case -1:
                        break;// TODO:联网接口
                    default:
                        this.m_chargeMoney = value;
                        break;
                }
            }
        }


        /// <summary>
        /// 获取角色某种类型全部技能
        /// </summary>
        /// <param name="type">枚举，技能类型</param>
        /// <returns>技能实体列表，该类型的所有技能</returns>
        public List<E_Skill> GetAllMyRoleSkillByType(byte type) {
            return this.m_skillList;
        }

        /// <summary>
        /// 获得物品
        /// </summary>
        /// <param name="items">物品实体列表，获得的物品</param>
        /// <returns>物体实体列表，未装下的物品列表（背包已满，无法装下所有物品）</returns>
        public List<E_Item> GainItems(List<E_Item> items) {
            return m_bag.StoreItems(items);
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="item">物品实体，使用的物品</param>
        /// <returns>是否成功</returns>
        public bool UseItem(E_Item item) {
            return false;
        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        /// <param name="item">物品实体，丢弃的物品</param>
        /// <param name="num">丢弃的数量</param>
        /// <returns>是否成功</returns>
        public bool abandonItem(E_Item item, short num) {
            return m_bag.RemoveItem(item, num);
        }

        /// <summary>
        /// 查看Repository中储存的所有物品
        /// </summary>
        /// <returns>物品实体列表，可以看到的所有物品</returns>
        public List<E_Item> GetAllItemsInBag() {
            return m_bag.GetAllItems();
        }

        /// <summary>
        /// 查看repository中某种类型的物品
        /// </summary>
        /// <param name="repository">枚举（背包、仓库、角色装备区），要查看的repository</param>
        /// <param name="type">枚举（消耗物品、装备物品、材料物品），要查看的类型</param>
        /// <returns>物品实体列表，可以看到的物品</returns>
        public List<E_Item> GetItemsInBagByType(ItemType type) {
            return m_bag.GetItemsByType(type);
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        /// <param name="item">物品实体，出售的物品</param>
        /// <param name="num">出售物品的数量</param>
        /// <param name="price">成交单价</param>
        /// <param name="currentType">货币种类</param>
        /// <returns>是否成功</returns>
        public bool SellItem(E_Item item, short num, long price, CurrencyType currentType) {
            long myMoney = this.getMoneyByCurrencyType(currentType);
            if(myMoney >= 0) {
                if(this.m_bag.RemoveItem(item, num)) {
                    this.SetMoneyByCurrencyType(myMoney + (price * num), currentType);
                    return true;
                }
            }else if(myMoney == -1) {
                //TODO:联网接口
            }
            return false;
        }

        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="item">物品实体，购买的物品</param>
        /// <param name="num">购买物品的数量</param>
        /// <param name="price">成交单价</param>
        /// <param name="CurrencyType">货币种类</param>
        /// <returns>是否成功</returns>
        public bool BuyItem(E_Item item, short num, long price, CurrencyType currencyType) {
            long myMoney = this.getMoneyByCurrencyType(currencyType);
            if(myMoney >= 0) {
                if(myMoney >= (price * num)) {
                    this.SetMoneyByCurrencyType(myMoney - (price * num), currencyType);
                    this.m_bag.StoreItem(item, num);
                }else {
                    return false;
                }
            }else if(myMoney == -1) {
                // TODO:联网接口
            }
            return false;
        }

        public override List<E_Item> DropLegacy() {
            return m_bag.DropLegacy();
        }

        /// <summary>
        /// 获取character的某种货币数量
        /// </summary>
        /// <param name="type">货币种类</param>
        /// <returns>数量</returns>
        public long getMoneyByCurrencyType(CurrencyType type) {
            switch(type) {
                case CurrencyType.CHARGE:
                    return this.m_chargeMoney;
                case CurrencyType.VIRTUAL:
                    return this.m_virtualMoney;
                default:
                    return 0;
            }
        }
        
        protected void SetMoneyByCurrencyType(long money, CurrencyType type) {
            if(money < 0) {
                return;
            }
            switch(type) {
                case CurrencyType.CHARGE:
                    this.m_chargeMoney = money;
                    break;
                case CurrencyType.VIRTUAL:
                    this.m_virtualMoney = money;
                    break;
                default:
                    break;
            }
        }
    }
}
