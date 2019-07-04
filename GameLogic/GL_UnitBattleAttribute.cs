using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理单位战斗相关属性与状态
    /// </summary>
    class GL_UnitBattleAttribute : GameLogicBase {
        struct Effect {
            private DE_Effect m_de;
            private short m_animId;
            public bool m_hit;
            public bool m_critical;
            public int m_deltaHp;
            public int m_deltaMp;
            public ValueTuple<short, float, float>[] m_statusIdAndValueAndTimeArr;
            public void InitWithCasterAndTarget (DE_Effect effectDe, short animId, E_Unit caster, E_Unit target) {
                m_de = effectDe;
                m_animId = animId;
                // xjb计算命中
                float hitRate = effectDe.m_hitRate * caster.m_HitRate * 0.01f;
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
                    // for(int i=0;i<effectDe.m_attributeArr.Length;i++){
                    //     switch(effectDe.m_attributeArr[i].Item1){
                    //         case ActorUnitConcreteAttributeType.ATTACK:
                    //             m_deltaHp=m_deltaHp+(int)(caster.m_Attack*effectDe.m_attributeArr[i].Item2);
                    //             break;
                    //         case ActorUnitConcreteAttributeType.MAGIC:
                    //             m_deltaHp=m_deltaHp+(int)(caster.m_Magic*effectDe.m_attributeArr[i].Item2);
                    //             break;
                    //         case ActorUnitConcreteAttributeType.MAX_HP:
                    //             m_deltaHp=m_deltaHp+(int)(caster.m_MaxHp*effectDe.m_attributeArr[i].Item2);
                    //             break;
                    //         case ActorUnitConcreteAttributeType.MAX_MP:
                    //             m_deltaHp=m_deltaHp+(int)(caster.m_MaxMp*effectDe.m_attributeArr[i].Item2);
                    //             break;
                    //     }
                    // }
                    // xjb计算暴击 应该ok
                    float criticalRate = effectDe.m_criticalRate * caster.m_CriticalRate * 0.01f;
                    m_critical = MyRandom.NextInt (1, 101) <= criticalRate;
                    if (m_critical)
                        m_deltaHp = (int) (m_deltaHp * (1f + (float) caster.m_CriticalBonus * 0.01f));

                    //减伤+易伤 TODO: 人物属性里没有减伤易伤？
                    if (m_deltaHp < 0) {
                        if (effectDe.m_type == EffectType.PHYSICS) {
                            //m_deltaHp=m_deltaHp*target.m_???
                        }

                    }

                    //仇恨 TODO:
                    if (m_deltaHp < 0) {
                        //target.
                    }

                    // xjb计算状态 TODO: 这里是不是要单独拎出去
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
                return new NO_Effect (m_animId, m_hit, m_critical, m_deltaHp, m_deltaMp);
            }

            //计算护甲和魔法抗性的减伤
            private int getDamage (int damage, int armor) {
                if (armor <= 100) {
                    return (int) (damage * (1 - armor / (armor + 100.0)));
                }
                if (armor <= 100) {
                    return (int) (damage * (0.5 - armor / (armor + 1000.0)));
                }
                return (int) (damage * (0.25 - armor / (armor + 10000.0)));
            }
        }
        public static GL_UnitBattleAttribute s_instance;
        public GL_UnitBattleAttribute (INetworkService netService) : base (netService) { }
        private const float c_isAttackedLastTime = 7.0f;
        private float m_secondTimer = 0;
        public override void Tick (float dT) {
            // 移除超时的状态
            var statusEn = EM_Status.s_instance.GetStatusEn ();
            while (statusEn.MoveNext ()) {
                int netId = statusEn.Current.Key;
                E_Unit unit = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
                if (unit == null) continue;
                var statusToRemoveList = new List<int> ();
                for (int i = 0; i < statusEn.Current.Value.Count; i++) {
                    if (MyTimer.CheckTimeUp (statusEn.Current.Value[i].m_endTime)) {
                        statusToRemoveList.Add (i);
                        StatusChanged (unit, statusEn.Current.Value[i], -1);
                    }
                }
                EM_Status.s_instance.RemoveOrderedStatus (netId, statusToRemoveList);
            }
            // 处理仇恨消失
            var unitEn = EM_Sight.s_instance.GetUnitVisibleEnumerator ();
            while (unitEn.MoveNext ()) {
                // 无伤害信息
                if (unitEn.Current.m_netIdAndDamageDict.Count == 0)
                    continue;
                // 若已死亡
                if (unitEn.Current.m_IsDead) {
                    unitEn.Current.m_netIdAndDamageDict.Clear ();
                    continue;
                }
                // 若不在被攻击状态
                if (MyTimer.CheckTimeUp (unitEn.Current.m_isAttackedTimer)) {
                    unitEn.Current.m_netIdAndDamageDict.Clear ();
                    continue;
                }
                var hatredEn = unitEn.Current.m_netIdAndDamageDict.GetEnumerator ();
                var hTarRemoveList = new List<int> ();
                while (hatredEn.MoveNext ()) {
                    // 仇恨目标下线或死亡
                    var tar = EM_Sight.s_instance.GetUnitVisibleByNetworkId (hatredEn.Current.Key);
                    if (tar == null || tar.m_IsDead) {
                        hTarRemoveList.Add (hatredEn.Current.Key);
                        continue;
                    }
                }
                for (int i = 0; i < hTarRemoveList.Count; i++)
                    unitEn.Current.m_netIdAndDamageDict.Remove (hTarRemoveList[i]);
            }
            // 每秒变化 (每秒回血回蓝)
            m_secondTimer += dT;
            if (m_secondTimer >= 1.0f) {
                m_secondTimer -= 1.0f;
                var en = EM_Sight.s_instance.GetUnitVisibleEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.m_IsDead)
                        continue;
                    int newHP = en.Current.m_curHp + en.Current.m_DeltaHpPerSecond;
                    int newMP = en.Current.m_curMp + en.Current.m_DeltaMpPerSecond;
                    en.Current.m_curHp = Math.Max (Math.Min (newHP, en.Current.m_MaxHp), 0);
                    en.Current.m_curMp = Math.Max (Math.Min (newMP, en.Current.m_MaxMp), 0);
                }
            }
        }
        public override void NetworkTick () {
            var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
            while (charEn.MoveNext ()) {
                var charObj = charEn.Current.Value;
                var sight = EM_Sight.s_instance.GetCharacterRawSight (charObj.m_networkId);
                // 发送 Hp 与 Mp 信息
                var sightNetIdList = new List<int> (sight.Count + 1);
                var hpMaxHpMpMaxMpList = new List < (int, int, int, int) > (sight.Count + 1);
                for (int i = 0; i < sight.Count; i++) {
                    sightNetIdList.Add (sight[i].m_networkId);
                    hpMaxHpMpMaxMpList.Add ((sight[i].m_curHp, sight[i].m_MaxHp, sight[i].m_curMp, sight[i].m_MaxMp));
                }
                sightNetIdList.Add (charObj.m_networkId);
                hpMaxHpMpMaxMpList.Add ((charObj.m_curHp, charObj.m_MaxHp, charObj.m_curMp, charObj.m_MaxMp));
                m_networkService.SendServerCommand (SC_SetAllHPAndMP.Instance (
                    charObj.m_networkId,
                    sightNetIdList,
                    hpMaxHpMpMaxMpList
                ));
                // 发送自身属性
                m_networkService.SendServerCommand (SC_SetSelfConcreteAttribute.Instance (
                    charObj.m_networkId,
                    charObj.m_Attack,
                    charObj.m_Defence,
                    charObj.m_Magic,
                    charObj.m_Resistance
                ));
            }
        }
        public E_Monster[] NotifyInitAllMonster (int[] netIdArr) {
            var mons = EM_Unit.s_instance.InitAllMonster (netIdArr);
            EM_Status.s_instance.InitAllMonster (netIdArr);
            return mons;
        }
        public E_Character NotifyInitCharacter (int netId, int charId) {
            E_Character newChar = EM_Unit.s_instance.InitCharacter (netId, charId);
            // client
            m_networkService.SendServerCommand (SC_InitSelfAttribute.Instance (
                netId,
                newChar.m_Occupation,
                newChar.m_Level,
                newChar.m_experience,
                newChar.m_Strength,
                newChar.m_Intelligence,
                newChar.m_Agility,
                newChar.m_Spirit,
                newChar.m_TotalMainPoint,
                newChar.m_VirtualCurrency,
                newChar.m_ChargeCurrency));
            EM_Status.s_instance.InitCharacterStatus (netId);
            return newChar;
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Unit.s_instance.RemoveCharacter (netId);
            EM_Status.s_instance.RemoveCharacterStatus (netId);
        }
        public void NotifyApplyEffect (DE_Effect effectDe, short animId, E_Unit caster, E_Unit target) {
            if (target.m_IsDead) return;
            Effect effect = new Effect ();
            effect.InitWithCasterAndTarget (effectDe, animId, caster, target);
            // Client
            m_networkService.SendServerCommand (SC_ApplyAllEffect.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (target.m_networkId, true),
                target.m_networkId,
                effect.GetNo ()));
            // 
            if (effect.m_hit) {
                // Hp Mp 状态
                HpAndMpChange (target, caster, effect.m_deltaHp, effect.m_deltaMp);
                AttachStatus (target, caster, effect.m_statusIdAndValueAndTimeArr);
            }
        }
        private void HpAndMpChange (E_Unit target, E_Unit caster, int dHp, int dMp) {
            target.m_curHp += dHp;
            target.m_curMp += dMp;
            if (dHp >= 0 && dMp >= 0) return;

            // 计算伤害量
            int newDmg = -dHp;
            target.m_isAttackedTimer = MyTimer.s_CurTime.Ticked (c_isAttackedLastTime);
            int oriDmg;
            if (!target.m_netIdAndDamageDict.TryGetValue (caster.m_networkId, out oriDmg))
                oriDmg = 0;
            target.m_netIdAndDamageDict[caster.m_networkId] = oriDmg + newDmg;

            // 若单位死亡
            if (target.m_IsDead) {
                target.Dead();
                // client
                m_networkService.SendServerCommand (SC_ApplyAllDead.Instance (
                    EM_Sight.s_instance.GetInSightCharacterNetworkId (target.m_networkId, true),
                    caster.m_networkId,
                    target.m_networkId
                ));
                // log
                if (target.m_UnitType == ActorUnitType.MONSTER && caster.m_UnitType == ActorUnitType.PLAYER)
                    GL_Log.s_instance.NotifyLog (GameLogType.KILL_MONSTER, caster.m_networkId, ((E_Monster) target).m_MonsterId);
                // 通知 CharacterLevel
                if (caster.m_UnitType == ActorUnitType.PLAYER)
                    GL_CharacterAttribute.s_instance.NotifyKillUnit ((E_Character) caster, target);
            }
        }
        private void AttachStatus (E_Unit target, E_Unit caster, ValueTuple<short, float, float>[] statusIdAndValueAndTimeArr) {
            var statusList = EM_Status.s_instance.AttachStatus (target.m_networkId, caster.m_networkId, statusIdAndValueAndTimeArr);
            for (int i = 0; i < statusList.Count; i++)
                StatusChanged (target, statusList[i], 1);
        }
        private void ConcreteAttributeChange (E_Unit target, IReadOnlyList < (ActorUnitConcreteAttributeType, int) > dAttr) {
            for (int i = 0; i < dAttr.Count; i++)
                target.AddBattleConAttr (dAttr[i].Item1, dAttr[i].Item2);
        }
        private void StatusChanged (E_Unit unit, E_Status status, int k) {
            // TODO: StatusChanged GL
            // // 处理具体属性
            // var cAttrList = status.m_dataEntity.m_affectAttributeList;
            // for (int i = 0; i < cAttrList.Count; i++)
            //     unit.AddBattleConAttr (cAttrList[i].Item1, (int) (cAttrList[i].Item2 * status.m_value * k));
            // // 处理特殊属性
            // var sAttrList = status.m_dataEntity.m_specialAttributeList;
            // for (int i = 0; i < sAttrList.Count; i++)
            //     unit.AddSpAttr (sAttrList[i], k);
            // // 通知Client
            // m_networkService.SendServerCommand (SC_ApplyAllStatus.Instance (
            //     EM_Sight.s_instance.GetInSightCharacterNetworkId (unit.m_networkId, true),
            //     unit.m_networkId,
            //     status.GetNo (),
            //     k == 1
            // ));
        }
    }
}