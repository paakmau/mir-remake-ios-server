using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    /// <summary>
    /// 技能目标选择器  
    /// 目标阵营: 友方, 敌方  
    /// 瞄准类型有: 自身, 其他Unit, 非锁定型  
    /// 范围类型有: 直线型, 圆型  
    /// 目标数量  
    /// </summary>
    class SkillTargetChooser : IRangeChecker {
        // 技能作用目标的阵营
        public CampType m_targetCamp;
        // 技能释放点类型
        public SkillAimType m_targetAimType;
        // 作用区域的形状
        public SkillRangeType m_rangeType;
        // 作用对象数量
        public byte m_targetNumber;
        // 射程
        public float m_castRange;
        // 伤害范围(半径或长度)
        public float m_damageRange;
        // 角度
        public short m_damageRadian;
        // 宽度(长方形)
        public float m_damageWidth;

        public SkillTargetChooser () {
            // TODO: 仅用于测试, 日后应当删除
            m_targetCamp = CampType.ENEMY;
            m_targetAimType = SkillAimType.OTHER;
            m_rangeType = SkillRangeType.SECTOR;
            m_targetNumber = 3;
            m_castRange = 0.1f;
            m_damageRange = 1.0f;
            m_damageRadian = 360;
        }
        /// <summary>
        /// 为技能释放选择一个目标(若并未锁定, 或锁定的目标阵营与技能不符合)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="aimedTarget"></param>
        /// <returns>
        /// 若技能为NOT_AIM, 返回null  
        /// 若已锁定目标与技能目标阵容匹配, 返回aimedTarget  
        /// 否则在玩家附近寻找匹配的目标, 若找到一个最近的target, 返回target  
        /// 若找不到, 返回null
        /// </returns>
        // public E_ActorUnit GetCastTargetIfAim (E_ActorUnit self, E_ActorUnit aimedTarget) {
        //     if (m_targetAimType == SkillAimType.NOT_AIM)
        //         return null;
        //     if (m_targetCamp == aimedTarget.m_camp)
        //         return aimedTarget;

        // }
        /// <summary>
        /// 得到技能目标(点)  
        /// 表明角色需要向它移动
        /// </summary>
        /// <param name="self"></param>
        /// <param name="aimedTarget"></param>
        /// <param name="parm"></param>
        /// <returns>
        /// 返回目标(点), 主角需要向目标点移动直到可以释放技能(若在射程之外)  
        /// 若找不到目标点, 返回TargetPosition.s_noTarget
        /// </returns>
        public TargetPosition GetCastTargetPosition (E_ActorUnit self, E_ActorUnit aimedTarget, Vector2 parm) {
            switch (m_targetAimType) {
                case SkillAimType.SELF:
                    // 释放目标为自身
                    return new TargetPosition (self);
                case SkillAimType.OTHER:
                    // 若释放目标为其他Unit
                    if (aimedTarget != null && SM_ActorUnit.s_instance.CheckCampMatch(self, aimedTarget, m_targetCamp))
                        // 已有释放目标且阵营匹配
                        return new TargetPosition (aimedTarget);
                    else {
                        // 寻找释放目标
                        List<E_ActorUnit> targetList = SM_ActorUnit.s_instance.GetActorUnitsInCircleRange (self, self.m_position, 5, m_targetCamp, 1);
                        if (targetList.Count == 1)
                            return new TargetPosition (targetList[0]);
                        return new TargetPosition(null);
                    }
                case SkillAimType.NOT_AIM:
                    if (m_rangeType == SkillRangeType.SECTOR) {
                        if (m_damageRadian == 360)
                            // 不锁定释放目标的圆形范围技能
                            return new TargetPosition (parm);
                        else
                            // 目前非圆扇形技能只能以自身为释放目标(作用目标一般不为自身)
                            return new TargetPosition (self);
                    } else if (m_rangeType == SkillRangeType.LINE)
                        // 目前直线技能只能锁定自身为释放目标(作用目标一般不为自身)
                        return new TargetPosition (self);
                    else break;
            }
            return new TargetPosition(null);
        }
        /// <summary>
        /// 检查是否在射程之内
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tarPos"></param>
        /// <returns></returns>
        public bool CheckInRange (E_ActorUnit self, TargetPosition tarPos) {
            if(m_targetAimType == SkillAimType.SELF) return true;
            if ((tarPos.m_Position - self.m_position).magnitude <= m_castRange)
                return true;
            else return false;
        }
        public List<E_ActorUnit> GetEffectTargets (E_ActorUnit self, TargetPosition tarPos, Vector2 parm) {
            if (m_targetAimType == SkillAimType.SELF) {
                if (m_targetCamp == CampType.SELF)
                    // 仅对自己的技能
                    return new List<E_ActorUnit> { self };
                else switch (m_rangeType) {
                    case SkillRangeType.SECTOR:
                        if (m_damageRadian == 360)
                            // 自己的圆周范围内
                            return SM_ActorUnit.s_instance.GetActorUnitsInCircleRange (self, self.m_position, m_damageRange, m_targetCamp, m_targetNumber);
                        else
                            // 自己出发的扇形
                            return SM_ActorUnit.s_instance.GetActorUnitsInSectorRange (self, self.m_position, parm, m_damageRange, m_damageRadian, m_targetCamp, m_targetNumber);
                    case SkillRangeType.LINE:
                        // 自己出发的直线
                        return SM_ActorUnit.s_instance.GetActorUnitsInLineRange (self, self.m_position, parm, m_damageRange, m_damageWidth, m_targetCamp, m_targetNumber);
                }
            } else if (m_targetAimType == SkillAimType.OTHER)  {
                if (m_targetNumber == 1)
                    // 有释放目标的单体技能
                    return new List<E_ActorUnit> { tarPos.m_Target };
                else {
                    if(m_rangeType == SkillRangeType.SECTOR && m_damageRadian == 360)
                        // 有释放目标的多体(溅射)技能, 且为圆形
                        return SM_ActorUnit.s_instance.GetActorUnitsInCircleRange(self, tarPos.m_Position, m_damageRange, m_targetCamp, m_targetNumber);
                }
            } else if (m_targetAimType == SkillAimType.NOT_AIM) {
                if(m_rangeType == SkillRangeType.SECTOR && m_damageRadian == 360)
                    // 非指向的圆形
                    return SM_ActorUnit.s_instance.GetActorUnitsInCircleRange(self, tarPos.m_Position, m_damageRange, m_targetCamp, m_targetNumber);
            }
            return null;
        }
        /// <summary>
        /// 获取技能释放的方向
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tarPos"></param>
        /// <param name="parm"></param>
        /// <returns>
        /// 返回技能释放的方向
        /// 若为自身发出的圆形技能或无方向技能, 返回Vector2.zero
        /// </returns>
        public Vector2 GetCastDirection(E_ActorUnit self, TargetPosition tarPos, Vector2 parm) {
            if(m_targetAimType == SkillAimType.SELF) {
                // 自身出发的技能
                if(m_rangeType == SkillRangeType.LINE)
                    return parm;
                else if(m_rangeType == SkillRangeType.SECTOR) {
                    if(m_damageRadian == 360)
                        return Vector2.zero;
                    else
                        return parm;
                }
            }else if(m_targetAimType == SkillAimType.OTHER) {
                // 锁定其他人
                return tarPos.m_Position-self.m_position;
            }else if(m_targetAimType == SkillAimType.NOT_AIM) {
                // 非指向性技能
                return tarPos.m_Position-self.m_position;
            }
            return Vector2.zero;
        }
    }
}