
using UnityEngine;

namespace MirRemakeBackend {
    // TODO: 改成接口md草
    /// <summary>
    /// 技能参数
    /// 技能释放的时候根据用户锁定的目标与方向决定
    /// </summary>
    struct SkillParam {
        public static SkillParam s_invalidSkillParam = new SkillParam () { m_isValid = false };
        public SkillAimType m_aimType;
        /// <summary>
        /// 技能的选定作用目标
        /// </summary>
        public E_ActorUnit m_target;
        public Vector2 m_direction;
        public Vector2 m_position;
        public bool m_isValid;
        public Vector2 m_TargetPosition {
            get {
                if (m_target != null)
                    return m_target.m_Position;
                return m_position;
            }
        }
        public SkillParam (SkillAimType aimType, E_ActorUnit target, Vector2 direciton, Vector2 position) {
            m_aimType = aimType;
            m_target = target;
            m_direction = direciton;
            m_position = position;
            m_isValid = true;
        }
        public NO_SkillParam GetNo () {
            return new NO_SkillParam (m_target.m_networkId, m_direction, m_position);
        }
    }
}