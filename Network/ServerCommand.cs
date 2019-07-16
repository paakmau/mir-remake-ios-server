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
    class SC_InitSelfNetworkId : ServerCommandBase {
        private static readonly SC_InitSelfNetworkId s_instance = new SC_InitSelfNetworkId ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_NETWORK_ID; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private int m_netId;
        public static SC_InitSelfNetworkId Instance (int netId) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_netId = netId;
            return s_instance;
        }
        private SC_InitSelfNetworkId () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_netId);
        }
    }
    class SC_InitSelfRegister : ServerCommandBase {
        private static readonly SC_InitSelfRegister s_instance = new SC_InitSelfRegister ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_REGISTER; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private bool m_success;
        public static SC_InitSelfRegister Instance (int netId, bool success) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_success = success;
            return s_instance;
        }
        private SC_InitSelfRegister () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_success);
        }
    }
    class SC_InitSelfLogin : ServerCommandBase {
        private static readonly SC_InitSelfLogin s_instance = new SC_InitSelfLogin ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_LOGIN; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private bool m_success;
        private int m_playerId;
        private IReadOnlyList<NO_LoginCharacter> m_loginCharList;
        public static SC_InitSelfLogin Instance (int netId, bool success, int playerId, IReadOnlyList<NO_LoginCharacter> loginCharList) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_success = success;
            s_instance.m_playerId = playerId;
            s_instance.m_loginCharList = loginCharList;
            return s_instance;
        }
        private SC_InitSelfLogin () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_success);
            writer.Put (m_playerId);
            writer.Put ((byte) m_loginCharList.Count);
            for (int i = 0; i < m_loginCharList.Count; i++)
                writer.Put (m_loginCharList[i]);
        }
    }
    class SC_InitSelfGetPasswordProtectProblem : ServerCommandBase {
        private static readonly SC_InitSelfGetPasswordProtectProblem s_instance = new SC_InitSelfGetPasswordProtectProblem ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_GET_PASSWORD_PROTECT_PROBLEM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private bool m_success;
        private string m_problem;
        public static SC_InitSelfGetPasswordProtectProblem Instance (int netId, bool success, string problem) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_success = success;
            s_instance.m_problem = problem;
            return s_instance;
        }
        private SC_InitSelfGetPasswordProtectProblem () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_success);
            writer.Put (m_problem);
        }
    }
    class SC_InitSelfFindPassword : ServerCommandBase {
        private static readonly SC_InitSelfFindPassword s_instance = new SC_InitSelfFindPassword ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_FIND_PASSWORD; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private bool m_success;
        public static SC_InitSelfFindPassword Instance (int netId, bool success) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_success = success;
            return s_instance;
        }
        private SC_InitSelfFindPassword () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_success);
        }
    }
    class SC_InitSelfModifyPassword : ServerCommandBase {
        private static readonly SC_InitSelfModifyPassword s_instance = new SC_InitSelfModifyPassword ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_MODIFY_PASSWORD; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private bool m_success;
        public static SC_InitSelfModifyPassword Instance (int netId, bool success) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_success = success;
            return s_instance;
        }
        private SC_InitSelfModifyPassword () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_success);
        }
    }
    class SC_InitSelfCreateCharacter : ServerCommandBase {
        private static readonly SC_InitSelfCreateCharacter s_instance = new SC_InitSelfCreateCharacter ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_CREATE_CHARACTER; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private bool m_success;
        public static SC_InitSelfCreateCharacter Instance (int netId, bool success) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_success = success;
            return s_instance;
        }
        private SC_InitSelfCreateCharacter () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_success);
        }
    }
    /// <summary>
    /// 初始化属性点与等级
    /// </summary>
    class SC_InitSelfAttribute : ServerCommandBase {
        private static readonly SC_InitSelfAttribute s_instance = new SC_InitSelfAttribute ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_ATTRIBUTE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 职业 </summary>
        private OccupationType m_ocp;
        /// <summary> 等级 </summary>
        private short m_level;
        /// <summary> 经验 </summary>
        private int m_exp;
        /// <summary> 力量 </summary>
        private short m_strength;
        /// <summary> 智力 </summary>
        private short m_intelligence;
        /// <summary> 敏捷 </summary>
        private short m_agility;
        /// <summary> 精神 </summary>
        private short m_spirit;
        /// <summary> 总可分配属性点 </summary>
        private short m_totalMainPoint;
        /// <summary> 虚拟币 </summary>
        private long m_virtualCy;
        /// <summary> 充值币 </summary>
        private long m_chargeCy;
        private Vector2 m_pos;
        public static SC_InitSelfAttribute Instance (
            int netId,
            OccupationType ocp,
            short level,
            int exp,
            short strenth,
            short intelligence,
            short agility,
            short spirit,
            short m_totalMainPoint,
            long virtualCy,
            long chargeCy,
            Vector2 pos
        ) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_ocp = ocp;
            s_instance.m_level = level;
            s_instance.m_exp = exp;
            s_instance.m_strength = strenth;
            s_instance.m_intelligence = intelligence;
            s_instance.m_agility = agility;
            s_instance.m_spirit = spirit;
            s_instance.m_totalMainPoint = m_totalMainPoint;
            s_instance.m_virtualCy = virtualCy;
            s_instance.m_chargeCy = chargeCy;
            s_instance.m_pos = pos;
            return s_instance;
        }
        private SC_InitSelfAttribute () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_ocp);
            writer.Put (m_level);
            writer.Put (m_exp);
            writer.Put (m_strength);
            writer.Put (m_intelligence);
            writer.Put (m_agility);
            writer.Put (m_spirit);
            writer.Put (m_totalMainPoint);
            writer.Put (m_virtualCy);
            writer.Put (m_chargeCy);
            writer.Put (m_pos);
        }
    }
    /// <summary>
    /// 初始化技能习得情况
    /// </summary>
    class SC_InitSelfSkill : ServerCommandBase {
        private static readonly SC_InitSelfSkill s_instance = new SC_InitSelfSkill ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_SKILL; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 职业可学的所有技能列表, (技能Id, 当前等级, 技能熟练度) </summary>
        private (short, short, int) [] m_skillIdAndLvAndMasterlys;
        public static SC_InitSelfSkill Instance (int netId, (short, short, int) [] skillIdAndLvAndMasterlys) {
            s_instance.m_toClientList = new List<int> { netId };
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
    /// 初始化任务情况
    /// </summary>
    class SC_InitSelfMission : ServerCommandBase {
        private static readonly SC_InitSelfMission s_instance = new SC_InitSelfMission ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_MISSION; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 已接任务Id列表 </summary>
        IReadOnlyList<short> m_acceptedMis;
        /// <summary> 可接任务Id列表 </summary>
        IReadOnlyList<short> m_acceptableMis;
        /// <summary> 不可接但已解锁任务Id列表 </summary>
        IReadOnlyList<short> m_unacceptableMis;
        public static SC_InitSelfMission Instance (int netId, IReadOnlyList<short> acceptedMis, IReadOnlyList<short> acceptableMis, IReadOnlyList<short> unacceptableMis) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_acceptedMis = acceptedMis;
            s_instance.m_acceptableMis = acceptableMis;
            s_instance.m_unacceptableMis = unacceptableMis;
            return s_instance;
        }
        private SC_InitSelfMission () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_acceptedMis.Count);
            for (int i = 0; i < m_acceptedMis.Count; i++)
                writer.Put (m_acceptedMis[i]);
            writer.Put ((byte) m_acceptableMis.Count);
            for (int i = 0; i < m_acceptableMis.Count; i++)
                writer.Put (m_acceptableMis[i]);
            writer.Put ((byte) m_unacceptableMis.Count);
            for (int i = 0; i < m_unacceptableMis.Count; i++)
                writer.Put (m_unacceptableMis[i]);
        }
    }
    /// <summary>
    /// 初始化所持道具
    /// </summary>
    class SC_InitSelfItem : ServerCommandBase {
        private static readonly SC_InitSelfItem s_instance = new SC_InitSelfItem ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.INIT_SELF_ITEM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 背包 </summary>
        private NO_Repository m_bag;
        /// <summary> 仓库 </summary>
        private NO_Repository m_storeHouse;
        /// <summary> 装备区 </summary>
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
    /// 新进入视野的怪物
    /// </summary>
    class SC_ApplyOtherMonsterInSight : ServerCommandBase {
        private static SC_ApplyOtherMonsterInSight s_instance = new SC_ApplyOtherMonsterInSight ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_MONSTER_IN_SIGHT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 新进入视野 怪物列表 </summary>
        private IReadOnlyList<NO_Monster> m_monList;
        public static SC_ApplyOtherMonsterInSight Instance (int netId, IReadOnlyList<NO_Monster> newMonList) {
            s_instance.m_toClientList = new List<int> { netId };
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
    /// 新进入视野中的其他角色
    /// </summary>
    class SC_ApplyOtherCharacterInSight : ServerCommandBase {
        private static SC_ApplyOtherCharacterInSight s_instance = new SC_ApplyOtherCharacterInSight ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_CHARACTER_IN_SIGHT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 新进入视野中的角色列表 (角色信息, 身上的装备Id列表) </summary>
        private IReadOnlyList < (NO_Character, IReadOnlyList<short>) > m_charAndEquipedIdList;
        public static SC_ApplyOtherCharacterInSight Instance (
            int netId,
            IReadOnlyList < (NO_Character, IReadOnlyList<short>) > newCharAndEquipedIdList
        ) {
            s_instance.m_toClientList = new List<int> { netId };
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
    /// 离开视野的单位
    /// </summary>
    class SC_ApplyOtherActorUnitOutOfSight : ServerCommandBase {
        private static SC_ApplyOtherActorUnitOutOfSight s_instance = new SC_ApplyOtherActorUnitOutOfSight ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_ACTOR_UNIT_OUT_OF_SIGHT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 离开视野单位的NetId列表 </summary>
        private IReadOnlyList<int> m_unitIdList;
        public static SC_ApplyOtherActorUnitOutOfSight Instance (int netId, IReadOnlyList<int> unitIdList) {
            s_instance.m_toClientList = new List<int> { netId };
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
        /// <summary> 其他单位的位置 (NetId, Pos) </summary>
        IReadOnlyList < (int, Vector2) > m_otherNetIdAndPosList;
        public static SC_SetOtherPosition Instance (int netId, IReadOnlyList < (int, Vector2) > otherNetIdAndPosList) {
            s_instance.m_toClientList = new List<int> { netId };
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
        /// <summary> 视野内所有角色的NetId (可能会多传或少传) </summary>
        IReadOnlyList<int> m_allNetIdList;
        /// <summary> 他们相对应的 (Hp, MaxHp, Mp, MaxMp) </summary>
        IReadOnlyList < (int, int, int, int) > m_hpAndMaxHpAndMpAndMaxMpList;
        public static SC_SetAllHPAndMP Instance (int netId, IReadOnlyList<int> allNetIdList, IReadOnlyList < (int, int, int, int) > hpAndMaxHpAndMpAndMaxMpList) {
            s_instance.m_toClientList = new List<int> { netId };
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
    /// 同步自身所有属性
    /// </summary>
    class SC_SetSelfConcreteAttribute : ServerCommandBase {
        private static readonly SC_SetSelfConcreteAttribute s_instance = new SC_SetSelfConcreteAttribute ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_SELF_CONCRETE_ATTRIBUTE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        int m_atk;
        int m_def;
        int m_mag;
        int m_res;
        public static SC_SetSelfConcreteAttribute Instance (int netId, int atk, int def, int mag, int res) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_atk = atk;
            s_instance.m_def = def;
            s_instance.m_mag = mag;
            s_instance.m_res = res;
            return s_instance;
        }
        private SC_SetSelfConcreteAttribute () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_atk);
            writer.Put (m_def);
            writer.Put (m_mag);
            writer.Put (m_res);
        }
    }
    /// <summary>
    /// 更新自身特殊属性  
    /// 如眩晕, 禁锢, 沉默等  
    /// </summary>
    class SC_ApplySelfSpecialAttribute : ServerCommandBase {
        private static readonly SC_ApplySelfSpecialAttribute s_instance = new SC_ApplySelfSpecialAttribute ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_SPECIAL_ATTRIBUTE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 特殊状态的类型 </summary>
        private ActorUnitSpecialAttributeType m_spAttrType;
        /// <summary> 特殊状态是被附加还是移除 </summary>
        private bool m_isAttach;
        public static SC_ApplySelfSpecialAttribute Instance (int netId, ActorUnitSpecialAttributeType spAttrType, bool isAttach) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_spAttrType = spAttrType;
            s_instance.m_isAttach = isAttach;
            return s_instance;
        }
        private SC_ApplySelfSpecialAttribute () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_spAttrType);
            writer.Put (m_isAttach);
        }
    }
    /// <summary>
    /// 更新自身等级与经验值与可分配的主属性点
    /// </summary>
    class SC_ApplySelfLevelAndExp : ServerCommandBase {
        private static readonly SC_ApplySelfLevelAndExp s_instance = new SC_ApplySelfLevelAndExp ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_LEVEL_AND_EXP; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        short m_lv;
        int m_exp;
        short m_totalMainPoint;
        public static SC_ApplySelfLevelAndExp Instance (int netId, short lv, int exp, short totalMainPoint) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_lv = lv;
            s_instance.m_exp = exp;
            s_instance.m_totalMainPoint = totalMainPoint;
            return s_instance;
        }
        private SC_ApplySelfLevelAndExp () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_lv);
            writer.Put (m_exp);
            writer.Put (m_totalMainPoint);
        }
    }
    /// <summary>
    /// 更新自身主属性加点
    /// </summary>
    class SC_ApplySelfMainAttribute : ServerCommandBase {
        private static readonly SC_ApplySelfMainAttribute s_instance = new SC_ApplySelfMainAttribute ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_MAIN_ATTRIBUTE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        short m_str;
        short m_intl;
        short m_agl;
        short m_spr;
        public static SC_ApplySelfMainAttribute Instance (int netId, short str, short intl, short agl, short spr) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_str = str;
            s_instance.m_intl = intl;
            s_instance.m_agl = agl;
            s_instance.m_spr = spr;
            return s_instance;
        }
        private SC_ApplySelfMainAttribute () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_str);
            writer.Put (m_intl);
            writer.Put (m_agl);
            writer.Put (m_spr);
        }
    }
    /// <summary>
    /// 更新所有单位的复活
    /// </summary>
    class SC_ApplyAllRespawn : ServerCommandBase {
        private static readonly SC_ApplyAllRespawn s_instance = new SC_ApplyAllRespawn ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_RESPAWN; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 复活的单位的NetId </summary>
        private int m_netId;
        /// <summary> 它的位置 </summary>
        private Vector2 m_pos;
        /// <summary> Hp </summary>
        private int m_hp;
        /// <summary> MaxHp </summary>
        private int m_maxHp;
        public static SC_ApplyAllRespawn Instance (List<int> toClientList, int netId, Vector2 pos, int hp, int maxHp) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_netId = netId;
            s_instance.m_pos = pos;
            s_instance.m_hp = hp;
            s_instance.m_maxHp = maxHp;
            return s_instance;
        }
        private SC_ApplyAllRespawn () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_netId);
            writer.Put (m_pos);
            writer.Put (m_hp);
            writer.Put (m_maxHp);
        }
    }
    /// <summary>
    /// 发送战斗力排行榜
    /// </summary>
    class SC_SendFightCapacityRank : ServerCommandBase {
        private static readonly SC_SendFightCapacityRank s_instance = new SC_SendFightCapacityRank ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SEND_FIGHT_CAPACITY_RANK; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        IReadOnlyList<NO_FightCapacityRankInfo> m_combatEfctRankList;
        int m_myCombatEfct;
        int m_myRank;
        public static SC_SendFightCapacityRank Instance (int netId, IReadOnlyList<NO_FightCapacityRankInfo> combatEfctRnkList, int myCombatEfct, int myRank) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_combatEfctRankList = combatEfctRnkList;
            s_instance.m_myCombatEfct = myCombatEfct;
            s_instance.m_myRank = myRank;
            return s_instance;
        }
        private SC_SendFightCapacityRank () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_combatEfctRankList.Count);
            for (int i = 0; i < m_combatEfctRankList.Count; i++)
                writer.Put (m_combatEfctRankList[i]);
            writer.Put (m_myCombatEfct);
            writer.Put (m_myRank);
        }
    }
    /// <summary>
    /// 其他单位开始释放技能
    /// </summary>
    class SC_ApplyAllCastSkillBegin : ServerCommandBase {
        private static readonly SC_ApplyAllCastSkillBegin s_instance = new SC_ApplyAllCastSkillBegin ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_CAST_SKILL_BEGIN; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 施法单位的NetId </summary>
        private int m_casterNetId;
        /// <summary> 技能的Id </summary>
        private short m_skillId;
        /// <summary> 技能参数 </summary>
        private NO_SkillParam m_parm;
        public static SC_ApplyAllCastSkillBegin Instance (IReadOnlyList<int> toClientList, int casterNetId, short skillId, NO_SkillParam parm) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_casterNetId = casterNetId;
            s_instance.m_skillId = skillId;
            s_instance.m_parm = parm;
            return s_instance;
        }
        private SC_ApplyAllCastSkillBegin () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_casterNetId);
            writer.Put (m_skillId);
            writer.Put (m_parm);
        }
    }
    /// <summary>
    /// 对单位施加Effect
    /// </summary>
    class SC_ApplyAllEffect : ServerCommandBase {
        private static SC_ApplyAllEffect s_instance = new SC_ApplyAllEffect ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_EFFECT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 被施加Effect单位的NetId </summary>
        private int m_targetNetId;
        /// <summary> Effect </summary>
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
        public static SC_ApplyAllStatus Instance (IReadOnlyList<int> toClientList, int targetNetId, NO_Status statusNo) {
            s_instance.m_toClientList = toClientList;
            s_instance.m_targetNetId = targetNetId;
            s_instance.m_statusNo = statusNo;
            return s_instance;
        }
        private SC_ApplyAllStatus () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_targetNetId);
            writer.Put (m_statusNo);
        }
    }
    /// <summary>
    /// 所有单位死亡信息
    /// </summary>
    class SC_ApplyAllDead : ServerCommandBase {
        private static SC_ApplyAllDead s_instance = new SC_ApplyAllDead ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_ALL_DEAD; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        /// <summary> 凶手NetId </summary>
        private int m_killerNetId;
        /// <summary> 死者NetId </summary>
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
        /// <summary> 该单位换上的装备的ItemId </summary>
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
    /// 更新物品数量  
    /// 数量为0则为丢弃
    /// </summary>
    class SC_ApplySelfUpdateItem : ServerCommandBase {
        private static SC_ApplySelfUpdateItem s_instance = new SC_ApplySelfUpdateItem ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_UPDATE_ITEM; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<NO_Item> m_itemList;
        public static SC_ApplySelfUpdateItem Instance (
            int netId,
            IReadOnlyList<NO_Item> itemList
        ) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_itemList = itemList;
            return s_instance;
        }
        private SC_ApplySelfUpdateItem () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_itemList.Count);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put (m_itemList[i]);
        }
    }
    /// <summary>
    /// 更新装备信息
    /// </summary>
    class SC_ApplySelfUpdateEquipment : ServerCommandBase {
        private static SC_ApplySelfUpdateEquipment s_instance = new SC_ApplySelfUpdateEquipment ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_UPDATE_EQUIPMENT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private long m_realId;
        private NO_EquipmentItemInfo m_eqInfo;
        public static SC_ApplySelfUpdateEquipment Instance (int netId, long realId, NO_EquipmentItemInfo eqInfo) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_realId = realId;
            s_instance.m_eqInfo = eqInfo;
            return s_instance;
        }
        private SC_ApplySelfUpdateEquipment () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_realId);
            writer.Put (m_eqInfo);
        }
    }
    /// <summary>
    /// 更新装备信息
    /// </summary>
    class SC_ApplySelfUpdateEnchantment : ServerCommandBase {
        private static SC_ApplySelfUpdateEnchantment s_instance = new SC_ApplySelfUpdateEnchantment ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_UPDATE_ENCHANTMENT; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private long m_realId;
        private NO_EnchantmentItemInfo m_ecmtInfo;
        public static SC_ApplySelfUpdateEnchantment Instance (int netId, long realId, NO_EnchantmentItemInfo ecmtInfo) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_realId = realId;
            s_instance.m_ecmtInfo = ecmtInfo;
            return s_instance;
        }
        private SC_ApplySelfUpdateEnchantment () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_realId);
            writer.Put (m_ecmtInfo);
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
            s_instance.m_toClientList = new List<int> { netId };
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
        private IReadOnlyList<NO_GroundItem> m_itemList;
        public static SC_ApplyGroundItemShow Instance (int netId, IReadOnlyList<NO_GroundItem> itemList) {
            s_instance.m_toClientList = new List<int> () { netId };
            s_instance.m_itemList = itemList;
            return s_instance;
        }
        private SC_ApplyGroundItemShow () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_itemList.Count);
            for (int i = 0; i < m_itemList.Count; i++)
                writer.Put (m_itemList[i]);
        }
    }

    /// <summary>
    /// 地面道具消失
    /// </summary>
    class SC_ApplyGroundItemDisappear : ServerCommandBase {
        private static SC_ApplyGroundItemDisappear s_instance = new SC_ApplyGroundItemDisappear ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_GROUND_ITEM_DISAPPEAR; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<long> m_gndItemIdList;
        public static SC_ApplyGroundItemDisappear Instance (int netId, IReadOnlyList<long> gndIdList) {
            s_instance.m_toClientList = new List<int> () { netId };
            s_instance.m_gndItemIdList = gndIdList;
            return s_instance;
        }
        private SC_ApplyGroundItemDisappear () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_gndItemIdList.Count);
            for (int i = 0; i < m_gndItemIdList.Count; i++)
                writer.Put (m_gndItemIdList[i]);
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
        public static SC_ApplySelfUpdateSkillLevelAndMasterly Instance (int netId, short skillId, short skillLv, int masterly) {
            s_instance.m_toClientList = new List<int> { netId };
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
        public static SC_ApplySelfAcceptMission Instance (int netId, short missionId) {
            s_instance.m_toClientList = new List<int> { netId };
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
        public static SC_ApplySelfDeliverMission Instance (int netId, short missionId) {
            s_instance.m_toClientList = new List<int> { netId };
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
        public static SC_ApplySelfCancelMission Instance (int netId, short missionId) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_missionId = missionId;
            return s_instance;
        }
        private SC_ApplySelfCancelMission () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
        }
    }
    class SC_ApplySelfMissionProgress : ServerCommandBase {
        private static SC_ApplySelfMissionProgress s_instance = new SC_ApplySelfMissionProgress ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_MISSION_PROGRESS; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private short m_missionId;
        private byte m_targetNum;
        private int m_value;
        public static SC_ApplySelfMissionProgress Instance (int netId, short missionId, byte targetNum, int value) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_missionId = missionId;
            s_instance.m_targetNum = targetNum;
            s_instance.m_value = value;
            return s_instance;
        }
        private SC_ApplySelfMissionProgress () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put (m_missionId);
            writer.Put (m_targetNum);
            writer.Put (m_value);
        }
    }
    class SC_ApplySelfMissionUnlock : ServerCommandBase {
        private static SC_ApplySelfMissionUnlock s_instance = new SC_ApplySelfMissionUnlock ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_MISSION_UNLOCK; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<short> m_acceptableMis;
        private IReadOnlyList<short> m_unacceptableMis;
        public static SC_ApplySelfMissionUnlock Instance (int netId, IReadOnlyList<short> acceptableMis, IReadOnlyList<short> unacceptableMis) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_acceptableMis = acceptableMis;
            s_instance.m_unacceptableMis = unacceptableMis;
            return s_instance;
        }
        private SC_ApplySelfMissionUnlock () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_acceptableMis.Count);
            for (int i = 0; i < m_acceptableMis.Count; i++)
                writer.Put (m_acceptableMis[i]);
            writer.Put ((byte) m_unacceptableMis.Count);
            for (int i = 0; i < m_unacceptableMis.Count; i++)
                writer.Put (m_unacceptableMis[i]);
        }
    }
    class SC_ApplySelfMissionAcceptable : ServerCommandBase {
        private static SC_ApplySelfMissionAcceptable s_instance = new SC_ApplySelfMissionAcceptable ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_SELF_MISSION_ACCEPTABLE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private IReadOnlyList<short> m_acceptableMis;
        public static SC_ApplySelfMissionAcceptable Instance (int netId, IReadOnlyList<short> acceptableMis) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_acceptableMis = acceptableMis;
            return s_instance;
        }
        private SC_ApplySelfMissionAcceptable () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_acceptableMis.Count);
            for (int i = 0; i < m_acceptableMis.Count; i++)
                writer.Put (m_acceptableMis[i]);
        }
    }
    class SC_SendShoppingMall : ServerCommandBase {
        private static SC_SendShoppingMall s_instance = new SC_SendShoppingMall ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SEND_SHOPPING_MALL; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        public static SC_SendShoppingMall Instance (int netId, IReadOnlyList<short> itemIdList, IReadOnlyList<long> itemVirtualCurrencyPriceList) {
            s_instance.m_toClientList = new List<int> { netId };
            return s_instance;
        }
        private SC_SendShoppingMall () { }
        public override void PutData (NetDataWriter writer) {
        }
    }
    class SC_SendMessage : ServerCommandBase {
        public static SC_SendMessage s_instance = new SC_SendMessage ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SEND_MESSAGE; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.ReliableOrdered; } }
        private ChattingChanelType m_channel;
        private int m_senderCharId;
        private string m_senderName;
        private string m_msg;
        public static SC_SendMessage Instance (int netId, ChattingChanelType channel, int senderCharId, string senderName, string msg) {
            s_instance.m_toClientList = new List<int> { netId };
            s_instance.m_channel = channel;
            s_instance.m_senderCharId = senderCharId;
            s_instance.m_senderName = senderName;
            s_instance.m_msg = msg;
            return s_instance;
        }
        private SC_SendMessage () { }
        public override void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_channel);
            writer.Put (m_senderCharId);
            writer.Put (m_senderName);
            writer.Put (m_msg);
        }
    }
}