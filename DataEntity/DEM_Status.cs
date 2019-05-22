using System.Collections.Generic;
using MirRemakeBackend.Data;
using UnityEngine;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Status {
        public static DEM_Status s_instance;
        private Dictionary<short, DE_Status> m_statusDict;
        public DEM_Status (IDS_Status statusDs) {
            var statusDoArr = statusDs.GetAllStatus ();
            foreach (var statusDo in statusDoArr)
                m_statusDict.Add (statusDo.m_statusId, new DE_Status (statusDo));
        }
        public DE_Status GetStatusById (short statusId) {
            DE_Status res = null;
            m_statusDict.TryGetValue (statusId, out res);
            return res;
        }
    }
}