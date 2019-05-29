using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Mission {
        private Dictionary<short, DE_Mission> m_missionDict;
        public DEM_Mission (IDS_Mission ds) {
            var doArr = ds.GetAllMission ();
            foreach (var mDo in doArr)
                m_missionDict.Add (mDo.m_id, new DE_Mission (mDo));
        }
        public DE_Mission GetMissionById (short missionId) {
            DE_Mission res = null;
            m_missionDict.TryGetValue (missionId, out res);
            return res;
        }
    }
}