using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    class EM_Log : EntityManagerBase {
        public static EM_MonsterSkill s_instance;
        private List<E_Log>[] m_logs = new List<E_Log>[] { new List<E_Log> (), new List<E_Log> () };
        private bool m_curTick;
        public List<E_Log> GetRawLogsCurTick () { return m_logs[m_curTick?1 : 0]; }
        public IReadOnlyList<E_Log> m_PrevTick { get { return m_logs[m_curTick?0 : 1]; } }
        public void Tick () {
            m_curTick = !m_curTick;
        }
    }
}