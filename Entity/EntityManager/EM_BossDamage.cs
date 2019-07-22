using System.Collections.Generic;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理阵营信息, 组队
    /// </summary>
    class EM_BossDamage : EntityManagerBase {
        public static EM_BossDamage s_instance;
        private const float c_bossDmgRefreshTime = 1800;
        private List < (int, Dictionary < int, (string, int) >) > m_bossDmgList = new List < (int, Dictionary < int, (string, int) >) > ();
        private List < (int, MyTimer.Time) > m_bossDmgRefreshList = new List < (int, MyTimer.Time) > ();
        public void AddBoss (int bossNetId) {
            m_bossDmgList.Add ((bossNetId, new Dictionary < int, (string, int) > ()));
            m_bossDmgRefreshList.Add ((bossNetId, MyTimer.s_CurTime));
        }
        public IReadOnlyList < (int, Dictionary < int, (string, int) >) > GetBossDmgList () {
            return m_bossDmgList;
        }
        public Dictionary < int, (string, int) > GetBossDmgDict (int bossNetId) {
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