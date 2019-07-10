namespace MirRemakeBackend.Util {
    static class MyTimer {
        /// <summary>
        /// 计时器  
        /// 浮点数, 单位为秒
        /// </summary>
        public struct Time {
            private const float c_cycleTime = 10000f;
            private long m_cycle;
            private float m_time;
            public Time (int cycle, float time) {
                m_cycle = cycle;
                m_time = time;
            }
            public Time (Time t) {
                m_cycle = t.m_cycle;
                m_time = t.m_time;
            }
            /// <summary>
            /// 自身后移dT
            /// </summary>
            public void Tick (float dT) {
                m_time += dT;
                while (m_time >= c_cycleTime) {
                    m_time -= c_cycleTime;
                    m_cycle++;
                }
            }
            /// <summary>
            /// 返回Tick后的新的Time, 自身不变
            /// </summary>
            public Time Ticked (float dT) {
                Time res = new Time (this);
                res.Tick (dT);
                return res;
            }
            public static bool operator <= (Time a, Time b) {
                if (a.m_cycle == b.m_cycle && a.m_time <= b.m_time)
                    return true;
                if (a.m_cycle <= b.m_cycle)
                    return true;
                return false;
            }
            public static bool operator >= (Time a, Time b) {
                if (a.m_cycle > b.m_cycle)
                    return true;
                if (a.m_cycle == b.m_cycle && a.m_time >= b.m_time)
                    return true;
                return false;
            }
            public override string ToString() {
                return m_cycle.ToString () + " " + m_time.ToString ();
            }
        }
        private static Time s_timer = new Time (0, 0f);
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