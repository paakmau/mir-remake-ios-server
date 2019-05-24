using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.GameLogic {
    partial class GL_BattleSettle : GameLogicBase {
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
            protected bool TryGetAimParamValue (IReadOnlyList<ValueTuple<SkillAimParamType, float>> parmList, SkillAimParamType type, out float value) {
                for (int i=0; i<parmList.Count; i++)
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
            private void GetNearestUnits (Vector2 center, byte num, List<E_ActorUnit> resList) {
                if (resList.Count <= num) return;
                resList.Sort ((E_ActorUnit a, E_ActorUnit b) => {
                    var disA = (a.m_position - center).LengthSquared ();
                    var disB = (b.m_position - center).LengthSquared ();
                    if (disA < disB) return -1;
                    if (disA > disB) return 1;
                    return 0;
                });
                resList.RemoveRange (num, resList.Count);
            }
            protected void GetActorUnitsInCircleRange (E_ActorUnit self, Vector2 center, float radius, CampType targetCamp, byte unitNum, List<E_ActorUnit> resList) {
                resList.Clear ();
                var unitInSightList = EM_Sight.s_instance.GetRawActorUnitsInSightByNetworkId (self.m_networkId);
                for (int i = 0; i < unitInSightList.Count; i++) {
                    // 若阵营不匹配
                    if (EM_Camp.s_instance.GetCampType (self, unitInSightList[i]) != targetCamp) continue;
                    // 若在范围之外
                    if ((self.m_position - unitInSightList[i].m_position).LengthSquared () > radius * radius) continue;
                    resList.Add (unitInSightList[i]);
                }
                GetNearestUnits (center, unitNum, resList);
            }
            public virtual void Reset (CampType targetCamp, byte targetNum, IReadOnlyList<ValueTuple<SkillAimParamType, float>> parm) {
                m_targetCamp = targetCamp;
                m_targetNumber = targetNum;
            }
            public abstract void GetEffectTargets (E_ActorUnit self, SkillParam parm, List<E_ActorUnit> resList);
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
            public override void GetEffectTargets (E_ActorUnit self, SkillParam parm, List<E_ActorUnit> resList) {
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
            public override void GetEffectTargets (E_ActorUnit self, SkillParam parm, List<E_ActorUnit> resList) {
                GetActorUnitsInCircleRange (self, self.m_position, m_radius, m_targetCamp, m_targetNumber, resList);
            }
        }
        private class TargetStage {
            private Dictionary<SkillAimType, EffectTargetChooserBase> m_targetChooserDict = new Dictionary<SkillAimType, EffectTargetChooserBase> ();
            private List<E_ActorUnit> m_targetList = new List<E_ActorUnit> ();
            public TargetStage () {
                // TODO: 用反射, 并全部写完
                m_targetChooserDict.Add (SkillAimType.AIM_CIRCLE, new ETC_AimCircle ());
                m_targetChooserDict.Add (SkillAimType.NOT_AIM_SELF_CIRCLE, new ETC_NotAimSelfCircle ());
            }
            public IReadOnlyList<E_ActorUnit> GetTargetList (E_ActorUnit self, DE_Skill skillDe, DE_SkillData skillDataDe, SkillParam skillParm) {
                EffectTargetChooserBase targetChooser = m_targetChooserDict[skillDe.m_skillAimType];
                targetChooser.Reset (skillDe.m_targetCamp, skillDataDe.m_targetNumber, skillDataDe.m_damageParamList);
                targetChooser.GetEffectTargets (self, skillParm, m_targetList);
                return m_targetList;
            }
        }
    }
}