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
                        StatusToAttr (unit, statusEn.Current.Value[i], -1);
                    }
                }
                EM_Status.s_instance.RemoveOrderedStatus (netId, statusToRemoveList);
            }
            // 处理具体属性的每秒变化
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
                }
            }
        }
        public override void NetworkTick () { }
        public void NotifyHpAndMpChange (E_Unit target, E_Unit caster, int dHp, int dMp) {
            target.m_CurHp += dHp;
            target.m_CurMp += dMp;
            // xjb计算仇恨
            float hatredTime = (float)(-dHp - dMp) / (float)target.m_MaxHp * 10;
            if (hatredTime < 0)return;
            // target.m_hatredRefreshDict.
        }
        public void NotifyAttachStatus (E_Unit target, E_Unit caster, ValueTuple<short, float, float, int>[] statusIdAndValueAndTimeAndCasterNetIdArr) {
            var statusList = EM_Status.s_instance.AttachStatus (target.m_networkId, statusIdAndValueAndTimeAndCasterNetIdArr);
            for (int i = 0; i < statusList.Count; i++)
                StatusToAttr (target, statusList[i], 1);
            // TODO: 处理caster
        }
        public void NotifyConcreteAttributeChange (E_Unit target, IReadOnlyList<(ActorUnitConcreteAttributeType, int)> dAttr) {
            for (int i=0; i<dAttr.Count; i++)
                target.AddConAttr(dAttr[i].Item1, dAttr[i].Item2);
        }
        private void StatusToAttr (E_Unit unit, E_Status status, int k) {
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
                EM_Sight.s_instance.GetCharacterInSightNetworkId (unit.m_networkId, true),
                unit.m_networkId,
                status.GetNo (),
                k == 1));
        }
    }
}