using System.Collections.Generic;

namespace MirRemakeBackend {
    static class EM_Mission {
        private static Dictionary<short, E_Mission> s_missionDict = new Dictionary<short, E_Mission> ();
        public static E_Mission GetMissionById (short missionId) {
            E_Mission res = null;
            s_missionDict.TryGetValue (missionId, out res);
            return res;
        }
        public static void LoadMissionList (List<E_Mission> missionList) {
            for (int i=0; i<missionList.Count; i++)
                s_missionDict.Add (missionList[i].m_id, missionList[i]);
        }
    }
}