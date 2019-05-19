using System.Collections.Generic;

namespace MirRemakeBackend {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    static class EM_Item {
        private static Dictionary<long, E_Item> m_realIdAndItemDict = new Dictionary<long, E_Item> ();
        public static E_Item GetItemByRealId (long realId) {
            E_Item res = null;
            m_realIdAndItemDict.TryGetValue (realId, out res);
            return res;
        }
        public static void UnloadItemByRealId (long realId) {
            m_realIdAndItemDict.Remove (realId);
        }
        public static void LoadItem (E_Item item) {
            m_realIdAndItemDict.Add (item.m_realId, item);
        }
        public static void LoadItemList (List<E_Item> itemList) {
            for (int i = 0; i < itemList.Count; i++)
                m_realIdAndItemDict.Add (itemList[i].m_realId, itemList[i]);
        }
    }
}