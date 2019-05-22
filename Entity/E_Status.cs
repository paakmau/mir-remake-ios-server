using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Network;
using UnityEngine;

namespace MirRemakeBackend.Entity {
    struct E_Status {
        // TODO: 有缘改成命令模式, 可以实现嘲讽
        public int m_castererNetworkId;
        public short m_id;
        public DE_Status m_dataEntity;
        public int m_Hatred {
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
        public E_Status (DE_Status de, short id, KeyValuePair<float, float> statusParm, int casterNetId) {
            m_castererNetworkId = casterNetId;
            m_id = id;
            m_dataEntity = de;

            m_value = statusParm.Key;
            m_durationTime = statusParm.Value;
            m_endTime = MyTimer.s_CurTime.Ticked (m_durationTime);
        }
        public NO_Status GetNo () {
            return new NO_Status (m_id, m_value, m_durationTime);
        }
    }
}