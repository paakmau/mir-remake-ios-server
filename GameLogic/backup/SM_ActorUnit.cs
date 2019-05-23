using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 处理战斗相关逻辑
    /// </summary>
    class SM_ActorUnit {
        public List<E_ActorUnit> GetActorUnitsInSectorRange (E_ActorUnit self, Vector2 center, Vector2 dir, float range, float radian, CampType targetCamp, byte num) {
            // TODO: 解决非圆扇形的作用目标判定
            List<E_ActorUnit> res = new List<E_ActorUnit> ();
            var unitEn = EM_ActorUnit.GetActorUnitValueEnumerator ();
            while (unitEn.MoveNext ()) {
                if (CheckCampMatch (self, unitEn.Current, targetCamp) && (center - unitEn.Current.m_Position).magnitude < range + unitEn.Current.m_CoverRadius)
                    res.Add (unitEn.Current);
            }
            return GetNearestUnits (center, res, num);
        }
        public List<E_ActorUnit> GetActorUnitsInCircleRange (E_ActorUnit self, Vector2 center, float range, CampType targetCamp, byte num) {
            List<E_ActorUnit> res = new List<E_ActorUnit> ();
            var unitEn = EM_ActorUnit.GetActorUnitValueEnumerator ();
            while (unitEn.MoveNext ()) {
                if (CheckCampMatch (self, unitEn.Current, targetCamp) && (center - unitEn.Current.m_Position).magnitude < range + unitEn.Current.m_CoverRadius)
                    res.Add (unitEn.Current);
            }
            return GetNearestUnits (center, res, num);
        }
        public List<E_ActorUnit> GetActorUnitsInLineRange (E_ActorUnit self, Vector2 center, Vector2 dir, float distance, float width, CampType targetCamp, byte num) {
            List<E_ActorUnit> res = new List<E_ActorUnit> ();
            var unitEn = EM_ActorUnit.GetActorUnitValueEnumerator ();
            while (unitEn.MoveNext ()) {
                if (CheckCampMatch (self, unitEn.Current, targetCamp) && false) // TODO: 解决直线的作用目标判定
                    res.Add (unitEn.Current);
            }
            return GetNearestUnits (center, res, num);
        }
        private List<E_ActorUnit> GetNearestUnits (Vector2 center, List<E_ActorUnit> units, byte num) {
            if (units.Count <= num) return units;
            // TODO: 对units进行排序并剔除多余的unit
            // units.Sort();
            return units;
        }
        public bool CheckCampMatch (E_ActorUnit self, E_ActorUnit target, CampType camp) {
            switch (camp) {
                case CampType.SELF:
                    return self == target;
                case CampType.FRIEND:
                    return false;
                case CampType.ENEMY:
                    return self != target;
            }
            return false;
        }
        public void NotifyApplyCastSkillSettle (E_ActorUnit unit, E_Skill skill, List<E_ActorUnit> tarList) {
            KeyValuePair<int, E_Status[]>[] statusPairArr;
            unit.CastSkillSettle (skill, tarList, out statusPairArr);
            m_networkService.SendServerCommand (new SC_ApplyAllEffect (GetCharacterInSightIdList (unit.m_networkId, true), skill.m_skillEffect.m_animId, (byte) skill.m_skillEffect.m_StatusAttachNum, statusPairArr));
        }
        public void NotifyApplyCastSkillBegin (E_ActorUnit unit, E_Skill skill, SkillParam parm) {
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillBegin (GetCharacterInSightIdList (unit.m_networkId, false), unit.m_networkId, skill.m_id, parm.GetNo ()));
        }
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