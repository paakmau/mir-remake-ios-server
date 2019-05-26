using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    class GL_Effect : GameLogicBase {
        struct Effect {
            public bool m_hit;
            public bool m_critical;
            public int m_deltaHp;
            public int m_deltaMp;
            public ValueTuple<short, float, float, int>[] m_statusIdAndValueAndTimeAndCasterNetIdArr;
            public void InitWithCasterAndTarget (DE_Effect effectDe, E_ActorUnit caster, E_ActorUnit target) {
                // 处理命中
                float hitRate = effectDe.m_hitRate * caster.m_HitRate / target.m_DodgeRate;
                m_hit = MyRandom.NextInt (1, 101) >= hitRate;
                if (m_hit) {
                    // 处理基础伤害(或能量剥夺)
                    m_deltaHp = effectDe.m_deltaHp;
                    m_deltaMp = effectDe.m_deltaMp;
                    switch (effectDe.m_type) {
                        case EffectType.PHYSICS:
                            m_deltaHp *= caster.m_Attack / target.m_Defence;
                            break;
                        case EffectType.MAGIC:
                            m_deltaHp *= caster.m_Magic / target.m_Resistance;
                            m_deltaMp *= caster.m_Magic / target.m_Resistance;
                            break;
                    }
                    // 处理暴击
                    float criticalRate = effectDe.m_criticalRate * caster.m_CriticalRate / target.m_DodgeRate;
                    m_critical = MyRandom.NextInt (1, 101) >= criticalRate;
                    if (m_critical)
                        m_deltaHp *= caster.m_CriticalBonus;
                    // 处理状态
                    m_statusIdAndValueAndTimeAndCasterNetIdArr = new ValueTuple<short, float, float, int>[effectDe.m_statusIdAndValueAndTimeList.Count];
                    for (int i = 0; i < effectDe.m_statusIdAndValueAndTimeList.Count; i++) {
                        var info = effectDe.m_statusIdAndValueAndTimeList[i];
                        float value = info.Item2 / target.m_Tenacity;
                        float durationTime = info.Item3 / target.m_Tenacity;
                        m_statusIdAndValueAndTimeAndCasterNetIdArr[i] = new ValueTuple<short, float, float, int> (info.Item1, value, durationTime, caster.m_networkId);
                    }
                }
            }
        }
        private List<int> t_intList = new List<int> ();
        public GL_Effect (INetworkService netService) : base (netService) {
            Messenger.AddListener<DE_Effect, E_ActorUnit, E_ActorUnit> ("NotifyApplyEffect", NotifyApplyEffect);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        /// <summary>
        /// 对目标的属性添加影响
        /// </summary>
        /// <param name="target"></param>
        public void NotifyApplyEffect (DE_Effect effectDe, E_ActorUnit caster, E_ActorUnit target) {
            Effect effect = new Effect ();
            effect.InitWithCasterAndTarget (effectDe, caster, target);
            if (effect.m_hit) {
                target.m_CurHp += effect.m_deltaHp;
                target.m_CurMp += effect.m_deltaMp;
                // 通知状态
                Messenger.Broadcast<E_ActorUnit, ValueTuple<short, float, float, int>[]> ("NotifyAddStatus", target, effect.m_statusIdAndValueAndTimeAndCasterNetIdArr);
            }
        }
    }
}