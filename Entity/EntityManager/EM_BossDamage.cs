using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理阵营信息, 组队
    /// </summary>
    class EM_BossDamage : EntityManagerBase {
        public static EM_BossDamage s_instance;
        private Dictionary<int, Dictionary<int, int>> m_bossDmgDict = new Dictionary<int, Dictionary<int, int>> ();
        public void AddBoss (int bossNetId) {
            m_bossDmgDict.Add (bossNetId, new Dictionary<int, int> ());
        }
        public Dictionary<int, int> GetBossDmgDict (int bossNetId) {
            Dictionary<int, int> res;
            m_bossDmgDict.TryGetValue (bossNetId, out res);
            return res;
        }
    }
}