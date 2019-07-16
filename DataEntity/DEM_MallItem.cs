using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 道具  
    /// </summary>
    class DEM_MallItem {
        private Dictionary<byte, string> m_mallClassDict = new Dictionary<byte, string> ();
        private Dictionary<byte, List<DE_MallItem>> m_mallItemDict = new Dictionary<byte, List<DE_MallItem>> ();
        public DEM_MallItem (IDS_Mall mallDs) {
            // 获取所有的Item
            var mallClassAndItems = mallDs.GetAllMallItemAndClass ();
            var mallClassList = mallClassAndItems.Item1;
            var mallItemList = mallClassAndItems.Item2;
            for (int i = 0; i < mallClassList.Count; i++) {
                m_mallClassDict.Add (mallClassList[i].m_mallItemClassId, mallClassList[i].m_mallItemClassName);
                m_mallItemDict.Add (mallClassList[i].m_mallItemClassId, new List<DE_MallItem> ());
            }
            for (int i = 0; i < mallItemList.Count; i++) {
                m_mallItemDict[mallItemList[i].m_mallItemClassId].Add (new DE_MallItem (mallItemList[i]));
            }
        }
        public Dictionary<byte, string>.Enumerator GetMallClassEn () {
            return m_mallClassDict.GetEnumerator ();
        }
        public List<DE_MallItem> GetMallItemByClassId (byte classId) {
            List<DE_MallItem> res;
            m_mallItemDict.TryGetValue (classId, out res);
            return res;
        }
    }
}