using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    class GL_Effect : GameLogicBase {
        public static GL_Effect s_instance;
        private List<int> t_intList = new List<int> ();
        private float deltaTimeAfterLastSecond = 0f;
        public GL_Effect (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            // 移除超时的状态
            var statusEn = EM_Status.s_instance.GetStatusEn ();
            while (statusEn.MoveNext ()) {
                int netId = statusEn.Current.Key;
                E_ActorUnit unit = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
                t_intList.Clear ();
                for (int i = 0; i < statusEn.Current.Value.Count; i++) {
                    if (MyTimer.CheckTimeUp (statusEn.Current.Value[i].m_endTime)) {
                        t_intList.Add (i);
                        StatusToAttr (unit, statusEn.Current.Value[i], -1);
                    }
                }
                EM_Status.s_instance.RemoveOrderedStatus (netId, t_intList);
            }
            // 根据状态处理具体属性的每秒变化
            deltaTimeAfterLastSecond += dT;
            if (deltaTimeAfterLastSecond >= 1.0f) {
                deltaTimeAfterLastSecond -= 1.0f;
                var en = EM_Sight.s_instance.GetActorUnitVisibleEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.m_IsDead)
                        continue;
                    int newHP = en.Current.m_CurHp + en.Current.m_DeltaHpPerSecond;
                    int newMP = en.Current.m_CurMp + en.Current.m_DeltaMpPerSecond;
                    en.Current.m_CurHp = Math.Max (Math.Min (newHP, en.Current.m_MaxHp), 0);
                    en.Current.m_CurMp = Math.Max (Math.Min (newMP, en.Current.m_MaxMp), 0);
                }
            }
        }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            EM_Status.s_instance.InitCharacterStatus (netId);
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Status.s_instance.RemoveCharacterStatus (netId);
        }

        private void StatusToAttr (E_ActorUnit unit, E_Status status, int k) {
            // 处理具体属性
            var cAttrList = status.m_dataEntity.m_affectAttributeList;
            for (int i = 0; i < cAttrList.Count; i++)
                unit.AddConAttr (cAttrList[i].Item1, (int) (cAttrList[i].Item2 * status.m_value * k));
            // 处理特殊属性
            var sAttrList = status.m_dataEntity.m_specialAttributeList;
            for (int i = 0; i < sAttrList.Count; i++)
                unit.AddSpAttr (sAttrList[i], k);
            // TODO: 通知Client
        }
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
                var statusList = EM_Status.s_instance.AttachStatus (target.m_networkId, effect.m_statusIdAndValueAndTimeAndCasterNetIdArr);
                for (int i = 0; i < statusList.Count; i++)
                    StatusToAttr (target, statusList[i], 1);
            }
        }
    }
}