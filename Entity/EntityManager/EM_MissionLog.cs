using System.Collections.Generic;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    // TODO: 添加充值的 Log
    class EM_MissionLog {
        class LogFactory {
            private const int c_poolSize = 5000;
            private Dictionary<MissionTargetType, ObjectPool> m_pool = new Dictionary<MissionTargetType, ObjectPool> ();
            public LogFactory () {
                m_pool.Add (MissionTargetType.KILL_MONSTER, new ObjectPool<E_KillMonsterLog> (5000));
                m_pool.Add (MissionTargetType.GAIN_ITEM, new ObjectPool<E_GainItemLog> (5000));
                m_pool.Add (MissionTargetType.LEVEL_UP, new ObjectPool<E_LevelUpLog> (5000));
                m_pool.Add (MissionTargetType.LEVEL_UP_SKILL, new ObjectPool<E_LevelUpSkillLog> (5000));
                m_pool.Add (MissionTargetType.TALK_TO_NPC, new ObjectPool<E_TalkToNpcLog> (5000));
            }
            public E_MissionLog GetLogInstance (MissionTargetType type) {
                return (E_MissionLog) m_pool[type].GetInstanceObj ();
            }
            public void RecycleInstance (E_MissionLog logObj) {
                m_pool[logObj.m_LogType].RecycleInstance (logObj);
            }
        }
        public static EM_MissionLog s_instance;
        private LogFactory m_logFactory = new LogFactory ();
        private const int c_tickToSave = 4;
        private List<E_MissionLog>[] m_logs = new List<E_MissionLog>[c_tickToSave] { new List<E_MissionLog> (), new List<E_MissionLog> (), new List<E_MissionLog> (), new List<E_MissionLog> () };
        private int m_curTick;
        private int GetNextTick (int tick) {
            int res = m_curTick + 1;
            while (res >= c_tickToSave)
                res -= c_tickToSave;
            return res;
        }
        public List<E_MissionLog> GetRawLogsCurTick () { return m_logs[m_curTick]; }
        public IReadOnlyList<E_MissionLog> GetLogsSecondTick () { return m_logs[GetNextTick (GetNextTick (m_curTick))]; }
        public void NextTick () {
            m_curTick = GetNextTick (m_curTick);
            GetRawLogsCurTick ().Clear ();
        }
        public E_MissionLog CreateLog (MissionTargetType type, int netId, int parm1, int parm2, int parm3) {
            var log = m_logFactory.GetLogInstance (type);
            log.Reset (netId, parm1, parm2, parm3);
            return log;
        }
    }
}