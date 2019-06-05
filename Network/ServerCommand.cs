using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    abstract class ServerCommandBase {
        public abstract NetworkToClientDataType m_DataType { get; }
        public abstract DeliveryMethod m_DeliveryMethod { get; }
        public IReadOnlyList<int> m_toClientList;
        public abstract void PutData (NetDataWriter writer);
    }
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
    class SC_SetAllHPAndMP : ServerCommandBase {
        private static readonly SC_SetAllHPAndMP s_instance = new SC_SetAllHPAndMP ();
        public override NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.SET_ALL_HP_AND_MP; } }
        public override DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        List<int> m_allNetIdList;
        List < (int, int, int, int) > m_hpAndMaxHpAndMpAndMaxMpList;
        public static SC_SetAllHPAndMP Instance (IReadOnlyList<int> toClientList, List<int> allNetIdList, List < (int, int, int, int) > hpAndMaxHpAndMpAndMaxMpList) {
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
}