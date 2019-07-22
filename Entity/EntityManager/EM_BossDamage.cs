using System.Collections.Generic;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理阵营信息, 组队
    /// </summary>
    class EM_BossDamage : EntityManagerBase {
        public static EM_BossDamage s_instance;
        private const float c_bossDmgRefreshTime = 1800;
        /// <summary>
        /// 键为 charId
        /// </summary>
        private List < (int, Dictionary < int, (int, string) >) > m_bossDmgList = new List < (int, Dictionary < int, (int, string) >) > ();
        private List < (int, MyTimer.Time) > m_bossDmgRefreshList = new List < (int, MyTimer.Time) > ();
        public void AddBoss (int bossNetId) {
            m_bossDmgList.Add ((bossNetId, new Dictionary < int, (int, string) > ()));
            m_bossDmgRefreshList.Add ((bossNetId, MyTimer.s_CurTime));
        }
        /// <summary>
        /// 键为 charId
        /// </summary>
        public IReadOnlyList < (int, Dictionary < int, (int, string) >) > GetBossDmgList () {
            return m_bossDmgList;
        }
        /// <summary>
        /// 键为 charId
        /// </summary>
        public Dictionary < int, (int, string) > GetBossDmgDict (int bossNetId) {
            for (int i = 0; i < m_bossDmgList.Count; i++)
                if (m_bossDmgList[i].Item1 == bossNetId)
                    return m_bossDmgList[i].Item2;
            return null;
        }
        public void UpdateBossDmg (int netId) {
            for (int i = 0; i < m_bossDmgRefreshList.Count; i++)
                if (m_bossDmgRefreshList[i].Item1 == netId)
                    m_bossDmgRefreshList[i] = (netId, MyTimer.s_CurTime.Ticked (c_bossDmgRefreshTime));
        }
        public void RefreshBossDmg () {
            for (int i = 0; i < m_bossDmgList.Count; i++) {
                if (m_bossDmgList[i].Item2.Count == 0) continue;
                if (MyTimer.CheckTimeUp (m_bossDmgRefreshList[i].Item2))
                    m_bossDmgList[i].Item2.Clear ();
            }
        }
    }
}