using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class SM_ActorUnit {
        public static SM_ActorUnit s_instance;
        private INetworkService m_networkService;
        private HashSet<int> m_playerNetIdSet = new HashSet<int> ();
        private const float c_monsterRefreshTime = 15f;
        private Dictionary<int, Vector2> m_networkIdAndMonsterPosDict = new Dictionary<int, Vector2> ();
        private Dictionary<int, short> m_networkIdAndMonsterIdDict = new Dictionary<int, short> ();
        private Dictionary<int, MyTimer.Time> m_networkIdAndMonsterRefreshTimeDict = new Dictionary<int, MyTimer.Time> ();
        private Stack<E_ActorUnit> m_networkIdBodyToDisappearStack = new Stack<E_ActorUnit> ();
        private Stack<int> m_monsterIdToRefreshStack = new Stack<int> ();
        public SM_ActorUnit (INetworkService netService) {
            m_networkService = netService;

            // TODO: 用于测试
            int monsterNetId;
            monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
            m_networkIdAndMonsterIdDict[monsterNetId] = 0;
            m_networkIdAndMonsterPosDict[monsterNetId] = new Vector2 (-1, 0);
            m_networkIdAndMonsterRefreshTimeDict[monsterNetId] = new MyTimer.Time (0, 1f);
            monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
            m_networkIdAndMonsterIdDict[monsterNetId] = 1;
            m_networkIdAndMonsterPosDict[monsterNetId] = new Vector2 (-3, 1);
            m_networkIdAndMonsterRefreshTimeDict[monsterNetId] = new MyTimer.Time (0, 10f);
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
        private List<int> GetPlayerInSightIdList (int selfNetId, bool includeSelf) {
            // TODO: 处理视野问题
            List<int> res = new List<int> ();
            foreach (var netId in m_playerNetIdSet) {
                if (includeSelf || netId != selfNetId)
                    res.Add (netId);
            }
            return res;
        }
        public bool CheckCampMatch (E_ActorUnit self, E_ActorUnit target, CampType camp) {
            // TODO: 解决组队问题
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
            m_networkService.SendServerCommand (new SC_ApplyAllDead (GetPlayerInSightIdList (deadUnitNetId, true), killerNetId, deadUnitNetId));
        }
        public void NotifyUnitBodyDisappear (E_ActorUnit deadUnit) {
            // 死亡单位移除准备
            m_networkIdBodyToDisappearStack.Push (deadUnit);
        }
        public void NotifyApplyCastSkillSettle (E_ActorUnit unit, E_Skill skill, List<E_ActorUnit> tarList) {
            KeyValuePair<int, E_Status[]>[] statusPairArr;
            unit.CastSkillSettle (skill, tarList, out statusPairArr);
            m_networkService.SendServerCommand (new SC_ApplyAllEffect (GetPlayerInSightIdList (unit.m_networkId, true), skill.m_skillEffect.m_animId, (byte) skill.m_skillEffect.m_StatusAttachNum, statusPairArr));
        }
        public void NotifyApplyCastSkillBegin (E_ActorUnit unit, E_Skill skill, SkillParam parm) {
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillBegin (GetPlayerInSightIdList (unit.m_networkId, false), unit.m_networkId, skill.m_id, parm.GetNo ()));
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
                if (bodyToDisappear.m_ActorUnitType == ActorUnitType.MONSTER) {
                    MyTimer.Time refreshTime = MyTimer.s_CurTime;
                    refreshTime.Tick (c_monsterRefreshTime);
                    m_networkIdAndMonsterRefreshTimeDict.Add (bodyToDisappear.m_networkId, refreshTime);
                }
            }

            // 处理怪物刷新
            var monsterDeathTimeEn = m_networkIdAndMonsterRefreshTimeDict.GetEnumerator ();
            while (monsterDeathTimeEn.MoveNext ())
                if (MyTimer.CheckTimeUp (monsterDeathTimeEn.Current.Value))
                    m_monsterIdToRefreshStack.Push (monsterDeathTimeEn.Current.Key);
            int monsterIdToRefresh;
            while (m_monsterIdToRefreshStack.TryPop (out monsterIdToRefresh)) {
                m_networkIdAndMonsterRefreshTimeDict.Remove (monsterIdToRefresh);
                EM_ActorUnit.LoadActorUnit (new E_Monster (monsterIdToRefresh, m_networkIdAndMonsterIdDict[monsterIdToRefresh], m_networkIdAndMonsterPosDict[monsterIdToRefresh]));
            }
        }
        public void NetworkTick () {
            var selfKeyEn = m_playerNetIdSet.GetEnumerator ();

            while (selfKeyEn.MoveNext ()) {
                var selfNetId = selfKeyEn.Current;
                var self = (E_Character) EM_ActorUnit.GetActorUnitByNetworkId (selfNetId);

                // 发送其他unit视野信息
                List<int> playerNetIdList = new List<int> ();
                List<int> monsterNetIdList = new List<int> ();
                var otherUnitEn = EM_ActorUnit.GetActorUnitEnumerator ();
                while (otherUnitEn.MoveNext ()) {
                    if (otherUnitEn.Current.Key == selfNetId) continue;
                    switch (otherUnitEn.Current.Value.m_ActorUnitType) {
                        case ActorUnitType.MONSTER:
                            monsterNetIdList.Add (otherUnitEn.Current.Key);
                            break;
                        case ActorUnitType.PLAYER:
                            playerNetIdList.Add (otherUnitEn.Current.Key);
                            break;
                    }
                }
                m_networkService.SendServerCommand (new SC_ApplyOtherActorUnitInSight (new List<int> { selfNetId }, ActorUnitType.PLAYER, playerNetIdList));
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
            m_playerNetIdSet.Remove (netId);
        }
        public void CommandInitCharacterPlayerId (int netId, int playerId) {
            // TODO: 获取DDO
            E_Character newChar = new E_Character (netId, playerId);
            EM_ActorUnit.LoadActorUnit (newChar);
            m_playerNetIdSet.Add (netId);
            short[] skillIdArr;
            short[] skillLvArr;
            int[] skillMasterlyArr;
            newChar.GetAllLearnedSkill (out skillIdArr, out skillLvArr, out skillMasterlyArr);

            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_Level, newChar.m_Experience, skillIdArr, skillLvArr, skillMasterlyArr));
        }
        public void CommandSetPosition (int netId, Vector2 pos) {
            E_ActorUnit unit = EM_ActorUnit.GetActorUnitByNetworkId (netId);
            if (unit == null) return;
            unit.m_Position = pos;
        }
        public void CommandApplyCastSkillBegin (int netId, short skillId, NO_SkillParam parmNo) {
            E_ActorUnit player = EM_ActorUnit.GetActorUnitByNetworkId (netId);
            if (player == null) return;
            E_Skill skill = SM_Skill.s_instance.GetSkillByIdAndPlayerNetworkId (netId, skillId);
            E_ActorUnit target = EM_ActorUnit.GetActorUnitByNetworkId (parmNo.m_targetNetworkId);
            SkillParam parm = new SkillParam (skill.m_AimType, target, parmNo.m_direction, parmNo.m_position);
            ((E_Character) player).CastSkillBegin (skill, parm);
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillBegin (GetPlayerInSightIdList (netId, false), netId, skillId, parmNo));
        }
        public void CommandApplyCastSkillSingCancel (int netId) {
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillSingCancel (GetPlayerInSightIdList (netId, false), netId));
        }
        public void CommandUseConsumableItem (int netId, int itemRealId) {
            E_ActorUnit player = EM_ActorUnit.GetActorUnitByNetworkId (netId);
            if (player == null) return;

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