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
    class STC_AimCircle : SkillTargetChooserBase {
        // 技能释放点类型
        public override SkillAimType m_TargetAimType { get { return SkillAimType.AIM_CICLE; } }
        // 伤害半径
        public float m_radius;
        public STC_AimCircle (CampType targetCamp, byte targetNum, float castRange, KeyValuePair<SkillAimParamType, float>[] param) : base (targetCamp, targetNum, castRange) {
            TryGetValue (param, SkillAimParamType.RADIUS, out m_radius);
        }
        public override SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget, SkillParam parm) {
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
}