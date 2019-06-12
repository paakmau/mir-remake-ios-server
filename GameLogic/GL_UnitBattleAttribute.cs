using System;
using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理单位战斗相关属性与状态
    /// </summary>
    class GL_UnitBattleAttribute : GameLogicBase {
        public static GL_UnitBattleAttribute s_instance;
        public GL_UnitBattleAttribute (INetworkService netService) : base (netService) { }
        private float m_secondTimer = 0;
        public override void Tick (float dT) {
            // 移除超时的状态
            var statusEn = EM_Status.s_instance.GetStatusEn ();
            while (statusEn.MoveNext ()) {
                int netId = statusEn.Current.Key;
                E_Unit unit = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
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
                var hatredEn = unitEn.Current.m_hatredRefreshDict.GetEnumerator ();
                var hTarRemoveList = new List<int> ();
                while (hatredEn.MoveNext ()) {
                    // 仇恨时间到
                    if (MyTimer.CheckTimeUp (hatredEn.Current.Value)) {
                        hTarRemoveList.Add (hatredEn.Current.Key);
                        continue;
                    }
                    // 仇恨目标下线或死亡
                    var tar = EM_Sight.s_instance.GetUnitVisibleByNetworkId (hatredEn.Current.Key);
                    if (tar == null || tar.m_IsDead) {
                        hTarRemoveList.Add (hatredEn.Current.Key);
                        continue;
                    }
                }
                for (int i = 0; i < hTarRemoveList.Count; i++)
                    unitEn.Current.m_hatredRefreshDict.Remove (hTarRemoveList[i]);
            }
            // 处理具体属性的每秒变化 TODO: 应当用 状态 处理
            m_secondTimer += dT;
            if (m_secondTimer >= 1.0f) {
                m_secondTimer -= 1.0f;
                var en = EM_Sight.s_instance.GetUnitVisibleEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.m_IsDead)
                        continue;
                    int newHP = en.Current.m_CurHp + en.Current.m_DeltaHpPerSecond;
                    int newMP = en.Current.m_CurMp + en.Current.m_DeltaMpPerSecond;
                    en.Current.m_CurHp = Math.Max (Math.Min (newHP, en.Current.m_MaxHp), 0);
                    en.Current.m_CurMp = Math.Max (Math.Min (newMP, en.Current.m_MaxMp), 0);
                    if (en.Current.m_IsDead)
                        // 单位死亡 TODO: 
                        UnitDead (en.Current, en.Current);
                }
            }
        }
        public override void NetworkTick () {
            var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
            while (charEn.MoveNext ()) {
                var charObj = charEn.Current.Value;
                var sight = EM_Sight.s_instance.GetCharacterRawSight (charObj.m_networkId);
                // 发送 Hp 与 Mp 信息
                var sightNetIdList = new List<int> (sight.Count);
                var hpMaxHpMpMaxMpList = new List < (int, int, int, int) > (sight.Count);
                for (int i = 0; i < sight.Count; i++) {
                    sightNetIdList.Add (sight[i].m_networkId);
                    hpMaxHpMpMaxMpList.Add ((sight[i].m_CurHp, sight[i].m_MaxHp, sight[i].m_CurMp, sight[i].m_MaxMp));
                }
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
        public void NotifyHpAndMpChange (E_Unit target, E_Unit caster, int dHp, int dMp) {
            target.m_CurHp += dHp;
            target.m_CurMp += dMp;
            // xjb计算仇恨
            float hatredTime = (float) (-dHp - dMp) / (float) target.m_MaxHp * 10;
            if (hatredTime < 0) return;
            MyTimer.Time oriHatred;
            if (!target.m_hatredRefreshDict.TryGetValue (target.m_networkId, out oriHatred))
                oriHatred = MyTimer.s_CurTime;
            target.m_hatredRefreshDict[target.m_networkId] = oriHatred.Ticked (hatredTime);

            // 若单位死亡
            if (target.m_IsDead)
                UnitDead (target, caster);
        }
        public void NotifyAttachStatus (E_Unit target, E_Unit caster, ValueTuple<short, float, float>[] statusIdAndValueAndTimeArr) {
            var statusList = EM_Status.s_instance.AttachStatus (target.m_networkId, caster.m_networkId, statusIdAndValueAndTimeArr);
            for (int i = 0; i < statusList.Count; i++)
                StatusChanged (target, statusList[i], 1);
        }
        public void NotifyConcreteAttributeChange (E_Unit target, IReadOnlyList < (ActorUnitConcreteAttributeType, int) > dAttr) {
            for (int i = 0; i < dAttr.Count; i++)
                target.AddConAttr (dAttr[i].Item1, dAttr[i].Item2);
        }
        private void UnitDead (E_Unit dead, E_Unit killer) {
            // client
            m_networkService.SendServerCommand (SC_ApplyAllDead.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (dead.m_networkId, true),
                killer.m_networkId,
                dead.m_networkId
            ));
            // 通知Mission
            if (dead.m_UnitType == ActorUnitType.MONSTER && killer.m_UnitType == ActorUnitType.PLAYER)
                GL_Mission.s_instance.ListenMissionTarget ((E_Character) killer, MissionTargetType.KILL_MONSTER, ((E_Monster) dead).m_MonsterId, 1);
        }
        private void StatusChanged (E_Unit unit, E_Status status, int k) {
            // 处理具体属性
            var cAttrList = status.m_dataEntity.m_affectAttributeList;
            for (int i = 0; i < cAttrList.Count; i++)
                unit.AddConAttr (cAttrList[i].Item1, (int) (cAttrList[i].Item2 * status.m_value * k));
            // 处理特殊属性
            var sAttrList = status.m_dataEntity.m_specialAttributeList;
            for (int i = 0; i < sAttrList.Count; i++)
                unit.AddSpAttr (sAttrList[i], k);
            // 通知Client
            m_networkService.SendServerCommand (SC_ApplyAllStatus.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (unit.m_networkId, true),
                unit.m_networkId,
                status.GetNo (),
                k == 1
            ));
        }
    }
}