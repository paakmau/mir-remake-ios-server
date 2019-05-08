
using UnityEngine;

namespace MirRemake {
    struct SkillParam {
        public Vector2 m_param;
        public Vector2 m_Direction { get { return m_param; } }
        public Vector2 m_Position { get { return m_param; } }
        public SkillParam (Vector2 parm) {
            m_param = parm;
        }
    }
}