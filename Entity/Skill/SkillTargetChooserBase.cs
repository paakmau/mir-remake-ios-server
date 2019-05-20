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
        public abstract SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget, SkillParam parm);
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
}