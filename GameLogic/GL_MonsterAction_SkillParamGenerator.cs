using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 控制Monster的AI
    /// </summary>
    partial class GL_MonsterAction : GameLogicBase {
        abstract class SkillParamGeneratorBase {
            public static Dictionary<SkillAimType, SkillParamGeneratorBase> s_spgDict = new Dictionary<SkillAimType, SkillParamGeneratorBase> () {
                {SkillAimType.AIM_CIRCLE, new SPG_AimCircle ()}
            };
            public abstract SkillAimType m_AimType { get; }
            public virtual bool InCastRange (E_Unit self, float castRange, E_Unit aimedTarget) {
                return (self.m_position - aimedTarget.m_position).LengthSquared () <= castRange * castRange;
            }
            /// <summary>
            /// 完善技能参数
            /// 例如自动选择最近的目标等, 看甲方
            /// </summary>
            /// <returns></returns>
            public abstract SkillParam GetSkillParam (E_Unit self, E_Unit aimedTarget);
        }
        class SPG_AimCircle : SkillParamGeneratorBase {
            public override SkillAimType m_AimType { get { return SkillAimType.AIM_CIRCLE; } }
            public override SkillParam GetSkillParam (E_Unit self, E_Unit aimedTarget) {
                return new SkillParam (m_AimType, aimedTarget, Vector2.Zero, Vector2.Zero);
            }
        }
    }
}