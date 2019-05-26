using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
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
    struct SkillParam {
        public SkillAimType m_aimType;
        /// <summary>
        /// 技能的选定作用目标
        /// </summary>
        public E_ActorUnit m_target;
        public Vector2 m_direction;
        public Vector2 m_position;
        public SkillParam (SkillAimType aimType, E_ActorUnit target, Vector2 direciton, Vector2 position) {
            m_aimType = aimType;
            m_target = target;
            m_direction = direciton;
            m_position = position;
        }
        public NO_SkillParam GetNo () {
            return new NO_SkillParam (m_target.m_networkId, m_direction, m_position);
        }
    }
}