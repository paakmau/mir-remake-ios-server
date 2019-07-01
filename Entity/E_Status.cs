using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_Status {
        public DE_Status m_dataEntity;
        public abstract StatusType m_Type { get; }
        public int m_casterNetId;
        public short m_id;
        public float m_value;
        private float m_durationTime;
        public MyTimer.Time m_endTime;
        public void Reset (DE_Status de, float value, float durationTime, int casterNetId) {
            m_dataEntity = de;
            m_id = de.m_id;
            m_value = value;
            m_durationTime = durationTime;
            m_endTime = MyTimer.s_CurTime.Ticked (m_durationTime);
            m_casterNetId = casterNetId;
        }
        public NO_Status GetNo () {
            return new NO_Status (m_id, m_value, m_durationTime);
        }
    }
    class E_ChangeHpStatus : E_Status {
        public override StatusType m_Type { get { return StatusType.CHANGE_HP; } }
    }
    class E_ChangeMpStatus : E_Status {
        public override StatusType m_Type { get { return StatusType.CHANGE_MP; } }
    }
    class E_ConcreteAttributeStatus : E_Status {
        public override StatusType m_Type { get { return StatusType.CONCRETE_ATTRIBUTE; } }
        public 
        public void Reset (DE_Status statusDe, DE_ConcreteAttributeStatus casDe, float value, float durationTime, int casterNetId) {
            base.Reset (statusDe, value, durationTime, casterNetId);
        }
    }
    class E_SpecialAttributeStatus : E_Status {
        public override StatusType m_Type { get { return StatusType.SPECIAL_ATTRIBUTE; } }
        public void Reset (DE_Status statusDe, DE_SpecialAttributeStatus sasDe, float value, float durationTime, int casterNetId) {
            base.Reset (statusDe, value, durationTime, casterNetId);
        }
    }
}