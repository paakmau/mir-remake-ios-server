using UnityEngine;

namespace MirRemake {
    struct SkillParam {
        public Vector2 m_param;
        public Vector2 m_Direction { get { return m_param; } }
        public Vector2 m_Position { get { return m_param; } }
        public SkillParam (Vector2 parm) {
            m_param = parm;
        }
        public SkillParam (NO_SkillParam no) : this (no.m_data) { }
        public NO_SkillParam GetNo () {
            return new NO_SkillParam (m_param);
        }
    }
}