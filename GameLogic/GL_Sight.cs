using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 每帧计算视野
    /// </summary>
    class GL_Sight : GameLogicBase {
        public static GL_Sight s_instance;
        private const float c_sightRadius = 100f;
        private const int c_maxUnitNumInSight = 50;
        private List<E_ActorUnit> t_unitList = new List<E_ActorUnit> ();
        private List<NO_Monster> t_monNoList = new List<NO_Monster> ();
        private List < (NO_Character, IReadOnlyList<short>) > t_charNoAndShortListList = new List < (NO_Character, IReadOnlyList<short>) > ();
        private List<int> t_intList = new List<int> ();
        private List<int> t_intList2 = new List<int> ();
        public GL_Sight (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            var en = EM_ActorUnit.s_instance.GetCharacterEnumerator ();
            while (en.MoveNext ()) {
                // 得到当前视野
                int charNetId = en.Current.Key;
                var charObj = en.Current.Value;
                var charNowSight = t_unitList;
                charNowSight.Clear ();
                var unitEn = EM_Sight.s_instance.GetActorUnitVisibleEnumerator ();
                while (unitEn.MoveNext ()) {
                    // 若在视野范围外
                    if ((charObj.m_position - unitEn.Current.m_position).LengthSquared () > c_sightRadius * c_sightRadius)
                        continue;
                    charNowSight.Add (unitEn.Current);
                    if (charNowSight.Count >= c_maxUnitNumInSight)
                        break;
                }
                var charOriSight = EM_Sight.s_instance.GetCharacterRawSight (charNetId);

                // 计算视野新增的怪物与角色
                var newMonNoList = t_monNoList;
                var newCharNoAndEquipedIdList = t_charNoAndShortListList;
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
                    switch (charNowSight[i].m_ActorUnitType) {
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
                var outUnitNetIdList = t_intList;
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
                // 发送给Client视野变化
                var toClientList = t_intList2;
                toClientList.Clear ();
                toClientList.Add (charNetId);
                m_networkService.SendServerCommand (SC_ApplyOtherMonsterInSight.Instance (
                    toClientList,
                    newMonNoList
                ));
                m_networkService.SendServerCommand (SC_ApplyOtherCharacterInSight.Instance (
                    toClientList,
                    newCharNoAndEquipedIdList
                ));
                m_networkService.SendServerCommand (SC_ApplyOtherActorUnitOutOfSight.Instance (
                    toClientList,
                    outUnitNetIdList
                ));
            }
        }
        public override void NetworkTick () { }
    }
}