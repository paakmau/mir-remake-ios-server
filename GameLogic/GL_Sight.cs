using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 每帧计算视野
    /// </summary>
    class GL_Sight : GameLogicBase {
        public static GL_Sight s_instance;
        private const float c_sightRadius = 12f;
        private const int c_maxUnitNumInSight = 30;
        public GL_Sight (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            var en = EM_Unit.s_instance.GetCharacterEnumerator ();
            while (en.MoveNext ()) {
                int charNetId = en.Current.Key;
                var charObj = en.Current.Value;
                // 原本视野
                var charOriSight = EM_Sight.s_instance.GetCharacterRawSight (charNetId);
                if (charOriSight == null) continue;

                // 计算当前视野
                var charNowSight = new List<E_Unit> ();
                var unitEn = EM_Sight.s_instance.GetUnitVisibleEnumerator ();
                while (unitEn.MoveNext ()) {
                    // 若在视野范围外
                    if ((charObj.m_position - unitEn.Current.m_position).LengthSquared () > c_sightRadius * c_sightRadius)
                        continue;
                    // 若为自己
                    if (charNetId == unitEn.Current.m_networkId)
                        continue;
                    charNowSight.Add (unitEn.Current);
                    if (charNowSight.Count >= c_maxUnitNumInSight)
                        break;
                }

                // 计算视野新增的怪物与角色
                var newMonNoList = new List<NO_Monster> ();
                var newCharNoAndEquipedIdList = new List < (NO_Character, IReadOnlyList<short>) > ();
                newMonNoList.Clear ();
                newCharNoAndEquipedIdList.Clear ();
                for (int i = 0; i < charNowSight.Count; i++) {
                    bool isNew = true;
                    for (int j = 0; j < charOriSight.Count; j++)
                        if (charNowSight[i].m_networkId == charOriSight[j].m_networkId) {
                            isNew = false;
                            break;
                        }
                    if (!isNew) continue;
                    switch (charNowSight[i].m_UnitType) {
                        case ActorUnitType.MONSTER:
                            newMonNoList.Add (((E_Monster) charNowSight[i]).GetNo ());
                            break;
                        case ActorUnitType.PLAYER:
                            newCharNoAndEquipedIdList.Add ((
                                (((E_Character) charNowSight[i]).GetNo ()),
                                EM_Item.s_instance.GetEquipedItemIdList (charNowSight[i].m_networkId)
                            ));
                            break;
                    }
                }
                // 计算视野中移除的单位
                var outUnitNetIdList = new List<int> ();
                for (int i = 0; i < charOriSight.Count; i++) {
                    bool isOut = true;
                    for (int j = 0; j < charNowSight.Count; j++)
                        if (charOriSight[i].m_networkId == charNowSight[j].m_networkId) {
                            isOut = false;
                            break;
                        }
                    if (!isOut) continue;
                    outUnitNetIdList.Add (charOriSight[i].m_networkId);
                }
                // 设定新视野
                charOriSight.Clear ();
                for (int i = 0; i < charNowSight.Count; i++)
                    charOriSight.Add (charNowSight[i]);
                for (int i = 0; i < newMonNoList.Count; i++) {
                    var inSightCharSet = EM_Sight.s_instance.GetRawUnitInSightCharacter (newMonNoList[i].m_netId);
                    if (inSightCharSet == null) continue;
                    inSightCharSet.Add (charNetId);
                }
                for (int i = 0; i < newCharNoAndEquipedIdList.Count; i++) {
                    var inSightCharSet = EM_Sight.s_instance.GetRawUnitInSightCharacter (newCharNoAndEquipedIdList[i].Item1.m_netId);
                    if (inSightCharSet == null) continue;
                    inSightCharSet.Add (charNetId);
                }
                for (int i = 0; i < outUnitNetIdList.Count; i++) {
                    var inSightCharSet = EM_Sight.s_instance.GetRawUnitInSightCharacter (outUnitNetIdList[i]);
                    if (inSightCharSet == null) continue;
                    inSightCharSet.Remove (charNetId);
                }

                // 发送给Client视野变化
                if (newMonNoList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplyOtherMonsterInSight.Instance (charNetId, newMonNoList));
                if (newCharNoAndEquipedIdList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplyOtherCharacterInSight.Instance (charNetId, newCharNoAndEquipedIdList));
                if (outUnitNetIdList.Count != 0)
                    m_networkService.SendServerCommand (SC_ApplyOtherActorUnitOutOfSight.Instance (charNetId, outUnitNetIdList));
            }
        }
        public override void NetworkTick () { }
        public void NotifyInitAllMonster (E_Monster[] monObjArr) {
            EM_Sight.s_instance.InitAllMonster (monObjArr);
        }
        public void NotifyInitCharacter (E_Character charObj) {
            EM_Sight.s_instance.InitCharacter (charObj);
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Sight.s_instance.RemoveCharacter (netId);
        }
    }
}