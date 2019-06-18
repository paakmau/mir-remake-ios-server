using System.Collections.Generic;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EM_Log : EntityManagerBase {
        class LogFactory {
            private const int c_poolSize = 5000;
            private Dictionary<GameLogType, ObjectPool> m_pool = new Dictionary<GameLogType, ObjectPool> ();
            public LogFactory () {
                m_pool.Add (GameLogType.KILL_MONSTER, new ObjectPool<E_KillMonsterLog> (5000));
                m_pool.Add (GameLogType.GAIN_ITEM, new ObjectPool<E_GainItemLog> (5000));
                m_pool.Add (GameLogType.LEVEL_UP, new ObjectPool<E_LevelUpLog> (5000));
                m_pool.Add (GameLogType.LEVEL_UP_SKILL, new ObjectPool<E_LevelUpSkillLog> (5000));
                m_pool.Add (GameLogType.TALK_TO_NPC, new ObjectPool<E_TalkToNpcLog> (5000));
            }
            public E_Log GetLogInstance (GameLogType type) {
                return (E_Log) m_pool[type].GetInstanceObj ();
            }
            public void RecycleInstance (E_Log logObj) {
                m_pool[logObj.m_LogType].RecycleInstance (logObj);
            }
        }
        public static EM_Log s_instance;
        private LogFactory m_logFactory = new LogFactory ();
        private const int c_tickToSave = 4;
        private List<E_Log>[] m_logs = new List<E_Log>[c_tickToSave] { new List<E_Log> (), new List<E_Log> (), new List<E_Log> (), new List<E_Log> () };
        private int m_curTick;
        private int GetNextTick (int tick) {
            int res = m_curTick + 1;
            while (res >= c_tickToSave)
                res -= c_tickToSave;
            return res;
        }
        public List<E_Log> GetRawLogsCurTick () { return m_logs[m_curTick]; }
        public IReadOnlyList<E_Log> GetLogsSecondTick () { return m_logs[GetNextTick (GetNextTick (m_curTick))]; }
        public void NextTick () {
            m_curTick = GetNextTick (m_curTick);
            GetRawLogsCurTick ().Clear ();
        }
        public E_Log CreateLog (GameLogType type, int parm1, int parm2, int parm3) {
            var log = m_logFactory.GetLogInstance (type);
            log.Reset (parm1, parm2, parm3);
            return log;
        }
    }
}