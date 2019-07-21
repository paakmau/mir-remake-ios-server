using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理阵营信息, 组队
    /// </summary>
    class EM_BossDamage : EntityManagerBase {
        public static EM_BossDamage s_instance;
        private Dictionary<int, Dictionary<int, (string, int)>> m_bossDmgDict = new Dictionary<int, Dictionary<int, (string, int)>> ();
        public void AddBoss (int bossNetId) {
            m_bossDmgDict.Add (bossNetId, new Dictionary<int, (string, int)> ());
        }
        public Dictionary<int, (string, int)> GetBossDmgDict (int bossNetId) {
            Dictionary<int, (string, int)> res;
            m_bossDmgDict.TryGetValue (bossNetId, out res);
            return res;
        }
    }
}