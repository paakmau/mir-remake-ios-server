using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 处理战斗相关逻辑
    /// </summary>
    class SM_Battle {
        public static SM_Battle s_instance;
        private IDDS_Character m_characterDynamicDataService;
        private IDS_Character m_characterDataService;
        private IDS_Monster m_monsterDataService;
        private INetworkService m_networkService;
        private HashSet<int> m_characterNetIdSet = new HashSet<int> ();
        private Stack<E_ActorUnit> m_networkIdBodyToDisappearStack = new Stack<E_ActorUnit> ();
        public SM_Battle (INetworkService netService) {
            m_networkService = netService;

        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr (int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit> (networkIdArr.Length);
            foreach (var netId in networkIdArr) {
                E_ActorUnit unit = EM_ActorUnit.GetActorUnitByNetworkId (netId);
                if (unit != null)
                    res.Add (unit);
            }
            return res;
        }
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
        private List<int> GetCharacterInSightIdList (int selfNetId, bool includeSelf) {
            // TODO: 处理视野问题
            List<int> res = new List<int> ();
            foreach (var netId in m_characterNetIdSet) {
                if (includeSelf || netId != selfNetId)
                    res.Add (netId);
            }
            return res;
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
        public void NotifyUnitDead (int killerNetId, int deadUnitNetId) {
            m_networkService.SendServerCommand (new SC_ApplyAllDead (GetCharacterInSightIdList (deadUnitNetId, true), killerNetId, deadUnitNetId));
        }
        public void NotifyUnitBodyDisappear (E_ActorUnit deadUnit) {
            // 死亡单位移除准备
            m_networkIdBodyToDisappearStack.Push (deadUnit);
        }
        public void NotifyApplyCastSkillSettle (E_ActorUnit unit, E_Skill skill, List<E_ActorUnit> tarList) {
            KeyValuePair<int, E_Status[]>[] statusPairArr;
            unit.CastSkillSettle (skill, tarList, out statusPairArr);
            m_networkService.SendServerCommand (new SC_ApplyAllEffect (GetCharacterInSightIdList (unit.m_networkId, true), skill.m_skillEffect.m_animId, (byte) skill.m_skillEffect.m_StatusAttachNum, statusPairArr));
        }
        public void NotifyApplyCastSkillBegin (E_ActorUnit unit, E_Skill skill, SkillParam parm) {
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillBegin (GetCharacterInSightIdList (unit.m_networkId, false), unit.m_networkId, skill.m_id, parm.GetNo ()));
        }
        public void Tick (float dT) {
            // 每个单位的Tick
            var unitEn = EM_ActorUnit.GetActorUnitEnumerator ();
            while (unitEn.MoveNext ())
                unitEn.Current.Value.Tick (dT);

            // 移除消失的尸体
            E_ActorUnit bodyToDisappear;
            while (m_networkIdBodyToDisappearStack.TryPop (out bodyToDisappear)) {
                EM_ActorUnit.UnloadActorUnitByNetworkId (bodyToDisappear.m_networkId);
                if (bodyToDisappear.m_ActorUnitType == ActorUnitType.MONSTER)
                    MonsterRespawnManager.SetMonsterToWaitRespawn (bodyToDisappear.m_networkId);
            }

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
        /// <summary>
        /// 在新的玩家连接到服务器后调用
        /// 为它分配并返回一个NetworkId
        /// </summary>
        /// <returns></returns>
        public int CommandAssignNetworkId () {
            return NetworkIdManager.GetNewActorUnitNetworkId ();
        }
        public void CommandRemoveCharacter (int netId) {
            NetworkIdManager.RemoveActorUnitNetworkId (netId);
            EM_ActorUnit.UnloadActorUnitByNetworkId (netId);
            m_characterNetIdSet.Remove (netId);
        }
        public void CommandInitCharacterId (int netId, int charId) {
            DDO_Character charDdo = m_characterDynamicDataService.GetCharacterById (charId);
            DO_Character charDo = m_characterDataService.GetCharacterByOccupationAndLevel (charDdo.m_occupation, charDdo.m_level);
            List<E_Skill> skillList = m_skillSceneManager.InitCharacterSkill (netId, charId);
            List<E_Item> itemInBagList, itemInStoreHouseList;
            List<E_Item> equipedList;
            m_itemSceneManager.InitCharacterItems (netId, charId, out itemInBagList, out itemInStoreHouseList, out equipedList);
            E_Character newChar = new E_Character (netId, charId, charDo, charDdo);
            EM_ActorUnit.LoadActorUnit (newChar);
            m_characterNetIdSet.Add (netId);
            short[] skillIdArr = new short[skillList.Count];
            short[] skillLvArr = new short[skillList.Count];
            int[] skillMasterlyArr = new int[skillList.Count];
            for (int i = 0; i < skillList.Count; i++) {
                skillIdArr[i] = skillList[i].m_id;
                skillLvArr[i] = skillList[i].m_level;
                skillMasterlyArr[i] = skillList[i].m_masterly;
            }

            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_Level, newChar.m_Experience, skillIdArr, skillLvArr, skillMasterlyArr));
        }
        public void CommandSetPosition (int netId, Vector2 pos) {
            E_ActorUnit unit = EM_ActorUnit.GetActorUnitByNetworkId (netId);
            if (unit == null) return;
            unit.m_Position = pos;
        }
        public void CommandApplyCastSkillBegin (int netId, short skillId, NO_SkillParam parmNo) {
            E_ActorUnit character = EM_ActorUnit.GetActorUnitByNetworkId (netId);
            if (character == null) return;
            E_Skill skill = m_skillSceneManager.GetSkillByIdAndNetworkId (skillId, netId);
            E_ActorUnit target = EM_ActorUnit.GetActorUnitByNetworkId (parmNo.m_targetNetworkId);
            SkillParam parm = new SkillParam (skill.m_AimType, target, parmNo.m_direction, parmNo.m_position);
            ((E_Character) character).CastSkillBegin (skill, parm);
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillBegin (GetCharacterInSightIdList (netId, false), netId, skillId, parmNo));
        }
        public void CommandApplyCastSkillSingCancel (int netId) {
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillSingCancel (GetCharacterInSightIdList (netId, false), netId));
        }
        public void CommandUseConsumableItem (int netId, int itemRealId) {
            E_ActorUnit character = EM_ActorUnit.GetActorUnitByNetworkId (netId);
            if (character == null) return;

        }
        public void CommandUseEquipmentItem (int netId, int itemRealId) {
            // TODO: 
        }
        public void CommandAcceptMission (int netId, short missionId) {
            // TODO: 
        }
        public void CommandFinishMission (int netId, short missionId) {
            // TODO: 
        }
        public void CommandCancelMission (int netId, short missionId) {
            // TODO: 
        }
        public void CommandTalkToMissionNpc (int netId, short npcId, short missionId) {
            // TODO: 
        }
        public void CommandUpdateSkillLevel (int netId, short skillId, short targetSkillLevel) {
            // TODO: 
        }
    }
}