/**
 *Enity，状态实体
 *创建者 yuk
 *时间 2019/4/3
 *最后修改者 yuk
 *时间 2019/4/3
 */

using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    struct E_Status {
        public short m_id;
        public int m_castererNetworkId;
        public StatusType m_type;
        public KeyValuePair<ActorUnitConcreteAttributeType, int>[] m_affectConcreteAttributeArr;
        public ActorUnitSpecialAttributeType[] m_affectSpecialAttributeArr;
        public int m_DeltaHP {
            get {
                int res = 0;
                foreach (var affectAttr in m_affectConcreteAttributeArr) {
                    if (affectAttr.Key == ActorUnitConcreteAttributeType.HP_DAMAGE_PER_SECOND_MAIGC || affectAttr.Key == ActorUnitConcreteAttributeType.HP_DAMAGE_PER_SECOND_PHYSICS)
                        res += affectAttr.Value;
                }
                return res;
            }
        }
        public float m_value;
        private float m_durationTime;
        public float m_DurationTime {
            get { return m_durationTime; }
            set {
                m_durationTime = value;
                m_endTime = MyTimer.s_CurTime.Ticked (value);
            }
        }
        public MyTimer.Time m_endTime;
        public E_Status (DO_Status statusDo, int casterNetId) {
            m_id = statusDo.m_id;
            m_type = statusDo.m_type;
            m_castererNetworkId = casterNetId;
            m_affectConcreteAttributeArr = statusDo.m_affectAttributeArr;
            m_affectSpecialAttributeArr = statusDo.m_specialAttributeArr;
            m_value = statusDo.m_value;
            m_durationTime = statusDo.m_lastingTime;
            m_endTime = MyTimer.s_CurTime.Ticked (m_durationTime);
        }
        public NO_Status GetNo () {
            return new NO_Status (m_id, m_value, m_durationTime);
        }
    }
}