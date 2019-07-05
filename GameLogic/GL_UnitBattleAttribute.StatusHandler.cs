using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic
{
    partial class GL_UnitBattleAttribute
    {
        private interface IStatusHandler {
            StatusType m_Type { get; }
            void Attach (E_Status status, E_Unit target, E_Unit caster);
            void TickPerSecond (E_Status status, E_Unit target);
            void Remove (E_Status status, E_Unit target);
        }
        private class SH_ChangeHp : IStatusHandler {
            public StatusType m_Type { get { return StatusType.CHANGE_HP; } }
            public void Attach (E_Status status, E_Unit target, E_Unit caster) { }
            public void TickPerSecond (E_Status status, E_Unit target) {
                target.m_curHp += (int) status.m_value;
            }
            public void Remove (E_Status status, E_Unit target) { }
        }
        private class SH_ChangeMp : IStatusHandler {
            public StatusType m_Type { get { return StatusType.CHANGE_MP; } }
            public void Attach (E_Status status, E_Unit target, E_Unit caster) { }
            public void TickPerSecond (E_Status status, E_Unit target) {
                target.m_curMp += (int) status.m_value;
            }
            public void Remove (E_Status status, E_Unit target) { }
        }
        private class SH_ConcreteAttribute : IStatusHandler {
            public StatusType m_Type { get { return StatusType.CONCRETE_ATTRIBUTE; } }
            public void Attach (E_Status status, E_Unit target, E_Unit caster) { }
            public void TickPerSecond (E_Status status, E_Unit target) { }
            public void Remove (E_Status status, E_Unit target) { }
        }
        private class SH_SpecialAttribute : IStatusHandler {
            public StatusType m_Type { get { return StatusType.SPECIAL_ATTRIBUTE; } }
            public void Attach (E_Status status, E_Unit target, E_Unit caster) { }
            public void TickPerSecond (E_Status status, E_Unit target) { }
            public void Remove (E_Status status, E_Unit target) { }
        }
    }
}