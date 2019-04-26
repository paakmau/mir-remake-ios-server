/**
 * Enity，战斗单元实体
 * 创建者 fn
 * 时间 2019/4/1
 * 最后修改者 yuk
 * 时间 2019/4/3
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
namespace MirRemake {
    abstract class E_ActorUnit {
        // 等级
        protected short m_level;
        public short m_Level { get { return this.m_level; } }
        // 单位种类 (玩家, 怪物, NPC随从)
        public virtual ActorUnitType m_ActorUnitType { get; }
        // 具体属性
        public Dictionary<ActorUnitConcreteAttributeType, int> m_concreteAttributeDict = new Dictionary<ActorUnitConcreteAttributeType, int> ();
        public int m_MaxHP { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_HP]; } }
        public int m_MaxMP { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAX_MP]; } }
        public int m_CurHP {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_HP]; }
            protected set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_HP] = value; }
        }
        public int m_CurMP {
            get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_MP]; }
            protected set { m_concreteAttributeDict[ActorUnitConcreteAttributeType.CURRENT_MP] = value; }
        }
        public int m_DeltaHPPerSecond { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND]; } }
        public int m_DeltaMPPerSecond { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND]; } }
        public int m_Attack { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.ATTACK]; } }
        public int m_Magic { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.MAGIC]; } }
        public int m_Defence { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DEFENCE]; } }
        public int m_Resistance { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.RESISTANCE]; } }
        public int m_Tenacity { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.TENACITY]; } }
        public int m_Speed { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.SPEED]; } }
        public int m_CriticalRate { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_RATE]; } }
        public int m_CriticalBonus { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.CRITICAL_BONUS]; } }
        public int m_HitRate { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.HIT_RATE]; } }
        public int m_DodgeRate { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.DODGE_RATE]; } }
        public bool m_IsFaint { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.FAINT] > 0; } }
        public bool m_IsSilent { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.SILENT] > 0; } }
        public bool m_IsImmobile { get { return m_concreteAttributeDict[ActorUnitConcreteAttributeType.IMMOBILE] > 0; } }
        // 自身状态列表
        protected List<E_Status> m_statusList = new List<E_Status> ();
        /// <summary>
        /// 当前位置
        /// </summary>
        private Vector2 m_position;
        public Vector2 m_Position { get { return m_position; } }

        public void AttachStatus (E_Status status) {
            m_statusList.Add(status);
            foreach (var item in status.m_affectAttributeDict)
                m_concreteAttributeDict[item.Key] += item.Value * status.m_value;
        }
        public void RemoveStatus (E_Status status) {
            m_statusList.Remove(status);
            foreach (var item in status.m_affectAttributeDict)
                m_concreteAttributeDict[item.Key] -= item.Value * status.m_value;
        }
        protected float deltaTimeAfterLastSecond = 0f;
        public void Tick (float dT) {
            // 处理具体属性的每秒变化
            deltaTimeAfterLastSecond += dT;
            while (deltaTimeAfterLastSecond >= 1.0f) {
                deltaTimeAfterLastSecond -= 1.0f;
                int newHP = m_CurHP + m_DeltaHPPerSecond;
                int newMP = m_CurMP + m_DeltaMPPerSecond;
                m_CurHP = Mathf.Max (Mathf.Min (newHP, m_MaxHP), 0);
                m_CurMP = Mathf.Max (Mathf.Min (newMP, m_MaxMP), 0);
            }

            // 移除超时的状态
            for (int i = m_statusList.Count - 1; i >= 0; i--) {
                m_statusList[i].Tick(dT);
                if (m_statusList[i].m_leftTime <= 0.0f)
                    RemoveStatus(m_statusList[i]);
            }
        }

        public void SetPosition (Vector2 pos) {
            m_position = pos;
        }
        /// <summary>
        /// 计算技能效果
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="targets"></param>
        public void ApplyCastSkill (E_Skill skill, List<E_ActorUnit> targets) {
            // 计算初始Effect
            E_Effect initEffect = skill.m_skillEffect.GetClone ();
            CalculateCastEffect (initEffect);
            foreach (var tar in targets)
                tar.CalculateAndApplyEffect (initEffect);
        }
        public void ApplyActiveEnterFSMState(FSMActiveEnterState state) {
        }
        /// <summary>
        /// 被施加Effect后, 计算出最终伤害与状态变化, 施加到自身
        /// 会修改传入的Effect
        /// </summary>
        /// <param name="initEffect">被施加到自身的Effect, 会被修改</param>
        /// <returns>把Effect返回(同一个对象)</returns>
        public void CalculateAndApplyEffect (E_Effect initEffect) {
            // 根据自身属性计算最终Effect
            bool hit = CalculateApplyEffect (initEffect);
            if (hit) {
                // 应用到自身属性上
                ApplyEffectToAttributes (initEffect);
                // 附加状态并应用到具体属性与FSM中
                if (initEffect.m_statusAttachArray != null) {
                    m_statusList.AddRange (initEffect.m_statusAttachArray);
                    foreach (var status in initEffect.m_statusAttachArray)
                        AttachStatus (status);
                }
                // 播放Effect特效
            } else {
                // 播放Miss动画
            }
        }
        /// <summary>
        /// 传入的Effect会被修改
        /// </summary>
        /// <param name="effect">技能或道具的原始Effect</param>
        public void CalculateCastEffect (E_Effect effect) {
            // 处理初始命中与暴击
            effect.m_hitRate *= m_HitRate;
            effect.m_criticalRate *= m_CriticalRate;
            // 处理初始伤害(或能量剥夺)
            switch (effect.m_deltaHPType) {
                case EffectDeltaHPType.PHYSICS:
                    effect.m_deltaHP *= m_Attack;
                    break;
                case EffectDeltaHPType.MAGIC:
                    effect.m_deltaHP *= m_Magic;
                    break;
            }
            switch (effect.m_deltaMPType) {
                case EffectDeltaMPType.MAGIC:
                    effect.m_deltaMP *= m_Magic;
                    break;
            }
            CalculateByHPStrategy (effect, effect.m_selfHPStrategy);
        }
        /// <summary>
        /// 传入的Effect会被修改, 不会被应用到自己身上
        /// </summary>
        /// <param name="effect">由释放者计算后的Effect</param>
        /// <returns>命中则返回true, 否则返回false</returns>
        public bool CalculateApplyEffect (E_Effect effect) {
            effect.m_hitRate /= m_DodgeRate;
            Random randObj = new Random(DateTime.Now.Millisecond);
            if (randObj.Next (1, 101) >= effect.m_hitRate)
                // 未命中
                return false;
            effect.m_criticalRate /= m_DodgeRate;
            // 是否暴击
            bool isCritical = (randObj.Next (1, 101) <= effect.m_criticalRate);

            switch (effect.m_deltaHPType) {
                case EffectDeltaHPType.PHYSICS:
                    effect.m_deltaHP /= m_Defence;
                    break;
                case EffectDeltaHPType.MAGIC:
                    effect.m_deltaHP /= m_Resistance;
                    break;
            }
            switch (effect.m_deltaMPType) {
                case EffectDeltaMPType.MAGIC:
                    effect.m_deltaMP /= m_Resistance;
                    break;
            }
            CalculateByHPStrategy (effect, effect.m_targetHPStrategy);

            // 计算异常状态持续时间(根据韧性)
            if (effect.m_statusAttachArray != null)
                for (int i=0; i<effect.m_statusAttachArray.Length; i++)
                    if (effect.m_statusAttachArray[i].m_type == StatusType.DEBUFF)
                        effect.m_statusAttachArray[i].m_leftTime -= (100 - m_Tenacity) * 0.01f;

            // 处理暴击
            if (isCritical)
                effect.m_deltaHP = (int) (effect.m_deltaHP * 1.5f);
            return true;
        }
        /// <summary>
        /// 把计算好的effect应用到自己身上
        /// </summary>
        /// <param name="effect"></param>
        public void ApplyEffectToAttributes (E_Effect effect) {
            int newHP = m_CurHP + effect.m_deltaHP;
            int newMP = m_CurMP + effect.m_deltaMP;
            m_CurHP = Mathf.Max (Mathf.Min (newHP, m_MaxHP), 0);
            m_CurMP = Mathf.Max (Mathf.Min (newMP, m_MaxMP), 0);
        }

        protected void CalculateByHPStrategy (E_Effect effect, HPStrategy hPStrategy) {
            if (hPStrategy == null) return;
            float value = 0;
            foreach (var item in hPStrategy.strategyFactorDict)
                value += m_concreteAttributeDict[item.Key] * item.Value;
            effect.m_deltaHP += (int) value;
        }

        /// <summary>
        /// 死亡之后掉落物品的接口
        /// </summary>
        /// <returns>掉落的物品</returns>
        public virtual List<E_Item> DropLegacy () {
            return null;
        }

        public int m_networkId;
    }
}