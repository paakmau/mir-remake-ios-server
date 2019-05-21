using System.Collections.Generic;

namespace MirRemakeBackend.EntityManager {
    class EM_Mission {
        private Dictionary<short, E_Mission> s_missionDict = new Dictionary<short, E_Mission> ();
        public EM_Mission () {
            
        }
        public E_Mission GetMissionById (short missionId) {
            E_Mission res = null;
            s_missionDict.TryGetValue (missionId, out res);
            return res;
        }
        public void LoadMissionList (List<E_Mission> missionList) {
            for (int i=0; i<missionList.Count; i++)
                s_missionDict.Add (missionList[i].m_id, missionList[i]);
        }
    }
}