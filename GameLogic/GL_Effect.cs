using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    class GL_Effect : GameLogicBase {
        public static GL_Effect s_instance;
        private List<int> t_intList = new List<int> ();
        private float deltaTimeAfterLastSecond = 0f;
        public GL_Effect (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        /// <summary>
        /// 对目标的属性添加影响
        /// </summary>
        /// <param name="target"></param>
        public void NotifyApplyEffect (DE_Effect effectDe, E_Unit caster, E_Unit target) {
            if (target.m_IsDead) return;
            Effect effect = new Effect ();
            effect.InitWithCasterAndTarget (effectDe, caster, target);
            // Client
            m_networkService.SendServerCommand (SC_ApplyAllEffect.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (target.m_networkId, true),
                target.m_networkId,
                effect.GetNo ()));
            // 其他模块
            if (effect.m_hit) {
                // Hp Mp 状态
                GL_UnitBattleAttribute.s_instance.NotifyHpAndMpChange (target, caster, effect.m_deltaHp, effect.m_deltaMp);
                GL_UnitBattleAttribute.s_instance.NotifyAttachStatus (target, caster, effect.m_statusIdAndValueAndTimeArr);
            }
        }
        struct Effect {
            private DE_Effect m_de;
            public bool m_hit;
            public bool m_critical;
            public int m_deltaHp;
            public int m_deltaMp;
            public ValueTuple<short, float, float>[] m_statusIdAndValueAndTimeArr;
            public void InitWithCasterAndTarget (DE_Effect effectDe, E_Unit caster, E_Unit target) {
                m_de = effectDe;
                // xjb计算命中
                float hitRate = effectDe.m_hitRate * caster.m_HitRate / target.m_DodgeRate;
                m_hit = MyRandom.NextInt (1, 101) <= hitRate;
                if (m_hit) {
                    // xjb计算基础伤害(或能量剥夺)
                    m_deltaHp = effectDe.m_deltaHp;
                    m_deltaMp = effectDe.m_deltaMp;
                    switch (effectDe.m_type) {
                        case EffectType.PHYSICS:
                            m_deltaHp = (int) ((float) m_deltaHp * (float) caster.m_Attack / (float) target.m_Defence);
                            break;
                        case EffectType.MAGIC:
                            m_deltaHp = (int) ((float) m_deltaHp * (float) caster.m_Magic / (float) target.m_Resistance);
                            m_deltaMp = (int) ((float) m_deltaMp * (float) caster.m_Magic / (float) target.m_Resistance);
                            break;
                    }
                    // xjb计算暴击
                    float criticalRate = effectDe.m_criticalRate * 0.01f * caster.m_CriticalRate / target.m_DodgeRate;
                    m_critical = MyRandom.NextInt (1, 101) <= criticalRate;
                    if (m_critical)
                        m_deltaHp = (int) (m_deltaHp * (1f + (float) caster.m_CriticalBonus * 0.01f));
                    // xjb计算状态
                    m_statusIdAndValueAndTimeArr = new (short, float, float) [effectDe.m_statusIdAndValueAndTimeList.Count];
                    for (int i = 0; i < effectDe.m_statusIdAndValueAndTimeList.Count; i++) {
                        var info = effectDe.m_statusIdAndValueAndTimeList[i];
                        float value = info.Item2 / target.m_Tenacity;
                        float durationTime = info.Item3 / target.m_Tenacity;
                        m_statusIdAndValueAndTimeArr[i] = (info.Item1, value, durationTime);
                    }
                }
            }
            public NO_Effect GetNo () {
                return new NO_Effect (m_de.m_animId, m_hit, m_critical, m_deltaHp, m_deltaMp);
            }
        }
    }
}