using System.Collections.Generic;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理商城物品
    /// </summary>
    class EM_MallItem {
        private class MallItemIdManager {
            private int m_groundItemIdCnt = 0;
            public int AssignGroundItemId () {
                return ++m_groundItemIdCnt;
            }
        }
        public static EM_MallItem s_instance;
        private MallItemIdManager m_mallItemIdManager = new MallItemIdManager ();
        private Dictionary<byte, E_MallClass> m_mallClassDict = new Dictionary<byte, E_MallClass> ();
        private Dictionary<int, E_MallItem> m_mallItemDict = new Dictionary<int, E_MallItem> ();
        public int m_MallClassCnt { get { return m_mallClassDict.Count; } }
        public EM_MallItem (DEM_MallItem dem) {
            var classEn = dem.GetMallClassEn ();
            while (classEn.MoveNext ()) {
                var mallItemDeList = dem.GetMallItemByClassId (classEn.Current.Key);
                var mallItemList = new List<E_MallItem> (mallItemDeList.Count);
                for (int i = 0; i < mallItemDeList.Count; i++) {
                    mallItemList.Add (new E_MallItem (m_mallItemIdManager.AssignGroundItemId (), mallItemDeList[i]));
                    m_mallItemDict.Add (mallItemList[i].m_mallItemId, mallItemList[i]);
                }
                var mall = new E_MallClass (classEn.Current.Key, classEn.Current.Value, mallItemList);
                m_mallClassDict.Add (classEn.Current.Key, mall);
            }
        }
        public Dictionary<byte, E_MallClass>.Enumerator GetMallClassEn () {
            return m_mallClassDict.GetEnumerator ();
        }
        public E_MallItem GetMallItemByMallItemId (int mallItemId) {
            E_MallItem res;
            m_mallItemDict.TryGetValue (mallItemId, out res);
            return res;
        }
    }
}