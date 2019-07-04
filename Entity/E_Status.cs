using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    abstract class E_Status {
        public DE_Status m_statusDe;
        public abstract StatusType m_Type { get; }
        public int m_casterNetId;
        public short m_Id { get { return m_statusDe.m_id; } }
        public float m_value;
        private float m_durationTime;
        public MyTimer.Time m_endTime;
        public void Reset (DE_Status de) {
            m_statusDe = de;
        }
        public void ResetValues (float value, float durationTime, int casterNetId) {
            m_value = value;
            m_durationTime = durationTime;
            m_endTime = MyTimer.s_CurTime.Ticked (m_durationTime);
            m_casterNetId = casterNetId;
        }
        public NO_Status GetNo () {
            return new NO_Status (m_Id, m_value, m_durationTime);
        }
    }
    class E_ChangeHpStatus : E_Status {
        public override StatusType m_Type { get { return StatusType.CHANGE_HP; } }
    }
    class E_ChangeMpStatus : E_Status {
        public override StatusType m_Type { get { return StatusType.CHANGE_MP; } }
    }
    class E_ConcreteAttributeStatus : E_Status {
        private DE_ConcreteAttributeStatus m_conAttrDe;
        public override StatusType m_Type { get { return StatusType.CONCRETE_ATTRIBUTE; } }
        public void ResetConAttrData (DE_ConcreteAttributeStatus casDe) {
            m_conAttrDe = casDe;
        }
    }
    class E_SpecialAttributeStatus : E_Status {
        private DE_SpecialAttributeStatus m_spAttrDe;
        public override StatusType m_Type { get { return StatusType.SPECIAL_ATTRIBUTE; } }
        public void ResetSpAttrData (DE_SpecialAttributeStatus sasDe) {
            m_spAttrDe = sasDe;
        }
    }
}