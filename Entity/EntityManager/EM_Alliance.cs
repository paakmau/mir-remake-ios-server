using System.Collections.Generic;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class EM_Alliance {
        public static EM_Alliance s_instance;
        private IDDS_Alliance m_dds;
        private Dictionary<int, string> m_allianceNameDict = new Dictionary<int, string> ();
        private Dictionary<int, List<int>> m_memberDict = new Dictionary<int, List<int>> ();
        private Dictionary<int, List<int>> m_applyDict = new Dictionary<int, List<int>> ();
        public EM_Alliance (IDDS_Alliance dds) {
            var allianceList = dds.GetAllAlliance ();
            for (int i=0; i<allianceList.Count; i++) {
                int allianceId = allianceList[i].m_id;
                m_allianceNameDict.Add (allianceId, allianceList[i].m_name);
                var memberList = dds.GetMemberByAllianceId (allianceId);
                var memberCharIdList = new List<int> (memberList.Count);
                for (int j = 0; j<memberList.Count; j++)
                    memberCharIdList.Add (memberList[j].m_charId);
                m_memberDict.Add (allianceId, memberCharIdList);
                var applyList = dds.GetApplyByAllianceId (allianceId);
                var applyCharIdList = new List<int> (applyList.Count);
                for (int j = 0; j<applyList.Count; j++)
                    applyCharIdList.Add (applyList[j].m_charId);
                m_applyDict.Add (allianceId, applyCharIdList);
            }
        }
        public int CreateAlliance (int charId, string name) {
            int allianceId = m_dds.InsertAlliance (new DDO_Alliance (-1, name));
            m_dds.InsertMember (new DDO_AllianceMember (charId, allianceId, AllianceJob.PRESIDENT));
            return allianceId;
        }
        public void RemoveAlliance (int id) {
            m_dds.DeleteAllianceById (id);
        }
        public int ApplyToJoin (int charId, int allianceId) {
            return m_dds.InsertApply (new DDO_AllianceApply (-1, allianceId, charId));
        }
        public void DeleteApplyToJoin (int id) {
            m_dds.DeleteApplyById (id);
        }
    }
}