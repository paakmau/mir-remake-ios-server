using System.Collections.Generic;
using System;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    class GL_Status : GameLogicBase {
        private List<int> t_intList = new List<int> ();
        public GL_Status (INetworkService netService) : base (netService) {
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<E_ActorUnit, ValueTuple<short, float, float, int>[]> ("NotifyAddStatus", NotifyAddStatus);
        }
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
                EM_Status.s_instance.RemoveStatus (netId, t_intList);
            }
        }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            EM_Status.s_instance.InitCharacterStatus (netId);
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Status.s_instance.RemoveCharacterStatus (netId);
        }

        public void NotifyAddStatus (E_ActorUnit unit, ValueTuple<short, float, float, int>[] statusIdAndValueAndTimeAndCasterNetIdArr) {
            var statusList = EM_Status.s_instance.AddStatus (unit.m_networkId, statusIdAndValueAndTimeAndCasterNetIdArr);
            for (int i=0; i<statusList.Count; i++)
                StatusToAttr (unit, statusList[i], 1);
        }
        private void StatusToAttr (E_ActorUnit unit, E_Status status, int k) {
            // 处理具体属性
            var cAttrList = status.m_dataEntity.m_affectAttributeList;
            for (int i=0; i<cAttrList.Count; i++)
                unit.AddConAttr (cAttrList[i].Item1, (int)(cAttrList[i].Item2 * status.m_value * k));
            // 处理特殊属性
            var sAttrList = status.m_dataEntity.m_specialAttributeList;
            for (int i=0; i<sAttrList.Count; i++)
                unit.AddSpAttr ()
            // TODO: 通知Client
        }
    }
}