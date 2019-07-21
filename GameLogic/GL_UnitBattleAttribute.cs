using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理单位战斗相关属性与状态
    /// </summary>
    partial class GL_UnitBattleAttribute : GameLogicBase {
        public static GL_UnitBattleAttribute s_instance;
        private EffectCalculateStage m_effectStage = new EffectCalculateStage ();
        private Dictionary<StatusType, IStatusHandler> m_statusHandlerDict = new Dictionary<StatusType, IStatusHandler> ();
        private const float c_isAttackedLastTime = 15.0f;
        private float m_secondTimer = 0;
        public GL_UnitBattleAttribute (INetworkService netService) : base (netService) {
            // 实例化所有 StatusHandler 接口的实现类
            var type = typeof (IStatusHandler);
            var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => p.IsClass && type.IsAssignableFrom (p));
            foreach (var impl in implTypes) {
                var sh = impl.GetConstructor (Type.EmptyTypes).Invoke (null) as IStatusHandler;
                m_statusHandlerDict.Add (sh.m_Type, sh);
            }
            // 初始化Status
            var monEn = EM_Unit.s_instance.GetMonsterEn ();
            while (monEn.MoveNext ())
                EM_Status.s_instance.InitUnitStatus (monEn.Current.Key);
        }
        public override void Tick (float dT) {
            // 移除超时的状态
            var allUnitStatusEn = EM_Status.s_instance.GetAllUnitStatusEn ();
            while (allUnitStatusEn.MoveNext ()) {
                int netId = allUnitStatusEn.Current.Key;
                var statusList = allUnitStatusEn.Current.Value;
                E_Unit unit = EM_Sight.s_instance.GetUnitVisibleByNetworkId (netId);
                if (unit == null) continue;
                var statusToRemoveList = new List<int> ();
                for (int i = 0; i < statusList.Count; i++)
                    if (MyTimer.CheckTimeUp (statusList[i].m_endTime))
                        statusToRemoveList.Add (i);
                RemoveStatus (unit, statusToRemoveList, statusList);
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
            // 每秒变化 (每秒回血回蓝) (Status)
            m_secondTimer += dT;
            if (m_secondTimer >= 1.0f) {
                m_secondTimer -= 1.0f;
                unitEn = EM_Sight.s_instance.GetUnitVisibleEnumerator ();
                while (unitEn.MoveNext ()) {
                    if (unitEn.Current.m_IsDead)
                        continue;
                    int newHP = unitEn.Current.m_curHp + unitEn.Current.m_DeltaHpPerSecond;
                    int newMP = unitEn.Current.m_curMp + unitEn.Current.m_DeltaMpPerSecond;
                    unitEn.Current.m_curHp = Math.Max (Math.Min (newHP, unitEn.Current.m_MaxHp), 0);
                    unitEn.Current.m_curMp = Math.Max (Math.Min (newMP, unitEn.Current.m_MaxMp), 0);
                }
                allUnitStatusEn = EM_Status.s_instance.GetAllUnitStatusEn ();
                while (allUnitStatusEn.MoveNext ()) {
                    var netId = allUnitStatusEn.Current.Key;
                    var statusList = allUnitStatusEn.Current.Value;
                    var unit = EM_Sight.s_instance.GetUnitVisibleByNetworkId (netId);
                    if (unit == null) continue;
                    for (int i = 0; i < statusList.Count; i++)
                        TickStatusPerSecond (unit, statusList[i]);
                }
            }
        }
        public override void NetworkTick () {
            var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
            while (charEn.MoveNext ()) {
                var charObj = charEn.Current.Value;
                var sight = EM_Sight.s_instance.GetCharacterRawSight (charObj.m_networkId);
                if (sight == null)
                    continue;
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
            }
        }
        public void NotifyInitCharacter (int netId) {
            EM_Status.s_instance.InitUnitStatus (netId);
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Status.s_instance.RemoveCharacterStatus (netId);
        }
        public void NotifyApplyEffect (DE_Effect effectDe, short animId, E_Unit caster, E_Unit target) {
            if (target.m_IsDead) return;
            m_effectStage.InitWithCasterAndTarget (effectDe, animId, caster, target);
            // Client
            m_networkService.SendServerCommand (SC_ApplyAllEffect.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (target.m_networkId, true),
                target.m_networkId,
                m_effectStage.GetNo ()));
            // 若命中
            if (m_effectStage.m_hit) {
                AttachHatred (target, caster, m_effectStage.m_hatred);
                AttachHpAndMpChange (target, caster, m_effectStage.m_deltaHp, m_effectStage.m_deltaMp);
                AttachStealHp (target, caster, m_effectStage.m_deltaHp);
                AttachStatus (target, caster, m_effectStage.m_statusIdAndValueAndTimeArr);
            }
        }
        public void NotifyMonsterAutoRecover (E_Monster monster) {
            if (monster.m_IsDead) return;
            if (monster.m_curHp == monster.m_MaxHp && monster.m_curMp == monster.m_MaxMp) return;
            int dHp = (int) (monster.m_curHp * 0.08f);
            int dMp = (int) (monster.m_curMp * 0.08f);
            monster.m_curHp = Math.Min (monster.m_MaxHp, monster.m_curHp + dHp);
            monster.m_curMp = Math.Min (monster.m_MaxMp, monster.m_curMp + dMp);
        }
        private void AttachHpAndMpChange (E_Unit target, E_Unit caster, int dHp, int dMp) {
            target.m_curHp = Math.Max (0, Math.Min (target.m_MaxHp, target.m_curHp + dHp));
            target.m_curMp = Math.Max (0, Math.Min (target.m_MaxMp, target.m_curMp + dMp));
            if (dHp >= 0 && dMp >= 0) return;

            // 统计伤害量
            int newDmg = -dHp;
            target.m_isAttackedTimer = MyTimer.s_CurTime.Ticked (c_isAttackedLastTime);
            int oriDmg;
            if (!target.m_netIdAndDamageDict.TryGetValue (caster.m_networkId, out oriDmg))
                oriDmg = 0;
            target.m_netIdAndDamageDict[caster.m_networkId] = oriDmg + newDmg;

            // 若单位死亡
            if (target.m_IsDead) {
                target.Dead ();

                // 处理物品掉落
                if (target.m_UnitType == ActorUnitType.MONSTER)
                    GL_Item.s_instance.NotifyMonsterDropLegacy (target as E_Monster, caster);
                if (target.m_UnitType == ActorUnitType.PLAYER)
                    GL_Item.s_instance.NotifyCharacterDropLegacy (target as E_Character, caster);

                // client
                m_networkService.SendServerCommand (SC_ApplyAllDead.Instance (
                    EM_Sight.s_instance.GetInSightCharacterNetworkId (target.m_networkId, true),
                    caster.m_networkId,
                    target.m_networkId
                ));
                // log
                if (target.m_UnitType == ActorUnitType.MONSTER && caster.m_UnitType == ActorUnitType.PLAYER)
                    GL_MissionLog.s_instance.NotifyLog (MissionTargetType.KILL_MONSTER, caster.m_networkId, ((E_Monster) target).m_MonsterId);
                // 通知 CharacterLevel
                if (caster.m_UnitType == ActorUnitType.PLAYER)
                    GL_CharacterAttribute.s_instance.NotifyKillUnit ((E_Character) caster, target);
            }
        }
        public void AttachHatred (E_Unit target, E_Unit caster, float hatred) {
            // 仇恨 (伤害列表)
            // xjb 仇恨 现在暂时使用 E_Unit.m_netIdAndDamageDict 来记录
        }
        private void AttachStatus (E_Unit target, E_Unit caster, (short, float, float) [] statusIdAndValueAndTimeArr) {
            foreach (var statusInfo in statusIdAndValueAndTimeArr) {
                var status = EM_Status.s_instance.GetStatusInstanceAndAttach (target.m_networkId, statusInfo);
                m_statusHandlerDict[status.m_Type].Attach (status, target, caster);
                // client
                m_networkService.SendServerCommand (SC_ApplyAllStatus.Instance (
                    EM_Sight.s_instance.GetInSightCharacterNetworkId (target.m_networkId, true),
                    target.m_networkId,
                    status.GetNo ()));
            }
        }
        private void AttachStealHp (E_Unit target, E_Unit caster, int deltaHp) {
            if (deltaHp < 0) {
                caster.m_curHp = (caster.m_curHp + deltaHp * caster.m_LifeSteal) > caster.m_MaxHp?caster.m_MaxHp : caster.m_curHp + deltaHp * caster.m_LifeSteal;
            }
        }
        private void TickStatusPerSecond (E_Unit target, E_Status status) {
            m_statusHandlerDict[status.m_Type].TickPerSecond (status, target);
        }
        private void RemoveStatus (E_Unit target, List<int> orderedIndexList, IReadOnlyList<E_Status> fullStatusList) {
            for (int i = 0; i < orderedIndexList.Count; i++)
                m_statusHandlerDict[fullStatusList[orderedIndexList[i]].m_Type].Remove (fullStatusList[orderedIndexList[i]], target);
            EM_Status.s_instance.RemoveOrderedStatus (target.m_networkId, orderedIndexList);
        }
    }
}