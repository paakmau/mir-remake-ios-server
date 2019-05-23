using System;
using System.Numerics;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    struct Effect {
        public int m_casterNetworkId;
        public EffectType m_type;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHp;
        public int m_deltaMp;
        public ValueTuple<short, ValueTuple<float, float>>[] m_statusIdAndValueAndTimeArray;
        public int m_StatusAttachNum { get { return m_statusIdAndValueAndTimeArray.Length; } }
        public Effect (DE_Effect effectDe, int casterNetId) {
            m_casterNetworkId = casterNetId;
            m_type = effectDe.m_type;
            m_hitRate = effectDe.m_hitRate;
            m_criticalRate = effectDe.m_criticalRate;
            m_deltaHp = effectDe.m_deltaHP;
            m_deltaMp = effectDe.m_deltaMP;
            List<int> res = new List<int> ();
            m_statusIdAndValueAndTimeArray = new ValueTuple<short, ValueTuple<float, float>>[effectDe.m_statusIdAndValueAndTimeList.Count];
            for (int i = 0; i < effectDe.m_statusIdAndValueAndTimeList.Count; i++)
                m_statusIdAndValueAndTimeArray[i] = effectDe.m_statusIdAndValueAndTimeList[i];
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