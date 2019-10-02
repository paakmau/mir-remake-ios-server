using System;
using System.Collections.Generic;
using System.Numerics;

namespace MirRemakeBackend.DynamicData {
    struct DDO_User {
        public int m_playerId;
        public string m_username;
        public string m_pwd;
        public string m_pwdProtectProblem;
        public string m_pwdProtectAnswer;
        public DDO_User (int playerId, string username, string pwd, string pwdProtectProblem, string pwdProtectAnswer) {
            m_playerId = playerId;
            m_username = username;
            m_pwd = pwd;
            m_pwdProtectProblem = pwdProtectProblem;
            m_pwdProtectAnswer = pwdProtectAnswer;
        }
    }
    struct DDO_Character {
        public int m_characterId;
        public int m_playerId;
        public OccupationType m_occupation;
        public string m_name;
        public DDO_Character (int charId, int playerId, OccupationType oc, string name) {
            m_characterId = charId;
            m_playerId = playerId;
            m_occupation = oc;
            m_name = name;
        }
    }
    struct DDO_CharacterAttribute {
        public int m_characterId;
        public short m_level;
        public int m_experience;
        // 已分配的主属性点情况
        public short m_str;
        public short m_intl;
        public short m_sprt;
        public short m_agl;
        public DDO_CharacterAttribute (int charId, short lv, int exp, short str, short intl, short sprt, short agl) {
            m_characterId = charId;
            m_level = lv;
            m_experience = exp;
            m_str = str;
            m_intl = intl;
            m_sprt = sprt;
            m_agl = agl;
        }
    }
    struct DDO_CharacterPosition {
        public int m_characterId;
        public Vector2 m_position;
        public DDO_CharacterPosition (int charId, Vector2 pos) {
            m_characterId = charId;
            m_position = pos;
        }
    }
    struct DDO_CharacterWallet {
        public int m_characterId;
        public long m_virtualCy;
        public long m_chargeCy;
        public DDO_CharacterWallet (int charId, long virtualCy, long chargeCy) {
            m_characterId = charId;
            m_virtualCy = virtualCy;
            m_chargeCy = chargeCy;
        }
    }
    struct DDO_CharacterVipCard {
        public int m_characterId;
        public int m_vipLevel;
        public int m_chargeMoney;
        public DDO_CharacterVipCard (int charId, int vipLevel, int chargeMoney) {
            m_characterId = charId;
            m_vipLevel = vipLevel;
            m_chargeMoney = chargeMoney;
        }
    }
    struct DDO_CombatEfct {
        public int m_charId;
        public int m_combatEfct;
        public OccupationType m_ocp;
        public string m_name;
        public short m_level;
        public DDO_CombatEfct (int charId, int combatEfct, OccupationType ocp, string name, short lv) {
            m_charId = charId;
            m_combatEfct = combatEfct;
            m_ocp = ocp;
            m_name = name;
            m_level = lv;
        }
    }
    struct DDO_EquipmentInfo {
        public long m_realId;
        public int m_characterId;
        /// <summary>
        /// 强化次数
        /// </summary>
        public byte m_strengthNum;
        /// <summary>
        /// 装备附魔的属性
        /// 可以为null
        /// </summary>
        public (ActorUnitConcreteAttributeType, int) [] m_enchantAttr;
        /// <summary>
        /// 镶嵌的宝石Id列表
        /// </summary>
        public List<short> m_inlaidGemIdList;
        public DDO_EquipmentInfo (long realId, int charId, byte strNum, List < (ActorUnitConcreteAttributeType, int) > enchantAttr, List<short> inlaidGemIds) {
            m_realId = realId;
            m_characterId = charId;
            m_strengthNum = strNum;
            m_enchantAttr = enchantAttr.ToArray ();
            m_inlaidGemIdList = inlaidGemIds;
        }
    }
    struct DDO_EnchantmentInfo {
        public long m_realId;
        public int m_characterId;
        public (ActorUnitConcreteAttributeType, int) [] m_attrArr;
        public DDO_EnchantmentInfo (long realId, int charId, List < (ActorUnitConcreteAttributeType, int) > attrList) {
            m_realId = realId;
            m_characterId = charId;
            m_attrArr = attrList.ToArray ();
        }
    }
    struct DDO_Item {
        public long m_realId;
        public short m_itemId;
        public int m_characterId;
        public short m_num;
        /// <summary>
        /// 该物品所在的地点 (背包, 仓库, 装备区, 地面)
        /// </summary>
        public ItemPlace m_place;
        /// <summary>
        /// 该物品在地点中的位置 (第几格)
        /// </summary>
        public short m_position;
        public DDO_Item (long realId, short itemId, int charId, short num, ItemPlace place, short pos) {
            m_realId = realId;
            m_itemId = itemId;
            m_characterId = charId;
            m_num = num;
            m_place = place;
            m_position = pos;
        }
    }
    struct DDO_Skill {
        public short m_skillId;
        public int m_characterId;
        public short m_skillLevel;
        public int m_masterly;
        public DDO_Skill (short skillId, int charId, short skillLv, int masterly) {
            m_skillId = skillId;
            m_characterId = charId;
            m_skillLevel = skillLv;
            m_masterly = masterly;
        }
    }

    struct DDO_Mission {
        public short m_missionId;
        public int m_characterId;
        public MissionStatus m_status;
        public List<int> m_missionTargetProgressList;
        public DDO_Mission (short misId, int charId, MissionStatus status, List<int> misProgList) {
            m_missionId = misId;
            m_characterId = charId;
            m_status = status;
            m_missionTargetProgressList = misProgList;
        }
    }

    struct DDO_MissionLog {
        public MissionTargetType m_misTarType;
        public int m_charId;
        public int m_parm1;
        public int m_parm2;
        public int m_parm3;
        public DDO_MissionLog (MissionTargetType type, int charId, int parm1, int parm2, int parm3) {
            m_misTarType = type;
            m_charId = charId;
            m_parm1 = parm1;
            m_parm2 = parm2;
            m_parm3 = parm3;
        }
    }

    struct DDO_Mail {
        public int m_id;
        public int m_senderCharId;
        public string m_senderName;
        public int m_receiverCharId;
        public DateTime m_sendTime;
        public string m_title;
        public string m_detail;
        public (short, short) [] m_itemIdAndNumArr;
        public long m_virtualCy;
        public long m_chargeCy;
        public bool m_isRead;
        public bool m_isReceived;
        public DDO_Mail (int id, int senderCharId, string senderName, int receiverCharId, DateTime sendTime, string title, string detail, (short, short) [] itemIdAndNum, long virtualCy, long chargeCy, bool isRead, bool isReceived) {
            m_id = id;
            m_senderCharId = senderCharId;
            m_senderName = senderName;
            m_receiverCharId = receiverCharId;
            m_sendTime = sendTime;
            m_title = title;
            m_detail = detail;
            m_itemIdAndNumArr = itemIdAndNum;
            m_virtualCy = virtualCy;
            m_chargeCy = chargeCy;
            m_isRead = isRead;
            m_isReceived = isReceived;
        }
    }

    struct DDO_Notice {
        public int m_id;
        public DateTime m_time;
        public string m_title;
        public string m_detail;
        public DDO_Notice (int id, DateTime time, string title, string detail) {
            m_id = id;
            m_time = time;
            m_title = title;
            m_detail = detail;
        }
    }

    struct DDO_Shortcut {
        public int m_charId;
        /// <summary> 快捷键位置 </summary>
        public byte m_position;
        public ShortcutType m_type;
        /// <summary> 快捷键数据, 视类型不同而不同, 如物品RealId, 技能id </summary>
        public long m_data;
        public DDO_Shortcut (int charId, byte pos, ShortcutType type, long data) {
            m_charId = charId;
            m_position = pos;
            m_type = type;
            m_data = data;
        }
    }
}