using System;
using System.Collections.Generic;
using UnityEngine;


namespace MirRemake {
    class SM_ActorUnit {
        public static SM_ActorUnit s_instance = new SM_ActorUnit ();
        private HashSet<int> m_playerNetIdSet = new HashSet<int> ();
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitDict = new Dictionary<int, E_ActorUnit> ();
        private const float c_monsterRefreshTime = 20f;
        private const float c_monsterRefreshTimerCycleTime = 10000f;
        private bool m_monsterRefreshTimerCycle = false;
        private float m_monsterRefreshTimer = 0f;
        private Dictionary<int, Vector2> m_networkIdAndMonsterPosDict = new Dictionary<int, Vector2> ();
        private Dictionary<int, short> m_networkIdAndMonsterIdDict = new Dictionary<int, short> ();
        private Dictionary<int, KeyValuePair<bool, float>> m_networkIdAndMonsterDeathTimeDict = new Dictionary<int, KeyValuePair<bool, float>> ();
        public SM_ActorUnit () {
            // TODO: 用于测试
            int monsterNetId;
            monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
            m_networkIdAndMonsterIdDict[monsterNetId] = 0;
            m_networkIdAndMonsterPosDict[monsterNetId] = new Vector2 (-1, 0);
            m_networkIdAndMonsterDeathTimeDict[monsterNetId] = new KeyValuePair<bool, float> (false, 1f);
            monsterNetId = NetworkIdManager.GetNewActorUnitNetworkId ();
            m_networkIdAndMonsterIdDict[monsterNetId] = 1;
            m_networkIdAndMonsterPosDict[monsterNetId] = new Vector2 (-3, 1);
            m_networkIdAndMonsterDeathTimeDict[monsterNetId] = new KeyValuePair<bool, float> (false, 10f);
        }
        private E_ActorUnit GetActorUnitByNetworkId (int networkId) {
            E_ActorUnit res = null;
            m_networkIdAndActorUnitDict.TryGetValue (networkId, out res);
            return res;
        }
        private List<E_ActorUnit> GetActorUnitArrByNetworkIdArr (int[] networkIdArr) {
            List<E_ActorUnit> res = new List<E_ActorUnit> (networkIdArr.Length);
            foreach (var netId in networkIdArr) {
                E_ActorUnit unit = null;
                m_networkIdAndActorUnitDict.TryGetValue (netId, out unit);
                if (unit != null)
                    res.Add (unit);
            }
            return res;
        }
        public List<E_ActorUnit> GetActorUnitsInSectorRange (E_ActorUnit self, Vector2 center, Vector2 dir, float range, float radian, CampType targetCamp, byte num) {
            // TODO: 解决非圆扇形的作用目标判定
            List<E_ActorUnit> res = new List<E_ActorUnit> ();
            var unitEn = m_networkIdAndActorUnitDict.Values.GetEnumerator ();
            while (unitEn.MoveNext ()) {
                if (CheckCampMatch(self, unitEn.Current, targetCamp) && (center - unitEn.Current.m_Position).magnitude < range + unitEn.Current.m_CoverRadius)
                    res.Add (unitEn.Current);
            }
            return GetNearestUnits (center, res, num);
        }
        public List<E_ActorUnit> GetActorUnitsInCircleRange (E_ActorUnit self, Vector2 center, float range, CampType targetCamp, byte num) {
            List<E_ActorUnit> res = new List<E_ActorUnit> ();
            var unitEn = m_networkIdAndActorUnitDict.Values.GetEnumerator ();
            while (unitEn.MoveNext ()) {
                if (CheckCampMatch(self, unitEn.Current, targetCamp) && (center - unitEn.Current.m_Position).magnitude < range + unitEn.Current.m_CoverRadius)
                    res.Add (unitEn.Current);
            }
            return GetNearestUnits (center, res, num);
        }
        public List<E_ActorUnit> GetActorUnitsInLineRange (E_ActorUnit self, Vector2 center, Vector2 dir, float distance, float width, CampType targetCamp, byte num) {
            List<E_ActorUnit> res = new List<E_ActorUnit> ();
            var unitEn = m_networkIdAndActorUnitDict.Values.GetEnumerator ();
            while (unitEn.MoveNext ()) {
                if (CheckCampMatch(self, unitEn.Current, targetCamp) && false) // TODO: 解决直线的作用目标判定
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
            // TODO: 解决组队问题
            switch (camp) {
                case CampType.SELF:
                    return self == target;
                case CampType.FRIEND:
                    return false;
                case CampType.ENEMY:
                    return true;
            }
            return false;
        }
        public void UnitDead (int netId) {
            var unit = GetActorUnitByNetworkId (netId);
            if (unit != null && unit.m_ActorUnitType == ActorUnitType.Monster) {
                m_networkIdAndActorUnitDict.Remove (netId);
                m_networkIdAndMonsterDeathTimeDict.Add (netId, new KeyValuePair<bool, float> (m_monsterRefreshTimerCycle, m_monsterRefreshTimer + c_monsterRefreshTime));
            }
        }
        public void Tick (float dT) {
            // 每个单位的Tick
            var unitEn = m_networkIdAndActorUnitDict.GetEnumerator ();
            while (unitEn.MoveNext ())
                unitEn.Current.Value.Tick (dT);

            // 处理怪物刷新
            m_monsterRefreshTimer += dT;
            while (m_monsterRefreshTimer >= c_monsterRefreshTimerCycleTime) {
                m_monsterRefreshTimer -= c_monsterRefreshTimerCycleTime;
                m_monsterRefreshTimerCycle = !m_monsterRefreshTimerCycle;
            }
            List<int> monsterIdToRefreshList = new List<int> ();
            var monsterDeathTimeEn = m_networkIdAndMonsterDeathTimeDict.GetEnumerator ();
            while (monsterDeathTimeEn.MoveNext ())
                if (monsterDeathTimeEn.Current.Value.Key == m_monsterRefreshTimerCycle && monsterDeathTimeEn.Current.Value.Value <= m_monsterRefreshTimer)
                    monsterIdToRefreshList.Add (monsterDeathTimeEn.Current.Key);
            for (int i = 0; i < monsterIdToRefreshList.Count; i++) {
                int monsterIdToRefresh = monsterIdToRefreshList[i];
                m_networkIdAndMonsterDeathTimeDict.Remove (monsterIdToRefresh);
                m_networkIdAndActorUnitDict.Add (monsterIdToRefresh, new E_Monster (monsterIdToRefresh, m_networkIdAndMonsterIdDict[monsterIdToRefresh], m_networkIdAndMonsterPosDict[monsterIdToRefresh]));
            }
        }
        public void NetworkTick () {
            var selfKeyEn = m_playerNetIdSet.GetEnumerator ();

            while (selfKeyEn.MoveNext ()) {
                var selfNetId = selfKeyEn.Current;
                var self = (E_Character) m_networkIdAndActorUnitDict[selfNetId];

                // 发送其他unit视野信息
                List<int> playerNetIdList = new List<int> ();
                List<int> monsterNetIdList = new List<int> ();
                var otherUnitEn = m_networkIdAndActorUnitDict.GetEnumerator ();
                while (otherUnitEn.MoveNext ()) {
                    if (otherUnitEn.Current.Key == selfNetId) continue;
                    if (otherUnitEn.Current.Value.m_IsDead) continue;
                    switch (otherUnitEn.Current.Value.m_ActorUnitType) {
                        case ActorUnitType.Monster:
                            monsterNetIdList.Add (otherUnitEn.Current.Key);
                            break;
                        case ActorUnitType.Player:
                            playerNetIdList.Add (otherUnitEn.Current.Key);
                            break;
                    }
                }
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight (selfNetId, ActorUnitType.Player, playerNetIdList);
                NetworkService.s_instance.NetworkSetOtherActorUnitInSight (selfNetId, ActorUnitType.Monster, monsterNetIdList);

                // 发送所有单位的位置信息
                List<int> unitNetIdList = new List<int> ();
                List<Vector2> posList = new List<Vector2> ();
                var allUnitEn = m_networkIdAndActorUnitDict.GetEnumerator ();
                while (allUnitEn.MoveNext ())
                    if (allUnitEn.Current.Key != selfNetId) {
                        unitNetIdList.Add (allUnitEn.Current.Key);
                        posList.Add (allUnitEn.Current.Value.m_Position);
                    }
                NetworkService.s_instance.NetworkSetOtherPosition (selfNetId, unitNetIdList, posList);

                // 发送所有单位的HP与MP
                unitNetIdList.Clear ();
                List<Dictionary<ActorUnitConcreteAttributeType, int>> HPMPList = new List<Dictionary<ActorUnitConcreteAttributeType, int>> ();
                allUnitEn = m_networkIdAndActorUnitDict.GetEnumerator ();
                while (allUnitEn.MoveNext ()) {
                    var allUnit = allUnitEn.Current.Value;
                    unitNetIdList.Add (allUnitEn.Current.Key);
                    HPMPList.Add (allUnit.m_concreteAttributeDict);
                }
                NetworkService.s_instance.NetworkSetAllHPAndMP (selfNetId, unitNetIdList, HPMPList);
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
            m_networkIdAndActorUnitDict.Remove (netId);
            m_playerNetIdSet.Remove (netId);
        }
        public void CommandSetCharacterPlayerId (int netId, int playerId) {
            E_Character newChar = new E_Character (netId, playerId);
            m_networkIdAndActorUnitDict[netId] = newChar;
            m_playerNetIdSet.Add (netId);
            short[] skillIdArr;
            short[] skillLvArr;
            int[] skillMasterlyArr;
            newChar.GetAllLearnedSkill (out skillIdArr, out skillLvArr, out skillMasterlyArr);

            NetworkService.s_instance.NetworkSetSelfInfo (netId, newChar.m_Level, newChar.m_Experience, skillIdArr, skillLvArr, skillMasterlyArr);
        }
        public void CommandSetPosition (int netId, Vector2 pos) {
            m_networkIdAndActorUnitDict[netId].SetPosition (pos);
        }
        public void CommandApplyCastSkill (int netId, short skillId, int[] tarIdArr) {
            E_Skill skill = new E_Skill (skillId);
            KeyValuePair<int, E_Status[]>[] statusPairArr;
            List<int> deadNetIdList;
            E_ActorUnit unit = GetActorUnitByNetworkId(netId);
            if(unit == null) return;
            unit.ApplyCastSkill (skill, GetActorUnitArrByNetworkIdArr (tarIdArr), out statusPairArr, out deadNetIdList);
            NetworkService.s_instance.NetworkSetAllEffectToAll (skill.m_skillEffect.m_animId, (byte) skill.m_skillEffect.m_StatusAttachNum, statusPairArr);
            if (deadNetIdList.Count != 0)
                NetworkService.s_instance.NetworkSetAllDeadToAll (netId, deadNetIdList);
        }
        public void CommandApplyActiveEnterFSMState (int netId, FSMActiveEnterState state) {
            m_networkIdAndActorUnitDict[netId].ApplyActiveEnterFSMState (state);
            NetworkService.s_instance.NetworkSetSelfFSMStateToOther (netId, state);
        }
        public void CommandAcceptingMission (int netId, short missionId) {
            E_Character character = GetPlayerByNetId (netId);
            E_Mission mission = new E_Mission ();
            // TODO:根据任务id从数据库获取任务
            character.AcceptingMission (mission);

            NetworkService.s_instance.NetworkConfirmAcceptingMission (netId, missionId);
        }

        public void CommandDeliveringMission (int netId, short missionId) {
            E_Character character = GetPlayerByNetId (netId);
            NetworkService.s_instance.NetworkConfirmDeliveringMission (netId, missionId, character.DeliveringMission (missionId));
        }

        public E_Character GetPlayerByNetId (int netId) {
            E_ActorUnit actorUnit = GetActorUnitByNetworkId (netId);
            if (actorUnit.m_ActorUnitType == ActorUnitType.Player) {
                return (E_Character) actorUnit;
            }
            return null;
        }

        public void CommandCancelMission (int netId, short missionId) {
            E_Character character = GetPlayerByNetId (netId);
            character.CancelMission (missionId);
            NetworkService.s_instance.NetworkConfirmMissionFailed (netId, missionId);
        }

        public void CommandBlacksmithBuilding(int netId, Dictionary<short, short> materials, short NPCId) {
            E_BlacksmithNPC blacksmith = new E_BlacksmithNPC();
            // TODO:计数
            BuildingEquipmentFortune face = blacksmith.LookIntoTheMirror(materials);
            E_Equipment equipment = new E_Equipment(face);
            List<E_Item> production = new List<E_Item>();
            production.Add(equipment);
            
            List<E_Item> e_materials = new List<E_Item>();
            foreach(KeyValuePair<short, short> material in materials) {
                E_Material e_material = new E_Material(material.Key);
                e_material.m_Num = material.Value;
                e_materials.Add(e_material);
            }

            E_Character character = GetPlayerByNetId(netId);
            character.LossItems(e_materials);
            character.LossMoneyByType(CurrencyType.VIRTUAL, 10000); // TODO:打造花费
            character.GainItems(production);

            NetworkService.s_instance.NetworkApplyBlacksmithBuilding(netId, NPCId, equipment.m_Id, equipment.m_RealityId, face);
        }
    }
}