using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Mission {
        private Dictionary<short, DE_Mission> m_missionDict = new Dictionary<short, DE_Mission> ();
        private List<DE_Mission> m_initUnlockMisList = new List<DE_Mission> ();
        public DEM_Mission (IDS_Mission ds) {
            var doArr = ds.GetAllMission ();
            foreach (var mDo in doArr) {
                var de = new DE_Mission (mDo);
                m_missionDict.Add (mDo.m_id, de);
                // TODO: 让yzj改为 0长度数组
                if (mDo.m_fatherMissionIdArr.Length == 1 && mDo.m_fatherMissionIdArr[0] == -1)
                    m_initUnlockMisList.Add (de);
            }
        }
        public DE_Mission GetMissionById (short missionId) {
            DE_Mission res = null;
            m_missionDict.TryGetValue (missionId, out res);
            return res;
        }
        public IReadOnlyList<DE_Mission> GetInitUnlockMisIdList () {
            return m_initUnlockMisList;
        }
    }
}