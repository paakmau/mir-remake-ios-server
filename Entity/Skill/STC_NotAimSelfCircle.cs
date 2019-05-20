using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 技能目标选择器  
    /// 目标阵营: 友方, 敌方  
    /// 瞄准类型有: 自身, 其他Unit, 非锁定型  
    /// 范围类型有: 直线型, 圆型  
    /// 目标数量  
    /// </summary>
    class STC_NotAimSelfCircle : SkillTargetChooserBase {
        // 技能释放点类型
        public override SkillAimType m_TargetAimType { get { return SkillAimType.NOT_AIM_SELF_CIRCLE; } }
        // 伤害半径
        public float m_radius;
        public STC_NotAimSelfCircle (CampType targetCamp, byte targetNum, float castRange, KeyValuePair<SkillAimParamType, float>[] param) : base (targetCamp, targetNum, castRange) {
            TryGetValue (param, SkillAimParamType.RADIUS, out m_radius);
        }
        public override SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget, SkillParam parm) {
            if (aimedTarget != null && !parm.m_isValid)
                return new SkillParam (m_TargetAimType, null, Vector2.zero, aimedTarget.m_Position);
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
}