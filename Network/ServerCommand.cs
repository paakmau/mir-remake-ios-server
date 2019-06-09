using System.Collections.Generic;
using System.Numerics;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    abstract class ServerCommandBase {
        public abstract NetworkToClientDataType m_DataType { get; }
        public abstract DeliveryMethod m_DeliveryMethod { get; }
        public IReadOnlyList<int> m_toClientList;
        public abstract void PutData (NetDataWriter writer);
    }

    /// <summary>
    /// 初始化NetId
    /// </summary>
    class SC_InitSelfNetworkId : ServerCommandBase {
        private static readonly SC_InitSelfNetworkId s_instance = new SC_InitSelfNetworkId ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_NETWORK_ID; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_networkId;
        public static SC_InitSelfNetworkId Instance (IReadOnlyList<int> toClientList, int netId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_networkId = netId;
            return s_instance;
        }
        private SC_InitSelfNetworkId () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_networkId);
        }
    }
    /// <summary>
    /// 初始化属性点与等级
    /// </summary>
    class SC_InitSelfAttribute : ServerCommandBase {
        private static readonly SC_InitSelfAttribute s_instance = new SC_InitSelfAttribute ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_ATTRIBUTE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_level;
        private int m_exp;
        private short m_strength;
        private short m_intelligence;
        private short m_agility;
        private short m_spirit;
        public static SC_InitSelfAttribute Instance (IReadOnlyList<int> toClientList, short level, int exp, short strenth, short intelligence, short agility, short spirit) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_level = level;
            s_instance.m_exp = exp;
            s_instance.m_strength = strenth;
            s_instance.m_intelligence = intelligence;
            s_instance.m_agility = agility;
            s_instance.m_spirit = spirit;
            return s_instance;
        }
        private SC_InitSelfAttribute () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_level);
            writer.Put (m_exp);
            writer.Put (m_strength);
            writer.Put (m_intelligence);
            writer.Put (m_agility);
            writer.Put (m_spirit);
        }
    }
    /// <summary>
    /// 初始化习得技能
    /// </summary>
    class SC_InitSelfSkill : ServerCommandBase {
        private static readonly SC_InitSelfSkill s_instance = new SC_InitSelfSkill ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_SKILL; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private (short, short, int) [] m_skillIdAndLvAndMasterlys;
        public static SC_InitSelfSkill Instance (IReadOnlyList<int> toClientList, (short, short, int) [] skillIdAndLvAndMasterlys) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_skillIdAndLvAndMasterlys = skillIdAndLvAndMasterlys;
            return s_instance;
        }
        private SC_InitSelfSkill () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((short) m_skillIdAndLvAndMasterlys.Length);
            for (int i = 0; i < m_skillIdAndLvAndMasterlys.Length; i++) {
                writer.Put (m_skillIdAndLvAndMasterlys[i].Item1);
                writer.Put (m_skillIdAndLvAndMasterlys[i].Item2);
                writer.Put (m_skillIdAndLvAndMasterlys[i].Item3);
            }
        }
    }
    /// <summary>
    /// 初始化所持道具
    /// </summary>
    class SC_InitSelfItem : ServerCommandBase {
        private static readonly SC_InitSelfItem s_instance = new SC_InitSelfItem ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_ITEM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private NO_Repository m_bag;
        private NO_Repository m_storeHouse;
        private NO_Repository m_equipmentRegion;
        public static SC_InitSelfItem Instance (
            IReadOnlyList<int> toClientList,
            NO_Repository bag,
            NO_Repository storeHouse,
            NO_Repository equips
        ) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_bag = bag;
            s_instance.m_storeHouse = storeHouse;
            s_instance.m_equipmentRegion = equips;
            return s_instance;
        }
        private SC_InitSelfItem () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_bag);
            writer.Put (m_storeHouse);
            writer.Put (m_equipmentRegion);
        }
    }
    /// <summary>
    /// 视野中的怪物
    /// </summary>
    class SC_ApplyOtherMonsterInSight : ServerCommandBase {
        private static SC_ApplyOtherMonsterInSight s_instance = new SC_ApplyOtherMonsterInSight ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_MONSTER_IN_SIGHT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<NO_Monster> m_monList;
        public static SC_ApplyOtherMonsterInSight Instance (IReadOnlyList<int> toClientList, IReadOnlyList<NO_Monster> newMonList) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_monList = newMonList;
            return s_instance;
        }
        private SC_ApplyOtherMonsterInSight () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_monList.Count);
            for (int i = 0; i < m_monList.Count; i++)
                writer.Put (m_monList[i]);
        }
    }
    /// <summary>
    /// 视野中的其他角色
    /// </summary>
    class SC_ApplyOtherCharacterInSight : ServerCommandBase {
        private static SC_ApplyOtherCharacterInSight s_instance = new SC_ApplyOtherCharacterInSight ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_CHARACTER_IN_SIGHT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList < (NO_Character, IReadOnlyList<short>) > m_charAndEquipedIdList;
        public static SC_ApplyOtherCharacterInSight Instance (
            IReadOnlyList<int> toClientList,
            IReadOnlyList < (NO_Character, IReadOnlyList<short>) > newCharAndEquipedIdList
        ) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_charAndEquipedIdList = newCharAndEquipedIdList;
            return s_instance;
        }
        private SC_ApplyOtherCharacterInSight () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_charAndEquipedIdList.Count);
            for (int i = 0; i < m_charAndEquipedIdList.Count; i++) {
                writer.Put (m_charAndEquipedIdList[i].Item1);
                writer.Put ((byte) m_charAndEquipedIdList[i].Item2.Count);
                for (int j = 0; j < m_charAndEquipedIdList[i].Item2.Count; j++)
                    writer.Put (m_charAndEquipedIdList[i].Item2[j]);
            }
        }
    }
    /// <summary>
    /// 需要在视野中移除的单位
    /// </summary>
    class SC_ApplyOtherActorUnitOutOfSight : ServerCommandBase {
        private static SC_ApplyOtherActorUnitOutOfSight s_instance = new SC_ApplyOtherActorUnitOutOfSight ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_ACTOR_UNIT_OUT_OF_SIGHT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<int> m_unitIdList;
        public static SC_ApplyOtherActorUnitOutOfSight Instance (IReadOnlyList<int> toClientList, IReadOnlyList<int> unitIdList) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_unitIdList = unitIdList;
            return s_instance;
        }
        private SC_ApplyOtherActorUnitOutOfSight () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_unitIdList.Count);
            for (int i = 0; i < m_unitIdList.Count; i++)
                writer.Put (m_unitIdList[i]);
        }
    }
    /// <summary>
    /// 同步其他单位位置
    /// </summary>
    class SC_SetOtherPosition : ServerCommandBase {
        private static SC_SetOtherPosition s_instance = new SC_SetOtherPosition ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_OTHER_POSITION; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        IReadOnlyList < (int, Vector2) > m_otherNetIdAndPosList;
        public static SC_SetOtherPosition Instance (IReadOnlyList<int> toClientList, IReadOnlyList < (int, Vector2) > otherNetIdAndPosList) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_otherNetIdAndPosList = otherNetIdAndPosList;
            return s_instance;
        }
        private SC_SetOtherPosition () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_otherNetIdAndPosList.Count);
            for (int i = 0; i < m_otherNetIdAndPosList.Count; i++) {
                writer.Put (m_otherNetIdAndPosList[i].Item1);
                writer.Put (m_otherNetIdAndPosList[i].Item2);
            }
        }
    }
    /// <summary>
    /// 同步所有角色Hp与Mp
    /// </summary>
    class SC_SetAllHPAndMP : ServerCommandBase {
        private static readonly SC_SetAllHPAndMP s_instance = new SC_SetAllHPAndMP ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_ALL_HP_AND_MP; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        IReadOnlyList<int> m_allNetIdList;
        IReadOnlyList < (int, int, int, int) > m_hpAndMaxHpAndMpAndMaxMpList;
        public static SC_SetAllHPAndMP Instance (IReadOnlyList<int> toClientList, IReadOnlyList<int> allNetIdList, IReadOnlyList < (int, int, int, int) > hpAndMaxHpAndMpAndMaxMpList) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_allNetIdList = allNetIdList;
            s_instance.m_hpAndMaxHpAndMpAndMaxMpList = hpAndMaxHpAndMpAndMaxMpList;
            return s_instance;
        }
        private SC_SetAllHPAndMP () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_allNetIdList.Count);
            for (int i = 0; i < m_allNetIdList.Count; i++) {
                writer.Put (m_allNetIdList[i]);
                writer.Put (m_hpAndMaxHpAndMpAndMaxMpList[i].Item1);
                writer.Put (m_hpAndMaxHpAndMpAndMaxMpList[i].Item2);
                writer.Put (m_hpAndMaxHpAndMpAndMaxMpList[i].Item3);
                writer.Put (m_hpAndMaxHpAndMpAndMaxMpList[i].Item4);
            }
        }
    }
    /// <summary>
    /// 其他单位开始释放技能
    /// </summary>
    class SC_ApplyOtherCastSkillBegin : ServerCommandBase {
        private static readonly SC_ApplyOtherCastSkillBegin s_instance = new SC_ApplyOtherCastSkillBegin ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_CAST_SKILL_BEGIN; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_casterNetId;
        private short m_skillId;
        private NO_SkillParam m_parm;
        public static SC_ApplyOtherCastSkillBegin Instance (IReadOnlyList<int> toClientList, int casterNetId, short skillId, NO_SkillParam parm) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_casterNetId = casterNetId;
            s_instance.m_skillId = skillId;
            s_instance.m_parm = parm;
            return s_instance;
        }
        private SC_ApplyOtherCastSkillBegin () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_casterNetId);
            writer.Put (m_skillId);
            writer.Put (m_parm);
        }
    }
    /// <summary>
    /// 其他单位中断技能吟唱
    /// </summary>
    class SC_ApplyOtherCastSkillSingCancel : ServerCommandBase {
        public static SC_ApplyOtherCastSkillSingCancel s_instance = new SC_ApplyOtherCastSkillSingCancel ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_CAST_SKILL_SING_CANCEL; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_casterNetId;
        public static SC_ApplyOtherCastSkillSingCancel Instance (IReadOnlyList<int> toClientList, int casterNetId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_casterNetId = casterNetId;
            return s_instance;
        }
        private SC_ApplyOtherCastSkillSingCancel () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_casterNetId);
        }
    }
    /// <summary>
    /// 所有单位施加Effect
    /// </summary>
    class SC_ApplyAllEffect : ServerCommandBase {
        private static SC_ApplyAllEffect s_instance = new SC_ApplyAllEffect ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_EFFECT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_targetNetId;
        private NO_Effect m_effect;
        public static SC_ApplyAllEffect Instance (IReadOnlyList<int> toClientList, int targetNetId, NO_Effect effectNo) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_targetNetId = targetNetId;
            s_instance.m_effect = effectNo;
            return s_instance;
        }
        private SC_ApplyAllEffect () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_targetNetId);
            writer.Put (m_effect);
        }
    }
    /// <summary>
    /// 所有单位状态改变
    /// </summary>
    class SC_ApplyAllStatus : ServerCommandBase {
        private static SC_ApplyAllStatus s_instance = new SC_ApplyAllStatus ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_STATUS; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_targetNetId;
        private NO_Status m_statusNo;
        private bool m_isAttach;
        public static SC_ApplyAllStatus Instance (IReadOnlyList<int> toClientList, int targetNetId, NO_Status statusNo, bool isAttach) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_targetNetId = targetNetId;
            s_instance.m_statusNo = statusNo;
            s_instance.m_isAttach = isAttach;
            return s_instance;
        }
        private SC_ApplyAllStatus () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_targetNetId);
            writer.Put (m_statusNo);
            writer.Put (m_isAttach);
        }
    }
    /// <summary>
    /// 所有单位死亡信息
    /// </summary>
    class SC_ApplyAllDead : ServerCommandBase {
        private static SC_ApplyAllDead s_instance = new SC_ApplyAllDead ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_DEAD; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_killerNetId;
        private int m_deadNetId;
        public static SC_ApplyAllDead Instance (IReadOnlyList<int> toClientList, int killNetId, int deadNetId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_killerNetId = killNetId;
            s_instance.m_deadNetId = deadNetId;
            return s_instance;
        }
        private SC_ApplyAllDead () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_killerNetId);
            writer.Put (m_deadNetId);
        }
    }
    /// <summary>
    /// 所有单位更换装备外观
    /// </summary>
    class SC_ApplyAllChangeEquipment : ServerCommandBase {
        private static SC_ApplyAllChangeEquipment s_instance = new SC_ApplyAllChangeEquipment ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_CHANGE_EQUIPMENT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_itemId;
        public static SC_ApplyAllChangeEquipment Instance (IReadOnlyList<int> toClientList, short itemId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_itemId = itemId;
            return s_instance;
        }
        private SC_ApplyAllChangeEquipment () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_itemId);
        }
    }
    /// <summary>
    /// 获得物品
    /// </summary>
    class SC_ApplySelfGainItem : ServerCommandBase {
        private static SC_ApplySelfGainItem s_instance = new SC_ApplySelfGainItem ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_GAIN_ITEM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<NO_Item> m_itemList;
        private IReadOnlyList<ItemPlace> m_itemPlaceList;
        private IReadOnlyList<short> m_itemPositionList;
        public static SC_ApplySelfGainItem Instance (
            IReadOnlyList<int> toClientList,
            IReadOnlyList<NO_Item> gainedItemList,
            IReadOnlyList<ItemPlace> placeList,
            IReadOnlyList<short> posList
        ) {
            // TODO: 这里的装备有问题
            s_instance.m_toClientList = toClientList;
            s_instance.m_itemList = gainedItemList;
            s_instance.m_itemPlaceList = placeList;
            s_instance.m_itemPositionList = posList;
            return s_instance;
        }
        private SC_ApplySelfGainItem () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_itemList.Count);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put (m_itemList[i]);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put ((byte) m_itemPlaceList[i]);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put (m_itemPositionList[i]);
        }
    }
    /// <summary>
    /// 更新物品数量  
    /// 数量为0则为丢弃
    /// </summary>
    class SC_ApplySelfUpdateItemNum : ServerCommandBase {
        private static SC_ApplySelfUpdateItemNum s_instance = new SC_ApplySelfUpdateItemNum ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_UPDATE_ITEM_NUM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<long> m_itemRealIdList;
        private IReadOnlyList<short> m_itemNumList;
        public static SC_ApplySelfUpdateItemNum Instance (
            IReadOnlyList<int> toClientList,
            IReadOnlyList<long> itemRealIdList,
            IReadOnlyList<short> itemNumList
        ) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_itemRealIdList = itemRealIdList;
            s_instance.m_itemNumList = itemNumList;
            return s_instance;
        }
        private SC_ApplySelfUpdateItemNum () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_itemRealIdList.Count);
            for (int i = 0; i < m_itemRealIdList.Count; i++)
                writer.Put (m_itemRealIdList[i]);
            for (int i = 0; i < m_itemRealIdList.Count; i++)
                writer.Put (m_itemNumList[i]);
        }
    }
    /// <summary>
    /// 交换物品位置
    /// </summary>
    class SC_ApplySelfMoveItem : ServerCommandBase {
        private static SC_ApplySelfMoveItem s_instance = new SC_ApplySelfMoveItem ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_MOVE_ITEM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private ItemPlace m_srcPlace;
        private short m_srcPosition;
        private ItemPlace m_tarPlace;
        private short m_tarPosition;
        public static SC_ApplySelfMoveItem Instance (IReadOnlyList<int> toClientList, ItemPlace srcPlace, short srcPos, ItemPlace tarPlace, short tarPos) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_srcPlace = srcPlace;
            s_instance.m_srcPosition = srcPos;
            s_instance.m_tarPlace = tarPlace;
            s_instance.m_tarPosition = tarPos;
            return s_instance;
        }
        private SC_ApplySelfMoveItem () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_srcPlace);
            writer.Put (m_srcPosition);
            writer.Put ((byte) m_tarPlace);
            writer.Put (m_tarPosition);
        }
    }
    
    /// <summary>
    /// 更新所持货币
    /// </summary>
    class SC_ApplySelfCurrency : ServerCommandBase {
        private static SC_ApplySelfCurrency s_instance = new SC_ApplySelfCurrency ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_CURRENCY; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private long m_virtualCy;
        private long m_chargeCy;
        public static SC_ApplySelfCurrency Instance (int netId, long virtualCy, long chargeCy) {
            s_instance.m_toClientList = new List<int> (netId);
            s_instance.m_virtualCy = virtualCy;
            s_instance.m_chargeCy = chargeCy;
            return s_instance;
        }
        private SC_ApplySelfCurrency () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_virtualCy);
            writer.Put (m_chargeCy);
        }
    }
    /// <summary>
    /// 地面道具出现
    /// </summary>
    class SC_ApplyGroundItemShow : ServerCommandBase {
        private static SC_ApplyGroundItemShow s_instance = new SC_ApplyGroundItemShow ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_GROUND_ITEM_SHOW; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<NO_Item> m_itemList;
        private IReadOnlyList<Vector2> m_posList;
        public static SC_ApplyGroundItemShow Instance (IReadOnlyList<int> toClientList, IReadOnlyList<NO_Item> itemList, IReadOnlyList<Vector2> posList) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_itemList = itemList;
            s_instance.m_posList = posList;
            return s_instance;
        }
        private SC_ApplyGroundItemShow () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_itemList.Count);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put (m_itemList[i]);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put (m_posList[i]);
        }
    }

    /// <summary>
    /// 地面道具消失
    /// </summary>
    class SC_ApplyGroundItemDisappear : ServerCommandBase {
        private static SC_ApplyGroundItemDisappear s_instance = new SC_ApplyGroundItemDisappear ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_GROUND_ITEM_DISAPPEAR; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<long> m_realIdList;
        public static SC_ApplyGroundItemDisappear Instance (IReadOnlyList<int> toClientList, IReadOnlyList<long> realIdList) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_realIdList = realIdList;
            return s_instance;
        }
        private SC_ApplyGroundItemDisappear () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_realIdList.Count);
            for (int i = 0; i < m_realIdList.Count; i++)
                writer.Put (m_realIdList[i]);
        }
    }

    /// <summary>
    /// 修改技能等级与熟练度
    /// </summary>
    class SC_ApplySelfUpdateSkillLevelAndMasterly : ServerCommandBase {
        private static SC_ApplySelfUpdateSkillLevelAndMasterly s_instance = new SC_ApplySelfUpdateSkillLevelAndMasterly ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_UPDATE_SKILL_LEVEL_AND_MASTERLY; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_skillId;
        private short m_skillLv;
        private int m_masterly;
        public static SC_ApplySelfUpdateSkillLevelAndMasterly Instance (IReadOnlyList<int> toClientList, short skillId, short skillLv, int masterly) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_skillId = skillId;
            s_instance.m_skillLv = skillLv;
            s_instance.m_masterly = masterly;
            return s_instance;
        }
        private SC_ApplySelfUpdateSkillLevelAndMasterly () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_skillId);
            writer.Put (m_skillLv);
            writer.Put (m_masterly);
        }
    }
    /// <summary>
    /// 接受任务
    /// </summary>
    class SC_ApplySelfAcceptMission : ServerCommandBase {
        private static SC_ApplySelfAcceptMission s_instance = new SC_ApplySelfAcceptMission ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_ACCECPT_MISSION; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_missionId;
        public static SC_ApplySelfAcceptMission Instance (IReadOnlyList<int> toClientList, short missionId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_missionId = missionId;
            return s_instance;
        }
        private SC_ApplySelfAcceptMission () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
        }
    }
    class SC_ApplySelfDeliverMission : ServerCommandBase {
        private static SC_ApplySelfDeliverMission s_instance = new SC_ApplySelfDeliverMission ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_DELIVER_MISSION; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_missionId;
        public static SC_ApplySelfDeliverMission Instance (IReadOnlyList<int> toClientList, short missionId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_missionId = missionId;
            return s_instance;
        }
        private SC_ApplySelfDeliverMission () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
        }
    }
    class SC_ApplySelfCancelMission : ServerCommandBase {
        private static SC_ApplySelfCancelMission s_instance = new SC_ApplySelfCancelMission ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_CANCEL_MISSION; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_missionId;
        public static SC_ApplySelfCancelMission Instance (IReadOnlyList<int> toClientList, short missionId) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_missionId = missionId;
            return s_instance;
        }
        private SC_ApplySelfCancelMission () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
        }
    }
}