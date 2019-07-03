using System.Numerics;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    struct SkillParam {
        public SkillAimType m_aimType;
        /// <summary>
        /// 技能的选定作用目标
        /// </summary>
        public E_Unit m_target;
        public Vector2 m_direction;
        public Vector2 m_position;
        public SkillParam (SkillAimType aimType, E_Unit target, Vector2 direciton, Vector2 position) {
            m_aimType = aimType;
            m_target = target;
            m_direction = direciton;
            m_position = position;
        }
        public NO_SkillParam GetNo () {
            return new NO_SkillParam (m_target.m_networkId, m_direction, m_position);
        }
    }
}