using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    partial class GL_BattleSettle {
        /// <summary>
        /// 技能目标选择器  
        /// 目标阵营: 友方, 敌方  
        /// 瞄准类型有: 自身, 其他Unit, 非锁定型  
        /// 范围类型有: 直线型, 圆型  
        /// 目标数量  
        /// </summary>
        private abstract class EffectTargetChooserBase {
            // 技能释放类型
            public abstract SkillAimType m_TargetAimType { get; }
            // 技能作用目标的阵营
            public CampType m_targetCamp;
            // 作用对象数量
            public byte m_targetNumber;
            protected bool TryGetAimParamValue (IReadOnlyList < (SkillAimParamType, float) > parmList, SkillAimParamType type, out float value) {
                for (int i = 0; i < parmList.Count; i++)
                    if (parmList[i].Item1 == type) {
                        value = parmList[i].Item2;
                        return true;
                    }
                value = 0.0f;
                return false;
            }
            /// <summary>
            /// 将unitList进行排序, 并将距离最远的超过作用目标数量的单位剔除
            /// </summary>
            private void GetNearestUnits (Vector2 center, byte num, List<E_Unit> resList) {
                if (resList.Count <= num) return;
                resList.Sort ((E_Unit a, E_Unit b) => {
                    var disA = (a.m_position - center).LengthSquared ();
                    var disB = (b.m_position - center).LengthSquared ();
                    if (disA < disB) return -1;
                    if (disA > disB) return 1;
                    return 0;
                });
                resList.RemoveRange (num, resList.Count);
            }
            protected void GetActorUnitsInCircleRange (E_Unit self, Vector2 center, float radius, CampType targetCamp, byte unitNum, List<E_Unit> resList) {
                resList.Clear ();
                var sight = EM_Sight.s_instance.GetCharacterRawSight (self.m_networkId);
                for (int i = 0; i < sight.Count; i++) {
                    // 若阵营不匹配
                    if (EM_Camp.s_instance.GetCampType (self, sight[i]) != targetCamp) continue;
                    // 若在范围之外
                    if ((self.m_position - sight[i].m_position).LengthSquared () > radius * radius) continue;
                    resList.Add (sight[i]);
                }
                GetNearestUnits (center, unitNum, resList);
            }
            protected void GetActorUnitsInRectRange (E_Unit self, Vector2 basePoint, Vector2 dir, float length, float width, CampType targetCamp, byte unitNum, List<E_Unit> resList) {
                resList.Clear ();
                var sight = EM_Sight.s_instance.GetCharacterRawSight (self.m_networkId);
                for (int i = 0; i < sight.Count; i++) {
                    // 若阵营不匹配
                    if (EM_Camp.s_instance.GetCampType (self, sight[i]) != targetCamp) continue;
                    // 若在范围之外
                    var dirNor = Vector2.Normalize (dir);
                    var dist = sight[i].m_position - basePoint;
                    float l = Vector2.Dot (dirNor, dist);
                    float w = (dirNor * dist).Length () * 2;
                    if (l < 0 || l > length || w > width) continue;
                    resList.Add (sight[i]);
                }
                GetNearestUnits (basePoint, unitNum, resList);
            }
            public virtual void Reset (CampType targetCamp, byte targetNum, IReadOnlyList<ValueTuple<SkillAimParamType, float>> parm) {
                m_targetCamp = targetCamp;
                m_targetNumber = targetNum;
            }
            public abstract void GetEffectTargets (E_Unit self, SkillParam parm, List<E_Unit> resList);
        }
        /// <summary>
        /// 指向性圆形溅射
        /// </summary>
        private class ETC_AimCircle : EffectTargetChooserBase {
            public override SkillAimType m_TargetAimType { get { return SkillAimType.AIM_CIRCLE; } }
            // 伤害半径
            public float m_radius;
            public override void Reset (CampType targetCamp, byte targetNum, IReadOnlyList<ValueTuple<SkillAimParamType, float>> parmList) {
                base.Reset (targetCamp, targetNum, parmList);
                TryGetAimParamValue (parmList, SkillAimParamType.RADIUS, out m_radius);
            }
            public override void GetEffectTargets (E_Unit self, SkillParam parm, List<E_Unit> resList) {
                if (m_targetNumber == 1) {
                    resList.Clear ();
                    resList.Add (parm.m_target);
                } else {
                    GetActorUnitsInCircleRange (self, parm.m_target.m_position, m_radius, m_targetCamp, m_targetNumber, resList);
                }
            }
        }
        /// <summary>
        /// 非指向性自身出发圆形溅射
        /// </summary>
        private class ETC_NotAimSelfCircle : EffectTargetChooserBase {
            public override SkillAimType m_TargetAimType { get { return SkillAimType.NOT_AIM_SELF_CIRCLE; } }
            // 伤害半径
            public float m_radius;
            public override void Reset (CampType targetCamp, byte targetNum, IReadOnlyList<ValueTuple<SkillAimParamType, float>> parmList) {
                base.Reset (targetCamp, targetNum, parmList);
                TryGetAimParamValue (parmList, SkillAimParamType.RADIUS, out m_radius);
            }
            public override void GetEffectTargets (E_Unit self, SkillParam parm, List<E_Unit> resList) {
                GetActorUnitsInCircleRange (self, self.m_position, m_radius, m_targetCamp, m_targetNumber, resList);
            }
        }
        /// <summary>
        /// 非指向性圆形溅射
        /// </summary>
        private class ETC_NotAimCircle : EffectTargetChooserBase {
            public override SkillAimType m_TargetAimType { get { return SkillAimType.NOT_AIM_CIRCLE; } }
            // 伤害半径
            public float m_radius;
            public override void Reset (CampType targetCamp, byte targetNum, IReadOnlyList<ValueTuple<SkillAimParamType, float>> parmList) {
                base.Reset (targetCamp, targetNum, parmList);
                TryGetAimParamValue (parmList, SkillAimParamType.RADIUS, out m_radius);
            }
            public override void GetEffectTargets (E_Unit self, SkillParam parm, List<E_Unit> resList) {
                GetActorUnitsInCircleRange (self, parm.m_position, m_radius, m_targetCamp, m_targetNumber, resList);
            }
        }
        /// <summary>
        /// 非指向性自身出发条形
        /// </summary>
        private class ETC_NotAimSelfRect : EffectTargetChooserBase {
            public override SkillAimType m_TargetAimType { get { return SkillAimType.NOT_AIM_SELF_RECT; } }
            // 伤害长度
            public float m_length;
            // 伤害宽度
            public float m_width;
            public override void Reset (CampType targetCamp, byte targetNum, IReadOnlyList<ValueTuple<SkillAimParamType, float>> parmList) {
                base.Reset (targetCamp, targetNum, parmList);
                TryGetAimParamValue (parmList, SkillAimParamType.LENGTH, out m_length);
                TryGetAimParamValue (parmList, SkillAimParamType.WIDTH, out m_width);
            }
            public override void GetEffectTargets (E_Unit self, SkillParam parm, List<E_Unit> resList) {
                GetActorUnitsInRectRange (self, self.m_position, parm.m_direction, m_length, m_width, m_targetCamp, m_targetNumber, resList);
            }
        }
    }
}