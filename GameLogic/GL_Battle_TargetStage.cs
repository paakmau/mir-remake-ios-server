using System.Collections.Generic;
using UnityEngine;

using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Battle {
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
        /// <summary>
        /// 技能目标选择器  
        /// 目标阵营: 友方, 敌方  
        /// 瞄准类型有: 自身, 其他Unit, 非锁定型  
        /// 范围类型有: 直线型, 圆型  
        /// 目标数量  
        /// </summary>
        abstract class SkillTargetChooserBase {
            // 技能作用目标的阵营
            public CampType m_targetCamp;
            // 技能释放点类型
            public abstract SkillAimType m_TargetAimType { get; }
            // 作用对象数量
            public byte m_targetNumber;
            // 射程
            public float m_castRange;
            public SkillTargetChooserBase (CampType targetCamp, byte targetNum, float castRange) {
                m_targetCamp = targetCamp;
                m_targetNumber = targetNum;
                m_castRange = castRange;
            }
            /// <summary>
            /// 完善技能参数
            /// 例如自动选择最近的目标等, 看甲方
            /// </summary>
            /// <returns></returns>
            public abstract SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget);
            /// <summary>
            /// 检查是否在射程之内
            /// </summary>
            /// <returns></returns>
            public abstract bool InRange (Vector2 pos, SkillParam parm);
            public abstract List<E_ActorUnit> GetEffectTargets (E_ActorUnit self, SkillParam parm);
            protected bool TryGetValue (KeyValuePair<SkillAimParamType, float>[] param, SkillAimParamType type, out float value) {
                foreach (var item in param)
                    if (item.Key == type) {
                        value = item.Value;
                        return true;
                    }
                value = 0.0f;
                return false;
            }
        }
        class STC_AimCircle : SkillTargetChooserBase {
            public override SkillAimType m_TargetAimType { get { return SkillAimType.AIM_CIRCLE; } }
            // 伤害半径
            public float m_radius;
            public STC_AimCircle (CampType targetCamp, byte targetNum, float castRange, KeyValuePair<SkillAimParamType, float>[] param) : base (targetCamp, targetNum, castRange) {
                TryGetValue (param, SkillAimParamType.RADIUS, out m_radius);
            }
            public override SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget) {
                // 已锁定目标且阵营匹配
                if (aimedTarget != null && SM_ActorUnit.s_instance.CheckCampMatch (self, aimedTarget, m_targetCamp)) {
                    parm.m_target = aimedTarget;
                    return parm;
                } else {
                    // 寻找释放目标
                    List<E_ActorUnit> targetList = SM_ActorUnit.s_instance.GetActorUnitsInCircleRange (self, self.m_Position, 5, m_targetCamp, 1);
                    if (targetList.Count == 1) {
                        parm.m_target = targetList[0];
                        return parm;
                    }
                    return SkillParam.s_invalidSkillParam;
                }
            }
            public override bool InRange (Vector2 pos, SkillParam parm) {
                if ((parm.m_TargetPosition - pos).magnitude <= m_castRange)
                    return true;
                return false;
            }
            public override List<E_ActorUnit> GetEffectTargets (E_ActorUnit self, SkillParam parm) {
                // TODO: 
                return null;
            }
        }
        class STC_NotAimSelfCircle : SkillTargetChooserBase {
            public override SkillAimType m_TargetAimType { get { return SkillAimType.NOT_AIM_SELF_CIRCLE; } }
            // 伤害半径
            public float m_radius;
            public STC_NotAimSelfCircle (CampType targetCamp, byte targetNum, float castRange, KeyValuePair<SkillAimParamType, float>[] param) : base (targetCamp, targetNum, castRange) {
                TryGetValue (param, SkillAimParamType.RADIUS, out m_radius);
            }
            public override SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget, SkillParam parm) {
                if (aimedTarget != null && !parm.m_isValid)
                    return new SkillParam (m_TargetAimType, null, Vector2.zero, aimedTarget.m_position);
                return parm;
            }
            public override bool InRange (Vector2 pos, SkillParam tarPos) {
                return true;
            }
            public override List<E_ActorUnit> GetEffectTargets (E_ActorUnit self, SkillParam parm) {
                // TODO: 
                return null;
            }
        }
        class TargetStage {
            public TargetStage () {

            }
        }
    }
}