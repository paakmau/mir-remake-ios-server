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
    class GL_MonsterAction : GameLogicBase {
        abstract class SkillParamGeneratorBase {
            public abstract SkillAimType m_AimType { get; }
            public virtual bool InCastRange (E_ActorUnit self, float castRange, E_ActorUnit aimedTarget) {
                return (self.m_position - aimedTarget.m_position).LengthSquared() <= castRange * castRange;
            }
            /// <summary>
            /// 完善技能参数
            /// 例如自动选择最近的目标等, 看甲方
            /// </summary>
            /// <returns></returns>
            public abstract SkillParam GetSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget);
        }
        class SPG_AIM_CIRCLE : SkillParamGeneratorBase {
            public override SkillAimType m_AimType { get { return SkillAimType.AIM_CIRCLE; } }
            public override SkillParam GetSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget) {
                return new SkillParam (m_AimType, aimedTarget, Vector2);
            }
        }
        public GL_MonsterAction (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
    }
}