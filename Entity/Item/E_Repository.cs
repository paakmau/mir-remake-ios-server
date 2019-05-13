/// <summary>
/// Enity，储存物品实体，包括背包、仓库、角色装备区
/// 创建者 fn
/// 时间 2019/4/1
/// 最后修改者 fn
/// 时间 2019/4/4
/// </summary>


using UnityEngine;
using System.Collections.Generic;
namespace MirRemake {
    class E_Repository : E_AbstractRepository {
        //该repository中的所有物品
        private List<E_Item>  items;
        private short size;

        /// <summary>
        /// 该repository中的所有物品
        /// </summary>
        /// <value></value>
        private List<E_Item> Items {
            get {
                return items;
            }
        }

        /// <summary>
        /// 容量
        /// </summary>
        /// <value></value>
        public short Size {
            set {

            }
            get {
                return size;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="items">包含的所有物品</param>
        /// <param name="size">容量</param>
        public E_Repository(List<E_Item> items, short size) {
            this.items = items;
            this.size = size;
        }

        /// <summary>
        /// 死亡后掉落物品，必须在角色死亡之后才能调用
        /// </summary>
        /// <returns>物品实体列表，掉落的物品</returns>
        public override List<E_Item> DropLegacy() {
            List<E_Item> dropableList = new List<E_Item>();
            List<E_Item> result = new List<E_Item>();
            byte[] weight = new byte[256];
            short totalWeight = 0;
            short i = 0;
            foreach(E_Item item in this.items) {
                if(item.m_BindCharacterId != -1){
                    continue;
                }
                switch(item.m_Type) {
                    case ItemType.EQUIPMENT:
                        weight[i] = (byte)(6-item.m_Quality);
                        totalWeight += weight[i];
                        i++;
                        break;
                    case ItemType.MATERIAL:
                        weight[i] = (byte)((6-item.m_Quality) * 2);
                        totalWeight += weight[i];
                        i++;
                        break;
                    case ItemType.CONSUMABLE:
                        weight[i] = (byte)((6-item.m_Quality) * 3);
                        totalWeight += weight[i];
                        i++;
                        break;
                }
            }
            byte dropNum = (byte)(Random.Range(1,(float)3.000001));
            for(int j = 0; j < dropNum; j++) {
                short luckyNum = (short)(Random.Range(0,totalWeight));
                for(byte count = 0; count < dropableList.Count; count ++) {
                    luckyNum = (short)(luckyNum - weight[count]);
                    if(luckyNum < 0) {
                        E_Item newItem = dropableList[count].Clone();
                        newItem.m_Num = 1;
                        result.Add(newItem);
                        this.RemoveItem(newItem, 1);
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 放入物品
        /// </summary>
        /// <param name="items">物品实体列表，放入的物品列表</param>
        /// <returns>物品实体列表，无法放入的物品，repository已满</returns>
        public override List<E_Item> StoreItems(List<E_Item> items) {
            List<E_Item> restItems = new List<E_Item>();
            foreach(E_Item item in items) {
                short restNum = this.StoreItem(item, item.m_Num);
                if(restNum > 0) {
                    item.m_Num = restNum;
                    restItems.Add(item);
                }
            }
            return restItems;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="items">物品实体列表，移除的物品列表</param>
        /// <returns>是否成功</returns>
        public override bool RemoveItems(List<E_Item> items) {
            foreach(E_Item item in items) {
                bool isSuccess = this.RemoveItem(item, item.m_Num);
                if(!isSuccess) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 放入某数量的物品
        /// </summary>
        /// <param name="item">物品实体，放入的物品</param>
        /// <param name="num">放入的物品数量</param>
        /// <returns>未放入的数量，repository已满</returns>
        public override short StoreItem(E_Item item, short num) {
            List<E_Item> sameItemsInThisRepository = this.searchItemById(item.m_Id);
            //自动堆叠
            foreach (E_Item sameItemEntityInThisRepository in sameItemsInThisRepository) {
                //repository中此格同类物品的数量
                short sameItemEntityInThisRepositoryNum = sameItemEntityInThisRepository.m_Num;
                //最大堆叠
                short maxNum = sameItemEntityInThisRepository.m_MaxNum;
                if(num == 0) {
                    break;
                }
                if(sameItemEntityInThisRepositoryNum < maxNum) {
                    if(num <= (maxNum - sameItemEntityInThisRepositoryNum)) {
                        sameItemEntityInThisRepository.m_Num = (short)(sameItemEntityInThisRepositoryNum + num);
                    }else {
                        sameItemEntityInThisRepository.m_Num = maxNum;
                        num = (short)(num - (maxNum - sameItemEntityInThisRepositoryNum));
                    }
                }
            }
            //占用新格子
            if (num != 0) {
                if(this.items.Count < this.size) {
                    item.m_Num = num;
                    this.items.Add(item);
                    return 0;
                }
            }
            return num;
        }

        /// <summary>
        /// 移除某数量的物品
        /// </summary>
        /// <param name="item">物品实体，移除的物品</param>
        /// <param name="num">移除的物品数量</param>
        /// <returns>是否成功</returns>
        public override bool RemoveItem(E_Item item, short num) {
            List<E_Item> deletingItem = this.searchItemById(item.m_Id);
            foreach(E_Item d_item in deletingItem) {
                if(num == 0) {
                    return true;
                }
                short d_num = d_item.m_Num;
                if(d_num > num) {
                    d_item.m_Num = (short)(d_num - num);
                    return true;
                }else {
                    this.items.Remove(d_item);
                    num = (short)(num - d_num);
                }
            }
            return false;
        }

        /// <summary>
        /// 使用消耗品
        /// </summary>
        /// <param name="consumable">使用的消耗品</param>
        /// <returns>消耗品效果</returns>
        public E_Effect UseConsumable(E_Item consumable) {
            if(this.RemoveItem(consumable, 1)) {
                return consumable.m_Effect;
            }
            return null;
        }

        /// <summary>
        /// 查询该repository中所有的物品
        /// </summary>
        /// <returns>物品实体列表，所有的物品</returns>
        public override List<E_Item> GetAllItems() {
            return this.items;
        }

        /// <summary>
        /// 查询该repository中某类型的物品
        /// </summary>
        /// <param name="type">枚举（消耗物品、装备物品、材料物品），需要查询的类型</param>
        /// <returns>物品实体列表，查询到的所有物品</returns>
        public override List<E_Item> GetItemsByType(ItemType type) {
            List<E_Item> result = new List<E_Item>();
            foreach(E_Item item in this.items) {
                if(item.m_Type == type) {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询该repository中某id的物品
        /// </summary>
        /// <param name="itemId">需要查询的物品id</param>
        /// <returns>查询结果</returns>
        public List<E_Item> searchItemById(short itemId) {
            List<E_Item> result = new List<E_Item>();
            foreach(E_Item item in this.items) {
                if (item.m_Id == itemId) {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}