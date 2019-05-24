using System;
using System.Collections.Generic;

namespace MirRemakeBackend {
    /// <summary>
    /// 处理战斗相关逻辑
    /// </summary>
    class SM_ActorUnit {
        public void NetworkTick () {
            var selfKeyEn = m_characterNetIdSet.GetEnumerator ();

            while (selfKeyEn.MoveNext ()) {
                var selfNetId = selfKeyEn.Current;
                var self = (E_Character) EM_ActorUnit.GetActorUnitByNetworkId (selfNetId);

                // 发送其他unit视野信息
                List<int> characterNetIdList = new List<int> ();
                List<int> monsterNetIdList = new List<int> ();
                var otherUnitEn = EM_ActorUnit.GetActorUnitEnumerator ();
                while (otherUnitEn.MoveNext ()) {
                    if (otherUnitEn.Current.Key == selfNetId) continue;
                    switch (otherUnitEn.Current.Value.m_ActorUnitType) {
                        case ActorUnitType.MONSTER:
                            monsterNetIdList.Add (otherUnitEn.Current.Key);
                            break;
                        case ActorUnitType.PLAYER:
                            characterNetIdList.Add (otherUnitEn.Current.Key);
                            break;
                    }
                }
                m_networkService.SendServerCommand (new SC_ApplyOtherActorUnitInSight (new List<int> { selfNetId }, ActorUnitType.PLAYER, characterNetIdList));
                m_networkService.SendServerCommand (new SC_ApplyOtherActorUnitInSight (new List<int> { selfNetId }, ActorUnitType.MONSTER, monsterNetIdList));

                // 发送视野内所有单位的位置信息
                List<int> unitNetIdList = new List<int> ();
                List<Vector2> posList = new List<Vector2> ();
                var allUnitEn = EM_ActorUnit.GetActorUnitEnumerator ();
                while (allUnitEn.MoveNext ())
                    if (allUnitEn.Current.Key != selfNetId) {
                        unitNetIdList.Add (allUnitEn.Current.Key);
                        posList.Add (allUnitEn.Current.Value.m_Position);
                    }
                m_networkService.SendServerCommand (new SC_SetOtherPosition (new List<int> { selfNetId }, unitNetIdList, posList));

                // 发送视野内所有单位的HP与MP
                unitNetIdList.Clear ();
                List<Dictionary<ActorUnitConcreteAttributeType, int>> HPMPList = new List<Dictionary<ActorUnitConcreteAttributeType, int>> ();
                allUnitEn = EM_ActorUnit.GetActorUnitEnumerator ();
                while (allUnitEn.MoveNext ()) {
                    var allUnit = allUnitEn.Current.Value;
                    if (allUnit.m_IsDead) continue;
                    unitNetIdList.Add (allUnitEn.Current.Key);
                    HPMPList.Add (allUnit.m_concreteAttributeDict);
                }
                m_networkService.SendServerCommand (new SC_SetAllHPAndMP (new List<int> { selfNetId }, unitNetIdList, HPMPList));
            }
        }
        public void CommandApplyCastSkillSingCancel (int netId) {
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillSingCancel (GetCharacterInSightIdList (netId, false), netId));
        }
    }
}