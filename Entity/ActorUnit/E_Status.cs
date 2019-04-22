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
        public StatusType m_type;
        public string m_name;
        public string m_details;
        public Dictionary<ActorUnitConcreteAttributeType, int> m_affectAttributeDict;
        public int m_value;
        public float m_leftTime;
        public void Tick(float dT) {
            m_leftTime -= dT;
        }
        public E_Status (short id, int value, float leftTime) {
            m_type = StatusType.DEBUFF; // TODO: 等yzj完成E_Status
            m_name = "龙之吐息";
            m_details = "gfy深深吸了一口气，并对你缓缓吐出";
            m_affectAttributeDict = new Dictionary<ActorUnitConcreteAttributeType, int> () {
                { ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, -100 }
            };

            m_id = id;
            m_value = value;
            m_leftTime = leftTime;
        }
    }
}