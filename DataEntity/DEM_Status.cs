using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Status {
        private Dictionary<short, DE_Status> m_statusDict = new Dictionary<short, DE_Status> ();
        private Dictionary<short, DE_ConcreteAttributeStatus> m_conAttrStatusDict = new Dictionary<short, DE_ConcreteAttributeStatus> ();
        private Dictionary<short, DE_SpecialAttributeStatus> m_spAttrStatusDict = new Dictionary<short, DE_SpecialAttributeStatus> ();
        public DEM_Status (IDS_Status statusDs) {
            var hpStatusArr = statusDs.GetAllChangeHpStatus ();
            var mpStatusArr = statusDs.GetAllChangeMpStatus ();
            var conAttrStatusArr = statusDs.GetAllConcreteAttributeStatus ();
            var spAttrStatusArr = statusDs.GetAllSpecialAttributeStatus ();
            foreach (var status in hpStatusArr)
                m_statusDict.Add (status.m_statusId, new DE_Status (status));
            foreach (var status in mpStatusArr)
                m_statusDict.Add (status.m_statusId, new DE_Status (status));
            foreach (var status in conAttrStatusArr) {
                m_statusDict.Add (status.Item1.m_statusId, new DE_Status (status.Item1));
                m_conAttrStatusDict.Add (status.Item1.m_statusId, new DE_ConcreteAttributeStatus (status.Item2));
            }
            foreach (var status in spAttrStatusArr) {
                m_statusDict.Add (status.Item1.m_statusId, new DE_Status (status.Item1));
                m_spAttrStatusDict.Add (status.Item1.m_statusId, new DE_SpecialAttributeStatus (status.Item2));
            }
        }
        public DE_Status GetStatusById (short statusId) {
            DE_Status res;
            m_statusDict.TryGetValue (statusId, out res);
            return res;
        }
        public DE_ConcreteAttributeStatus GetConAttrStatusById (short statusId) {
            DE_ConcreteAttributeStatus res;
            m_conAttrStatusDict.TryGetValue (statusId, out res);
            return res;
        }
        public DE_SpecialAttributeStatus GetSpAttrStatusById (short statusId) {
            DE_SpecialAttributeStatus res;
            m_spAttrStatusDict.TryGetValue (statusId, out res);
            return res;
        }
    }
}