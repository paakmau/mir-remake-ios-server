/**
 *抽象类，repository类
 *创建者 fn
 *时间 2019/4/3
 *最后修改者 fn
 *时间 2019/4/3
 */
/// <summary>
/// 抽象类，repository类
/// 创建者 fn
/// 时间 2019/4/3
/// 最后修改者 fn
/// 时间 2019/4/4
/// </summary>

using UnityEngine;
using System.Collections.Generic;
namespace MirRemake {
    abstract class E_AbstractRepository {
        public abstract List<E_Item> DropLegacy();
        public abstract List<E_Item> StoreItems(List<E_Item> items);
        public abstract bool RemoveItems(List<E_Item> items);
        public abstract short StoreItem(E_Item item, short num);
        public abstract bool RemoveItem(E_Item item, short num);
        public abstract List<E_Item> GetAllItems();
        public abstract List<E_Item> GetItemsByType(ItemType type);
    }
}