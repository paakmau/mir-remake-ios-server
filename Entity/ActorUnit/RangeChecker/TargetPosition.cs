using UnityEngine;

namespace MirRemake {
    /// <summary>
    /// 目标的位置
    /// 目标可以为可动BattleUnit, 也可以为静态Vector2
    /// </summary>
    struct TargetPosition {
        private bool m_isMovable;
        public bool m_IsMovable { get { return m_isMovable; } }
        private E_ActorUnit m_target;
        private Vector2 m_position;
        private bool m_hasTarPos;
        public bool m_HasTarPos { get { return m_hasTarPos; } }
        public Vector2 m_Position {
            get {
                if (m_isMovable)
                    return m_target.m_Position;
                return m_position;
            }
        }
        public E_ActorUnit m_Target {
            get { return m_target; }
        }
        public TargetPosition (E_ActorUnit target) {
            m_isMovable = true;
            m_target = target;
            m_position = Vector2.zero;
            if(target == null)
                m_hasTarPos = false;
            else
                m_hasTarPos = true;
        }
        public TargetPosition (Vector2 pos) {
            m_isMovable = false;
            m_target = null;
            m_position = pos;
            m_hasTarPos = true;
        }
        public TargetPosition (NO_TargetPosition no) {
            m_isMovable = no.m_isMovable;
            if (no.m_isMovable) {
                m_target = SM_ActorUnit.s_instance.GetActorUnitByNetworkId (no.m_targetNetworkId);
                m_position = Vector2.zero;
                m_hasTarPos = m_target != null;
            }
            else {
                m_target = null;
                m_position = no.m_targetPosition;
                m_hasTarPos = true;
            }
        }
        public NO_TargetPosition GetNo () {
            if (m_isMovable)
                return new NO_TargetPosition (m_target.m_networkId);
            else
                return new NO_TargetPosition (m_position);
        }
    }
}