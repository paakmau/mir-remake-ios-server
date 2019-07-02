using System.Numerics;
using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic
{
    partial class GL_MonsterAction
    {
        abstract class SkillParamGeneratorBase {
            public static Dictionary<SkillAimType, SkillParamGeneratorBase> s_spgDict = new Dictionary<SkillAimType, SkillParamGeneratorBase> () { { SkillAimType.AIM_CIRCLE, new SPG_AimCircle () } };
            public abstract SkillAimType m_AimType { get; }
            public virtual bool InCastRange (E_Unit self, float castRange, E_Unit aimedTarget) {
                return (self.m_position - aimedTarget.m_position).LengthSquared () <= castRange * castRange;
            }
            /// <summary>
            /// 根据目标完善技能参数
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
        class SPG_AimOneTarget : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.AIM_ONE_TARGET;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,Vector2.Zero,Vector2.Zero);
            }
        }
        class SPG_AimSelfRect : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.AIM_SELF_RECT;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,Vector2.Zero,Vector2.Zero);
            }
        }
        class SPG_AimSelfSector : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.AIM_SELF_SECTOR;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,Vector2.Zero,Vector2.Zero);
            }
        }
        class SPG_NotAimCircle : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.NOT_AIM_CIRCLE;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,Vector2.Zero,aimedTarget.m_position);
            }
        }
        class SPG_NotAimSelfCircle : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.NOT_AIM_SELF_CIRCLE;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,Vector2.Zero,aimedTarget.m_position);
            }
        }
        class SPG_NotAimSelfRect : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.NOT_AIM_SELF_RECT;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,aimedTarget.m_position-self.m_position,Vector2.Zero);
            }
        }
        class SPG_NotAimSelfSector : SkillParamGeneratorBase{
            public override SkillAimType m_AimType{ get {return SkillAimType.NOT_AIM_SELF_SECTOR;}}
            public override SkillParam GetSkillParam(E_Unit self, E_Unit aimedTarget){
                return new SkillParam(m_AimType,aimedTarget,aimedTarget.m_position-self.m_position,Vector2.Zero);
            }
        }
        
    }
}