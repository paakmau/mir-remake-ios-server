using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 索引所有Character的任务
    /// </summary>
    class EM_Mission {
        public static EM_Mission s_instance;
        private Dictionary<short, E_Mission> s_missionDict = new Dictionary<short, E_Mission> ();
        public EM_Mission () { }
        public void InitCharacterMission () {
            for (int i = 0; i < missionList.Count; i++)
                s_missionDict.Add (missionList[i].m_id, missionList[i]);
        }
        public E_Mission GetMissionById (short missionId) {
            E_Mission res = null;
            s_missionDict.TryGetValue (missionId, out res);
            return res;
        }
    }
}