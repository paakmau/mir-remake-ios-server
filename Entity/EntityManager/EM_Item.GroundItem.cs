using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理游戏场景中出现的所有道具
    /// 范围: 仓库, 背包, 地面
    /// </summary>
    partial class EM_Item {
        private class GroundItemIdManager {
            private long m_groundItemIdCnt = 0;
            public long AssignGroundItemId () {
                return ++m_groundItemIdCnt;
            }
        }
        private const float c_groundItemDisappearTime = 15;
        private GroundItemIdManager m_groundItemIdManager = new GroundItemIdManager ();
        private List<E_GroundItem> m_groundItemList = new List<E_GroundItem> ();
        private Dictionary<int, List<E_GroundItem>> m_characterGroundItemSightDict = new Dictionary<int, List<E_GroundItem>> ();
        private List < (short, Vector2, MyTimer.Time) > m_renewableItemList = new List < (short, Vector2, MyTimer.Time) > ();
        private float c_renewableItemRefreshTimeMin = 12;
        private float c_renewableItemRefreshTimeMax = 18;
        private float c_renewableItemRefreshRadian = 2;
        public short CharacterPickGroundItem (int charId, E_GroundItem gndItem, E_Bag bag, out List < (short, E_Item) > resChangedItemList, out E_Item resStoreItem, out short resStorePos) {
            var res = GainItem (charId, gndItem.m_item, bag, out resChangedItemList, out resStoreItem, out resStorePos);
            // 移除gndItem
            for (int i = 0; i < m_groundItemList.Count; i++)
                if (m_groundItemList[i] == gndItem)
                    m_groundItemList.RemoveAt (i);
            // 回收gndItem
            m_groundItemPool.RecycleInstance (gndItem);
            return res;
        }
        public void CharacterDropItemOntoGround (E_Item item, short num, int charId, E_Bag repo, short repoPos, Vector2 gndCenterPos) {
            if (num == 0) return;
            E_GroundItem gndItem = m_groundItemPool.GetInstance ();
            long gndItemId = m_groundItemIdManager.AssignGroundItemId ();
            Vector2 gndPos = gndCenterPos + new Vector2 (MyRandom.NextFloat (0, 2) - 1, MyRandom.NextFloat (0, 2) - 1);
            if (num >= item.m_num) {
                // 完全丢弃
                E_EmptyItem slot = m_itemFactory.GetEmptyItemInstance ();
                repo.SetItem (slot, repoPos);
                m_ddh.Delete (item);
                m_ddh.Insert (slot, charId, repo.m_repositoryPlace, repoPos);
                item.ResetRealId (-1);
                gndItem.Reset (gndItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), item, -1, gndPos);
            } else {
                // 部分丢弃
                item.RemoveNum (num);
                m_ddh.Save (item, charId, repo.m_repositoryPlace, repoPos);
                var sepItem = m_itemFactory.GetAndInitInstance (item.m_ItemId, num);
                gndItem.Reset (gndItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), sepItem, -1, gndPos);
            }
            m_groundItemList.Add (gndItem);
        }
        public void GenerateGroundItem (IReadOnlyList < (short, short) > itemIdAndNumList, int charId, Vector2 centerPos) {
            for (int i = 0; i < itemIdAndNumList.Count; i++) {
                var pos = centerPos + new Vector2 (MyRandom.NextFloat (0, 2) - 1, MyRandom.NextFloat (0, 2) - 1);
                GenerateGroundItem (itemIdAndNumList[i].Item1, itemIdAndNumList[i].Item2, charId, pos);
            }
        }
        /// <summary>
        /// 创建地面物品
        /// </summary>
        public void GenerateGroundItem (short itemId, short num, int charId, Vector2 pos) {
            var item = m_itemFactory.GetAndInitInstance (itemId, num);
            if (item == null)
                return;
            var gndItem = m_groundItemPool.GetInstance ();
            long groundItemId = m_groundItemIdManager.AssignGroundItemId ();
            gndItem.Reset (groundItemId, MyTimer.s_CurTime.Ticked (c_groundItemDisappearTime), item, charId, pos);
            m_groundItemList.Add (gndItem);
        }
        public void RefreshGroundItemAutoDisappear () {
            for (int i = m_groundItemList.Count - 1; i >= 0; i--)
                if (MyTimer.CheckTimeUp (m_groundItemList[i].m_disappearTime)) {
                    m_groundItemPool.RecycleInstance (m_groundItemList[i]);
                    m_itemFactory.RecycleItem (m_groundItemList[i].m_item);
                    m_groundItemList.RemoveAt (i);
                }
        }
        public E_GroundItem GetGroundItem (long gndItemId) {
            for (int i = 0; i < m_groundItemList.Count; i++)
                if (m_groundItemList[i].m_groundItemId == gndItemId)
                    return m_groundItemList[i];
            return null;
        }
        public List<E_GroundItem> GetRawGroundItemList () {
            return m_groundItemList;
        }
        public List<E_GroundItem> GetCharacterGroundItemRawSight (int netId) {
            List<E_GroundItem> res;
            m_characterGroundItemSightDict.TryGetValue (netId, out res);
            return res;
        }
        public void RefreshRenewableItem () {
            for (int i = 0; i < m_renewableItemList.Count; i++) {
                var itemIdPosTime = m_renewableItemList[i];
                if (MyTimer.CheckTimeUp (itemIdPosTime.Item3)) {
                    short itemId = itemIdPosTime.Item1;
                    Vector2 pos = itemIdPosTime.Item2 + new Vector2 (MyRandom.NextFloat (0, c_renewableItemRefreshRadian * 2) - c_renewableItemRefreshRadian, MyRandom.NextFloat (0, c_renewableItemRefreshRadian * 2) - c_renewableItemRefreshRadian);
                    // 生成地面物品
                    GenerateGroundItem (itemId, 1, -1, pos);

                    // 准备下一次刷新
                    itemIdPosTime.Item3 = MyTimer.s_CurTime.Ticked (MyRandom.NextFloat (c_renewableItemRefreshTimeMin, c_renewableItemRefreshTimeMax));
                    m_renewableItemList[i] = itemIdPosTime;
                }
            }
        }
    }
}