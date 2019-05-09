/**
 *Enity，状态实体
 *创建者 yuk
 *时间 2019/4/3
 *最后修改者 yuk
 *时间 2019/4/3
 */

using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    struct E_Status {
        public short m_id;
        public int m_attackerNetId;
        public StatusType m_type;
        public string m_name;
        public string m_details;
        public Dictionary<ActorUnitConcreteAttributeType, int> m_affectConcreteAttributeDict;
        public Dictionary<ActorUnitSpecialAttributeType, int> m_affectSpecialAttributeDict;
        public int m_DeltaHP {
            get {
                int res = 0;
                m_affectConcreteAttributeDict.TryGetValue(ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, out res);
                return res;
            }
        }
        public int m_value;
        private float m_durationTime;
        public float m_DurationTime {
            get { return m_durationTime; }
            set {
                m_durationTime = value;
                m_endTime = MyTimer.s_CurTime.Ticked (value);
            }
        }
        public MyTimer.Time m_endTime;
        public E_Status (short id, int value, float durationTime) {
            // TODO: 测试用
            m_type = StatusType.DEBUFF;
            m_name = "龙之吐息";
            m_details = "gfy深深吸了一口气，并对你缓缓吐出";
            m_affectConcreteAttributeDict = new Dictionary<ActorUnitConcreteAttributeType, int> () {
                { ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, -10 }
            };
            m_affectSpecialAttributeDict = null;

            m_id = id;
            m_attackerNetId = 0;
            m_value = value;
            m_durationTime = durationTime;
            m_endTime = MyTimer.s_CurTime.Ticked (durationTime);
        }
    }
}