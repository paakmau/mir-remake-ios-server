
namespace MirRemakeBackend.Util {
    static class MyTimer {
        /// <summary>
        /// 循环计时器  
        /// 浮点数, 单位为秒
        /// </summary>
        public struct Time {
            private const float c_cycleTime = 10000f;
            private int m_cycle;
            private float m_time;
            public static bool operator <= (Time a, Time b) {
                if (a.m_cycle == b.m_cycle && a.m_time <= b.m_time)
                    return true;
                if (a.m_cycle + 1 == b.m_cycle && a.m_time <= b.m_time + c_cycleTime)
                    return true;
                return false;
            }
            public static bool operator >= (Time a, Time b) {
                if (a.m_cycle == b.m_cycle && a.m_time >= b.m_time)
                    return true;
                if (a.m_cycle == b.m_cycle + 1 && a.m_time + c_cycleTime >= b.m_time)
                    return true;
                return false;
            }
            public Time (int cycle, float time) {
                m_cycle = cycle;
                m_time = time;
            }
            public void Tick (float dT) {
                m_time += dT;
                if (m_time >= c_cycleTime) {
                    m_time -= c_cycleTime;
                    m_cycle ++;
                }
            }
            public Time Ticked (float dT) {
                Tick (dT);
                return this;
            }
        }
        private static Time s_timer = new Time (0 , 0f);
        public static Time s_CurTime { get { return s_timer; } }
        public static void Tick (float dT) {
            s_timer.Tick (dT);
        }
        /// <summary>
        /// 判断time是否已经达到
        /// </summary>
        public static bool CheckTimeUp (Time time) {
            return s_CurTime >= time;
        }
    }
}