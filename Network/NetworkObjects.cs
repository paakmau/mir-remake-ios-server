using System;
using System.Collections.Generic;
using System.Numerics;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    struct NO_LoginCharacter {
        public int m_charId;
        public OccupationType m_ocp;
        public string m_name;
        public short m_level;
        public NO_LoginCharacter (int charId, OccupationType ocp, string name, short lv) {
            m_charId = charId;
            m_ocp = ocp;
            m_name = name;
            m_level = lv;
        }
    }
    struct NO_SkillParam {
        public int m_targetNetworkId;
        public Vector2 m_direction;
        public Vector2 m_position;
        public NO_SkillParam (int targetNetId, Vector2 direction, Vector2 position) {
            m_targetNetworkId = targetNetId;
            m_direction = direction;
            m_position = position;
        }
    }
    struct NO_Status {
        public short m_id;
        public float m_value;
        public float m_time;
        public NO_Status (short id, float value, float time) {
            m_id = id;
            m_value = value;
            m_time = time;
        }
    }
    struct NO_Effect {
        public short m_animId;
        public bool m_hit;
        public bool m_isCritical;
        public int m_deltaHp;
        public int m_deltaMp;
        public NO_Effect (short animId, bool hit, bool isCritical, int dHp, int dMp) {
            m_animId = animId;
            m_hit = hit;
            m_isCritical = isCritical;
            m_deltaHp = dHp;
            m_deltaMp = dMp;
        }
    }
    struct NO_Monster {
        public int m_netId;
        public Vector2 m_position;
        public short m_monsterId;
        public MonsterType m_monsterType;
        public NO_Monster (int netId, Vector2 pos, short monId, MonsterType monType) {
            m_netId = netId;
            m_position = pos;
            m_monsterId = monId;
            m_monsterType = monType;
        }
    }
    struct NO_SightCharacter {
        public int m_netId;
        public Vector2 m_position;
        public OccupationType m_occupation;
        public string m_name;
        public short m_level;
        public bool m_isMarket;
        public NO_SightCharacter (int netId, Vector2 pos, OccupationType ocp, string name, short lv, bool isMarket) {
            m_netId = netId;
            m_position = pos;
            m_occupation = ocp;
            m_name = name;
            m_level = lv;
            m_isMarket = isMarket;
        }
    }
    struct NO_AttributeCharacter {
        public int m_netId;
        public string m_name;
        public short m_level;
        public short m_str;
        public short m_intl;
        public short m_sprt;
        public short m_agl;
        public int m_maxHp;
        public int m_maxMp;
        public int m_dHpPerSecond;
        public int m_dMpPerSecond;
        public int m_atk;
        public int m_def;
        public int m_mag;
        public int m_res;
        public int m_tenacity;
        public int m_speed;
        public int m_criticalRate;
        public int m_criticalBonus;
        public int m_hitRate;
        public int m_dodgeRate;
        public int m_lifeSteal;
        public int m_damageReduction;
        public NO_AttributeCharacter (int netId, string name, short lv, short str, short intl, short sprt, short agl, int maxHp, int maxMp, int dHp, int dMp, int atk, int def, int mag, int res, int tenacity, int speed, int criticalRate, int criticalBonus, int hitRate, int dodgeRate, int lifeSteal, int dmgReduction) {
            m_netId = netId;
            m_name = name;
            m_level = lv;
            m_str = str;
            m_intl = intl;
            m_sprt = sprt;
            m_agl = agl;
            m_maxHp = maxHp;
            m_maxMp = maxMp;
            m_dHpPerSecond = dHp;
            m_dMpPerSecond = dMp;
            m_atk = atk;
            m_def = def;
            m_mag = mag;
            m_res = res;
            m_tenacity = tenacity;
            m_speed = speed;
            m_criticalRate = criticalRate;
            m_criticalBonus = criticalBonus;
            m_hitRate = hitRate;
            m_dodgeRate = dodgeRate;
            m_lifeSteal = lifeSteal;
            m_damageReduction = dmgReduction;
        }
    }
    struct NO_DamageRankCharacter {
        public int m_charId;
        public string m_name;
        public int m_damage;
        public NO_DamageRankCharacter (int charId, string name, int dmg) {
            m_charId = charId;
            m_name = name;
            m_damage = dmg;
        }
    }
    struct NO_MarketItem {
        public long m_realId;
        public short m_itemId;
        public short m_onSaleNum;
        public long m_virtualCyPrice;
        public long m_chargeCyPrice;
        public NO_MarketItem (long realId, short itemId, short onSaleNum, long virtualCyPrice, long chargeCyPrice) {
            m_realId = realId;
            m_itemId = itemId;
            m_onSaleNum = onSaleNum;
            m_virtualCyPrice = virtualCyPrice;
            m_chargeCyPrice = chargeCyPrice;
        }
    }
    struct NO_MallClass {
        public byte m_mallClassId;
        public string m_mallName;
        public List<NO_MallItem> m_mallItemList;
        public NO_MallClass (byte mallClassId, string mallName, List<NO_MallItem> itemList) {
            m_mallClassId = mallClassId;
            m_mallName = mallName;
            m_mallItemList = itemList;
        }
    }
    struct NO_MallItem {
        public int m_mallItemId;
        public short m_itmeId;
        /// <summary> 为-1则为不可用虚拟币支付 </summary>
        public long m_virtualCyPrice;
        /// <summary> 为-1则为不可用充值币支付 </summary>
        public long m_chargeCyPrice;
        public NO_MallItem (int mallItemId, short itemId, long virtualCyPrice, long chargeCyPrice) {
            m_mallItemId = mallItemId;
            m_itmeId = itemId;
            m_virtualCyPrice = virtualCyPrice;
            m_chargeCyPrice = chargeCyPrice;
        }
    }
    struct NO_GroundItem {
        public long m_groundItemId;
        public short m_itemId;
        public short m_num;
        public int m_charId;
        public Vector2 m_pos;
        public NO_GroundItem (long gndItemId, short itemId, short num, int charId, Vector2 pos) {
            m_groundItemId = gndItemId;
            m_itemId = itemId;
            m_num = num;
            m_charId = charId;
            m_pos = pos;
        }
    }
    struct NO_Item {
        public ItemPlace m_ip;
        public short m_position;
        public long m_realId;
        public short m_itemId;
        public short m_num;
        public NO_Item (ItemPlace ip, short pos, long realId, short itemId, short num) {
            m_ip = ip;
            m_position = pos;
            m_realId = realId;
            m_itemId = itemId;
            m_num = num;
        }
    }
    struct NO_EquipmentItemInfo {
        public long m_realId;
        public byte m_strengthNum;
        public IReadOnlyList < (ActorUnitConcreteAttributeType, int) > m_enchantAttrList;
        public IReadOnlyList<short> m_inlaidGemIdList;
        public NO_EquipmentItemInfo (
            long realId,
            byte strengthNum,
            IReadOnlyList < (ActorUnitConcreteAttributeType, int) > enchantAttrList,
            IReadOnlyList<short> gemIdList
        ) {
            m_realId = realId;
            m_strengthNum = strengthNum;
            m_enchantAttrList = enchantAttrList;
            m_inlaidGemIdList = gemIdList;
        }
    }
    struct NO_EnchantmentItemInfo {
        public long m_realId;
        public IReadOnlyList < (ActorUnitConcreteAttributeType, int) > m_attrList;
        public NO_EnchantmentItemInfo (long realId, IReadOnlyList < (ActorUnitConcreteAttributeType, int) > attrList) {
            m_realId = realId;
            m_attrList = attrList;
        }
    }
    struct NO_Repository {
        public IReadOnlyList<NO_Item> m_itemList;
        public IReadOnlyList<NO_EquipmentItemInfo> m_equipmentInfoList;
        public NO_Repository (IReadOnlyList<NO_Item> itemList, IReadOnlyList<NO_EquipmentItemInfo> equipsList) {
            m_itemList = itemList;
            m_equipmentInfoList = equipsList;
        }
    }
    struct NO_Mission {
        public short m_misId;
        public IReadOnlyList<int> m_targetProgressList;
        public NO_Mission (short misId, IReadOnlyList<int> tarProgressList) {
            m_misId = misId;
            m_targetProgressList = tarProgressList;
        }
    }
    struct NO_FightCapacityRankInfo {
        public int m_charId;
        public string m_name;
        public short m_level;
        /// <summary>排名</summary>
        public short m_rank;
        /// <summary>战力</summary>
        public int m_fightCapacity;
        /// <summary>家族(TODO: 扩展用, 暂时不用管)</summary>
        public string m_family;
        public byte m_occupation;
        public NO_FightCapacityRankInfo (int charId, string name, short level, short rank, int fightCapacity, string family, byte occupation) {
            m_charId = charId;
            m_family = family;
            m_fightCapacity = fightCapacity;
            m_level = level;
            m_name = name;
            m_rank = rank;
            m_occupation = occupation;
        }
    }
    struct NO_Mail {
        public int m_mailId;
        public int m_senderCharId;
        public string m_senderName;
        public int m_recvCharId;
        public DateTime m_sendTime;
        public string m_title;
        public string m_detail;
        public List < (short, short) > m_itemIdAndNumList;
        public long m_virtualCy;
        public long m_chargeCy;
        public bool m_isRead;
        public bool m_isReceived;
        public NO_Mail (int id, int senderCharId, string senderName, int recvCharId, DateTime sendTime, string title, string detail, List < (short, short) > itemIdAndNumList, long virtualCy, long chargeCy, bool isRead, bool isReceived) {
            m_mailId = id;
            m_senderCharId = senderCharId;
            m_senderName = senderName;
            m_recvCharId = recvCharId;
            m_sendTime = sendTime;
            m_title = title;
            m_detail = detail;
            m_itemIdAndNumList = itemIdAndNumList;
            m_virtualCy = virtualCy;
            m_chargeCy = chargeCy;
            m_isRead = isRead;
            m_isReceived = isReceived;
        }
    }
    static class NetworkObjectExtensions {
        public static void Put (this NetDataWriter writer, Vector2 value) {
            writer.Put (value.X);
            writer.Put (value.Y);
        }
        public static Vector2 GetVector2 (this NetDataReader reader) {
            return new Vector2 (reader.GetFloat (), reader.GetFloat ());
        }
        public static void Put (this NetDataWriter writer, DateTime value) {
            writer.Put (value.ToString ());
        }
        public static DateTime GetDateTime (this NetDataReader reader) {
            return DateTime.Parse (reader.GetString ());
        }
        public static void Put (this NetDataWriter writer, NO_LoginCharacter value) {
            writer.Put (value.m_charId);
            writer.Put ((byte) value.m_ocp);
            writer.Put (value.m_name);
            writer.Put (value.m_level);
        }
        public static NO_LoginCharacter GetLoginCharacter (this NetDataReader reader) {
            int charId = reader.GetInt ();
            OccupationType ocp = (OccupationType) reader.GetByte ();
            string name = reader.GetString ();
            short lv = reader.GetShort ();
            return new NO_LoginCharacter (charId, ocp, name, lv);
        }
        public static void Put (this NetDataWriter writer, NO_SkillParam value) {
            writer.Put (value.m_targetNetworkId);
            writer.Put (value.m_direction);
            writer.Put (value.m_position);
        }
        public static NO_SkillParam GetSkillParam (this NetDataReader reader) {
            return new NO_SkillParam (reader.GetInt (), reader.GetVector2 (), reader.GetVector2 ());
        }
        public static void Put (this NetDataWriter writer, NO_Status status) {
            writer.Put (status.m_id);
            writer.Put (status.m_value);
            writer.Put (status.m_time);
        }
        public static NO_Status GetStatus (this NetDataReader reader) {
            short statusId = reader.GetShort ();
            int value = reader.GetInt ();
            float time = reader.GetFloat ();
            return new NO_Status (statusId, value, time);
        }
        public static void Put (this NetDataWriter writer, NO_Effect effect) {
            writer.Put (effect.m_animId);
            writer.Put (effect.m_hit);
            writer.Put (effect.m_isCritical);
            writer.Put (effect.m_deltaHp);
            writer.Put (effect.m_deltaMp);
        }
        public static NO_Effect GetEffect (this NetDataReader reader) {
            short animId = reader.GetShort ();
            bool isHit = reader.GetBool ();
            bool isCritical = reader.GetBool ();
            int dHp = reader.GetInt ();
            int dMp = reader.GetInt ();
            return new NO_Effect (animId, isHit, isCritical, dHp, dMp);
        }
        public static void Put (this NetDataWriter writer, NO_Monster monNo) {
            writer.Put (monNo.m_netId);
            writer.Put (monNo.m_position);
            writer.Put (monNo.m_monsterId);
            writer.Put ((byte) monNo.m_monsterType);
        }
        public static NO_Monster GetMonster (this NetDataReader reader) {
            int netId = reader.GetInt ();
            Vector2 pos = reader.GetVector2 ();
            short monsterId = reader.GetShort ();
            MonsterType monType = (MonsterType) reader.GetByte ();
            return new NO_Monster (netId, pos, monsterId, monType);
        }
        public static void Put (this NetDataWriter writer, NO_SightCharacter charNo) {
            writer.Put (charNo.m_netId);
            writer.Put (charNo.m_position);
            writer.Put ((byte) charNo.m_occupation);
            writer.Put (charNo.m_name);
            writer.Put (charNo.m_level);
            writer.Put (charNo.m_isMarket);
        }
        public static NO_SightCharacter GetSightCharacter (this NetDataReader reader) {
            int netId = reader.GetInt ();
            Vector2 pos = reader.GetVector2 ();
            OccupationType ocp = (OccupationType) reader.GetByte ();
            string name = reader.GetString ();
            short lv = reader.GetShort ();
            bool isMarket = reader.GetBool ();
            return new NO_SightCharacter (netId, pos, ocp, name, lv, isMarket);
        }
        public static void Put (this NetDataWriter writer, NO_AttributeCharacter attrChar) {
            writer.Put (attrChar.m_netId);
            writer.Put (attrChar.m_name);
            writer.Put (attrChar.m_level);
            writer.Put (attrChar.m_str);
            writer.Put (attrChar.m_intl);
            writer.Put (attrChar.m_sprt);
            writer.Put (attrChar.m_agl);
            writer.Put (attrChar.m_maxHp);
            writer.Put (attrChar.m_maxMp);
            writer.Put (attrChar.m_dHpPerSecond);
            writer.Put (attrChar.m_dMpPerSecond);
            writer.Put (attrChar.m_atk);
            writer.Put (attrChar.m_def);
            writer.Put (attrChar.m_mag);
            writer.Put (attrChar.m_res);
            writer.Put (attrChar.m_tenacity);
            writer.Put (attrChar.m_speed);
            writer.Put (attrChar.m_criticalRate);
            writer.Put (attrChar.m_criticalBonus);
            writer.Put (attrChar.m_hitRate);
            writer.Put (attrChar.m_dodgeRate);
            writer.Put (attrChar.m_lifeSteal);
            writer.Put (attrChar.m_damageReduction);
        }
        public static NO_AttributeCharacter GetAttributeCharacter (this NetDataReader reader) {
            int netId = reader.GetInt ();
            string name = reader.GetString ();
            short lv = reader.GetShort ();
            short str = reader.GetShort ();
            short intl = reader.GetShort ();
            short sprt = reader.GetShort ();
            short agl = reader.GetShort ();
            int maxHp = reader.GetInt ();
            int maxMp = reader.GetInt ();
            int dHp = reader.GetInt ();
            int dMp = reader.GetInt ();
            int atk = reader.GetInt ();
            int def = reader.GetInt ();
            int mag = reader.GetInt ();
            int res = reader.GetInt ();
            int tenacity = reader.GetInt ();
            int speed = reader.GetInt ();
            int criticalRate = reader.GetInt ();
            int criticalBonus = reader.GetInt ();
            int hitRate = reader.GetInt ();
            int dodgeRate = reader.GetInt ();
            int lifeSteal = reader.GetInt ();
            int dmgReduction = reader.GetInt ();
            return new NO_AttributeCharacter (netId, name, lv, str, intl, sprt, agl, maxHp, maxMp, dHp, dMp, atk, def, mag, res, tenacity, speed, criticalRate, criticalBonus, hitRate, dodgeRate, lifeSteal, dmgReduction);
        }
        public static void Put (this NetDataWriter writer, NO_DamageRankCharacter dmgRankChar) {
            writer.Put (dmgRankChar.m_charId);
            writer.Put (dmgRankChar.m_name);
            writer.Put (dmgRankChar.m_damage);
        }
        public static NO_DamageRankCharacter GetDamageRankCharacter (this NetDataReader reader) {
            int charId = reader.GetInt ();
            string name = reader.GetString ();
            int dmg = reader.GetInt ();
            return new NO_DamageRankCharacter (charId, name, dmg);
        }
        public static void Put (this NetDataWriter writer, NO_MarketItem marketItem) {
            writer.Put (marketItem.m_realId);
            writer.Put (marketItem.m_itemId);
            writer.Put (marketItem.m_onSaleNum);
            writer.Put (marketItem.m_virtualCyPrice);
            writer.Put (marketItem.m_chargeCyPrice);
        }
        public static NO_MarketItem GetMarketItem (this NetDataReader reader) {
            long realId = reader.GetLong ();
            short itemId = reader.GetShort ();
            short onSaleNum = reader.GetShort ();
            long virtualCyPrice = reader.GetLong ();
            long chargeCyPrice = reader.GetLong ();
            return new NO_MarketItem (realId, itemId, onSaleNum, virtualCyPrice, chargeCyPrice);
        }
        public static void Put (this NetDataWriter writer, NO_MallClass mallClass) {
            writer.Put (mallClass.m_mallClassId);
            writer.Put (mallClass.m_mallName);
            writer.Put ((byte) mallClass.m_mallItemList.Count);
            for (int i = 0; i < mallClass.m_mallItemList.Count; i++)
                writer.Put (mallClass.m_mallItemList[i]);
        }
        public static NO_MallClass GetMallClass (this NetDataReader reader) {
            byte mallClassId = reader.GetByte ();
            string mallName = reader.GetString ();
            byte mallItemCnt = reader.GetByte ();
            List<NO_MallItem> itemList = new List<NO_MallItem> (mallItemCnt);
            for (int i = 0; i < mallItemCnt; i++)
                itemList.Add (reader.GetMallItem ());
            return new NO_MallClass (mallClassId, mallName, itemList);
        }
        public static void Put (this NetDataWriter writer, NO_MallItem mallItem) {
            writer.Put (mallItem.m_mallItemId);
            writer.Put (mallItem.m_itmeId);
            writer.Put (mallItem.m_virtualCyPrice);
            writer.Put (mallItem.m_chargeCyPrice);
        }
        public static NO_MallItem GetMallItem (this NetDataReader reader) {
            int mallItemId = reader.GetInt ();
            short itemId = reader.GetShort ();
            long virtualCyPrice = reader.GetLong ();
            long chargeCyPrice = reader.GetLong ();
            return new NO_MallItem (mallItemId, itemId, virtualCyPrice, chargeCyPrice);
        }
        public static void Put (this NetDataWriter writer, NO_GroundItem gndItem) {
            writer.Put (gndItem.m_groundItemId);
            writer.Put (gndItem.m_itemId);
            writer.Put (gndItem.m_num);
            writer.Put (gndItem.m_charId);
            writer.Put (gndItem.m_pos);
        }
        public static NO_GroundItem GetGroundItem (this NetDataReader reader) {
            long gndItemId = reader.GetLong ();
            short itemId = reader.GetShort ();
            short num = reader.GetShort ();
            int charId = reader.GetInt ();
            Vector2 pos = reader.GetVector2 ();
            return new NO_GroundItem (gndItemId, itemId, num, charId, pos);
        }
        public static void Put (this NetDataWriter writer, NO_Item item) {
            writer.Put ((byte) item.m_ip);
            writer.Put (item.m_position);
            writer.Put (item.m_realId);
            writer.Put (item.m_itemId);
            writer.Put (item.m_num);
        }
        public static NO_Item GetItem (this NetDataReader reader) {
            ItemPlace ip = (ItemPlace) reader.GetByte ();
            short pos = reader.GetShort ();
            long realId = reader.GetLong ();
            short itemId = reader.GetShort ();
            short num = reader.GetShort ();
            return new NO_Item (ip, pos, realId, itemId, num);
        }
        public static void Put (this NetDataWriter writer, NO_EquipmentItemInfo equipInfo) {
            writer.Put (equipInfo.m_realId);
            writer.Put (equipInfo.m_strengthNum);
            writer.Put ((byte) equipInfo.m_enchantAttrList.Count);
            for (int i = 0; i < equipInfo.m_enchantAttrList.Count; i++) {
                writer.Put ((byte) equipInfo.m_enchantAttrList[i].Item1);
                writer.Put (equipInfo.m_enchantAttrList[i].Item2);
            }
            writer.Put ((byte) equipInfo.m_inlaidGemIdList.Count);
            for (int i = 0; i < equipInfo.m_inlaidGemIdList.Count; i++)
                writer.Put (equipInfo.m_inlaidGemIdList[i]);
        }
        public static NO_EquipmentItemInfo GetEquipmentItemInfo (this NetDataReader reader) {
            long realId = reader.GetLong ();
            byte strengthNum = reader.GetByte ();
            byte enchantAttrNum = reader.GetByte ();
            var enchantAttrList = new List < (ActorUnitConcreteAttributeType, int) > (enchantAttrNum);
            for (int i = 0; i < enchantAttrNum; i++) {
                ActorUnitConcreteAttributeType attrType = (ActorUnitConcreteAttributeType) reader.GetByte ();
                int attrValue = reader.GetInt ();
                enchantAttrList.Add ((attrType, attrValue));
            }
            byte gemNum = reader.GetByte ();
            var gemIdList = new List<short> (gemNum);
            for (int i = 0; i < gemNum; i++)
                gemIdList.Add (reader.GetShort ());
            return new NO_EquipmentItemInfo (realId, strengthNum, enchantAttrList, gemIdList);
        }
        public static void Put (this NetDataWriter writer, NO_EnchantmentItemInfo enchantmentInfo) {
            writer.Put (enchantmentInfo.m_realId);
            writer.Put ((byte) enchantmentInfo.m_attrList.Count);
            for (int i = 0; i < enchantmentInfo.m_attrList.Count; i++) {
                writer.Put ((byte) enchantmentInfo.m_attrList[i].Item1);
                writer.Put (enchantmentInfo.m_attrList[i].Item2);
            }
        }
        public static NO_EnchantmentItemInfo GetEnchantmentItemInfo (this NetDataReader reader) {
            long realId = reader.GetLong ();
            byte enchantAttrNum = reader.GetByte ();
            var enchantAttrList = new List < (ActorUnitConcreteAttributeType, int) > (enchantAttrNum);
            for (int i = 0; i < enchantAttrNum; i++) {
                ActorUnitConcreteAttributeType attrType = (ActorUnitConcreteAttributeType) reader.GetByte ();
                int attrValue = reader.GetInt ();
                enchantAttrList.Add ((attrType, attrValue));
            }
            return new NO_EnchantmentItemInfo (realId, enchantAttrList);
        }
        public static void Put (this NetDataWriter writer, NO_Repository repo) {
            writer.Put ((byte) repo.m_itemList.Count);
            for (int i = 0; i < repo.m_itemList.Count; i++)
                writer.Put (repo.m_itemList[i]);
            writer.Put ((byte) repo.m_equipmentInfoList.Count);
            for (int i = 0; i < repo.m_equipmentInfoList.Count; i++)
                writer.Put (repo.m_equipmentInfoList[i]);
        }
        public static NO_Repository GetRepository (this NetDataReader reader) {
            byte itemNum = reader.GetByte ();
            var itemList = new List<NO_Item> ();
            for (int i = 0; i < itemNum; i++)
                itemList.Add (reader.GetItem ());
            byte equipNum = reader.GetByte ();
            var equipList = new List<NO_EquipmentItemInfo> ();
            for (int i = 0; i < equipNum; i++)
                equipList.Add (reader.GetEquipmentItemInfo ());
            return new NO_Repository (itemList, equipList);
        }
        public static void Put (this NetDataWriter writer, NO_Mission mission) {
            writer.Put (mission.m_misId);
            writer.Put ((byte) mission.m_targetProgressList.Count);
            for (int i = 0; i < mission.m_targetProgressList.Count; i++)
                writer.Put (mission.m_targetProgressList[i]);
        }
        public static NO_Mission GetMission (this NetDataReader reader) {
            short misId = reader.GetShort ();
            byte misTarCnt = reader.GetByte ();
            List<int> misTarProgressList = new List<int> (misTarCnt);
            for (int i = 0; i < misTarCnt; i++)
                misTarProgressList.Add (reader.GetInt ());
            return new NO_Mission (misId, misTarProgressList);
        }
        public static void Put (this NetDataWriter writer, NO_FightCapacityRankInfo fcri) {
            writer.Put (fcri.m_charId);
            writer.Put (fcri.m_name);
            writer.Put (fcri.m_rank);
            writer.Put (fcri.m_level);
            writer.Put (fcri.m_fightCapacity);
            writer.Put (fcri.m_family);
            writer.Put (fcri.m_occupation);
        }
        public static NO_FightCapacityRankInfo GetFightCapacityRankInfo (this NetDataReader reader) {
            int charId = reader.GetInt ();
            string name = reader.GetString ();
            short rank = reader.GetShort ();
            short level = reader.GetShort ();
            int fightCapacity = reader.GetInt ();
            string family = reader.GetString ();
            byte occupation = reader.GetByte ();
            return new NO_FightCapacityRankInfo (charId, name, level, rank, fightCapacity, family, occupation);
        }
        public static void Put (this NetDataWriter writer, NO_Mail value) {
            writer.Put (value.m_mailId);
            writer.Put (value.m_senderCharId);
            writer.Put (value.m_senderName);
            writer.Put (value.m_recvCharId);
            writer.Put (value.m_sendTime);
            writer.Put (value.m_title);
            writer.Put (value.m_detail);
            writer.Put ((byte) value.m_itemIdAndNumList.Count);
            for (int i = 0; i < value.m_itemIdAndNumList.Count; i++) {
                writer.Put (value.m_itemIdAndNumList[i].Item1);
                writer.Put (value.m_itemIdAndNumList[i].Item2);
            }
            writer.Put (value.m_virtualCy);
            writer.Put (value.m_chargeCy);
            writer.Put (value.m_isRead);
            writer.Put (value.m_isReceived);
        }
        public static NO_Mail GetMail (this NetDataReader reader) {
            int mailId = reader.GetInt ();
            int senderCharId = reader.GetInt ();
            string senderName = reader.GetString ();
            int recvCharId = reader.GetInt ();
            DateTime sendTime = reader.GetDateTime ();
            string title = reader.GetString ();
            string detail = reader.GetString ();
            byte itemCount = reader.GetByte ();
            List < (short, short) > itemIdAndNum = new List < (short, short) > (itemCount);
            for (int i = 0; i < itemCount; i++)
                itemIdAndNum.Add ((reader.GetShort (), reader.GetShort ()));
            long virtualCy = reader.GetLong ();
            long chargeCy = reader.GetLong ();
            bool isRead = reader.GetBool ();
            bool isReceived = reader.GetBool ();
            return new NO_Mail (mailId, senderCharId, senderName, recvCharId, sendTime, title, detail, itemIdAndNum, virtualCy, chargeCy, isRead, isReceived);
        }
    }
}