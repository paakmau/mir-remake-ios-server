using UnityEngine;

namespace MirRemake {
    /// <summary>
    /// 目标的位置
    /// 目标可以为可动BattleUnit, 也可以为静态Vector2
    /// </summary>
    struct TargetPosition {
        private E_ActorUnit m_target;
        private Vector2 m_position;
        private bool m_hasTarPos;
        public bool m_HasTarPos { get { return m_hasTarPos; } }
        public TargetPosition (E_ActorUnit target) {
            m_target = target;
            m_position = Vector2.zero;
            if(target == null)
                m_hasTarPos = false;
            else
                m_hasTarPos = true;
        }
        public TargetPosition (Vector2 pos) {
            m_target = null;
            m_position = pos;
            m_hasTarPos = true;
        }
        /// <summary>
        /// 目标点有效
        /// </summary>
        /// <param name="target"></param>
        /// <param name="pos"></param>
        public TargetPosition (E_ActorUnit target, Vector2 pos) {
            m_target = target;
            m_position = pos;
            m_hasTarPos = true;
        }
        public Vector2 m_Position {
            get {
                if (m_target != null)
                    return m_target.m_position;
                return m_position;
            }
        }
        public E_ActorUnit m_Target {
            get { return m_target; }
        }
    }
}