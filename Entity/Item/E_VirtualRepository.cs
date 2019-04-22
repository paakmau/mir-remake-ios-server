/// <summary>
/// Enity，虚拟背包，用于其他玩家的角色
/// 创建者 fn
/// 时间 2019/4/3
/// 最后修改者 fn
/// 时间 2019/4/4
/// </summary>


using UnityEngine;
using System.Collections.Generic;
namespace MirRemake {
    // TODO:全部联网接口
    class E_VirtualRepository : E_AbstractRepository {
        public override List<E_Item> DropLegacy() {
            return null;
        }

        public override List<E_Item> StoreItems(List<E_Item> items) {
            return null;
        }

        public override bool RemoveItems(List<E_Item> items) {
            return false;
        }

        public override short StoreItem(E_Item item, short num) {
            return 0;
        }

        public override bool RemoveItem(E_Item item, short num) {
            return false;
        }

        public override List<E_Item> GetAllItems() {
            return null;
        }

        public override List<E_Item> GetItemsByType(ItemType type) {
            return null;
        }
    }
}