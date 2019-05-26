using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class E_Status {
        // TODO: 有缘改成命令模式, 可以高雅实现秒伤, 嘲讽等复杂状态
        public DE_Status m_dataEntity;
        public int m_castererNetworkId;
        public short m_id;
        public int m_Hatred {
            get {
                return (int)(m_durationTime * m_value);
            }
        }
        public float m_value;
        private float m_durationTime;
        public MyTimer.Time m_endTime;
        public void Reset (DE_Status de, short id, float value, float durationTime, int casterNetId) {
            m_dataEntity = de;
            m_castererNetworkId = casterNetId;
            m_id = id;
            m_value = value;
            m_durationTime = durationTime;
            m_endTime = MyTimer.s_CurTime.Ticked (m_durationTime);
        }
        public NO_Status GetNo () {
            return new NO_Status (m_id, m_value, m_durationTime);
        }
    }
}