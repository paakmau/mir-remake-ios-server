using System;
using UnityEngine;

namespace MirRemake {
    struct FSMS_CastBegin : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_BEGIN; } }
        public E_ActorUnit m_Self { get; set; }
        private TargetPosition m_tarPos;
        public TargetPosition m_TarPos { get { return m_tarPos; } }
        private E_Skill m_skill;
        public E_Skill m_Skill { get { return m_skill; } }
        private Vector2 m_parm;
        public Vector2 m_Parm { get { return m_parm; } }
        private IFSMState m_nextState;
        public FSMS_CastBegin (E_ActorUnit self, TargetPosition tarPos, E_Skill skill, Vector2 parm) {
            m_Self = self;
            m_tarPos = tarPos;
            m_skill = skill;
            m_parm = parm;
            m_nextState = null;
        }
        public bool HasActiveTransitionTo (FSMStateType next) {
            return false;
        }
        public bool CanActiveExit (FSMStateType next) {
            return false;
        }
        public bool CanActiveEnter (FSMStateType prev) {
            // 若被沉默或者消耗无法满足
            if (m_Self.m_IsSilent || !m_skill.CheckCostEnough (m_Self))
                return false;
            return true;
        }
        public void OnEnter (FSMStateType prevType) {
            if (m_skill.m_NeedSing)
                m_nextState = new FSMS_CastSing (m_Self, m_skill, m_tarPos, m_parm);
            else
                m_nextState = new FSMS_CastFront (m_Self, m_skill, m_tarPos, m_parm);
        }
        public void OnTick (float dT) { }
        public IFSMState GetNextState () {
            return m_nextState;
        }
        public void OnExit (FSMStateType nextType) { }
    }
}