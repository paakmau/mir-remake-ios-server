using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using LitJson;
namespace MirRemakeBackend.DynamicData {
    class DynamicDataServiceImpl : IDDS_Item, IDDS_Skill, IDDS_Mission, IDDS_Character, IDDS_CharacterPosition, IDDS_User, IDDS_CombatEfct,IDDS_VipCard {
        private SqlConfig sqlConfig;
        private SQLPool pool;
        public DynamicDataServiceImpl () {
            sqlConfig = new SqlConfig ();
            sqlConfig.username = "root";
            sqlConfig.pwd = "root";
            sqlConfig.database = "legend";
            sqlConfig.server = "localhost";
            pool = new SQLPool (sqlConfig);
        }
        // ITEM
        public List<DDO_Item> GetBagByCharacterId (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `item` where charid=" + charId + " and place=\"BAG\";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_Item> res = new List<DDO_Item> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                short realid = short.Parse (dt.Rows[i]["realid"].ToString ());
                short itemid = short.Parse (dt.Rows[i]["itemid"].ToString ());
                short num = short.Parse (dt.Rows[i]["num"].ToString ());
                ItemPlace place = ItemPlace.BAG;
                short pos = short.Parse (dt.Rows[i]["position"].ToString ());
                res.Add (new DDO_Item (realid, itemid, charId, num, place, pos));
            }
            return res;
        }
        public List<DDO_Item> GetStoreHouseByCharacterId (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `item` where charid=" + charId + " and place=\"STORE_HOUSE\";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_Item> res = new List<DDO_Item> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                short realid = short.Parse (dt.Rows[i]["realid"].ToString ());
                short itemid = short.Parse (dt.Rows[i]["itemid"].ToString ());
                short num = short.Parse (dt.Rows[i]["num"].ToString ());
                ItemPlace place = ItemPlace.STORE_HOUSE;
                short pos = short.Parse (dt.Rows[i]["position"].ToString ());
                res.Add (new DDO_Item (realid, itemid, charId, num, place, pos));
            }
            return res;
        }
        public List<DDO_Item> GetEquipmentRegionByCharacterId (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `item` where charid=" + charId + " and `place`=\"EQUIPMENT_REGION\";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_Item> res = new List<DDO_Item> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                short realid = short.Parse (dt.Rows[i]["realid"].ToString ());
                short itemid = short.Parse (dt.Rows[i]["itemid"].ToString ());
                short num = short.Parse (dt.Rows[i]["num"].ToString ());
                ItemPlace place = ItemPlace.STORE_HOUSE;
                short pos = short.Parse (dt.Rows[i]["position"].ToString ());
                res.Add (new DDO_Item (realid, itemid, charId, num, place, pos));
            }
            return res;
        }
        public List<DDO_EquipmentInfo> GetAllEquipmentByCharacterId (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `equipment` where charid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_EquipmentInfo> res = new List<DDO_EquipmentInfo> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                DDO_EquipmentInfo equipment = new DDO_EquipmentInfo ();
                equipment.m_strengthNum = byte.Parse (dt.Rows[i]["strength_num"].ToString ());
                //equipment.m_holeNum = short.Parse (dt.Rows[i]["hole_num"].ToString ());
                equipment.m_realId = short.Parse (dt.Rows[i]["realid"].ToString ());
                string gems = dt.Rows[i]["gem_list"].ToString ();
                equipment.m_inlaidGemIdList = new List<short> ();
                if (gems.Length != 0) {
                    string[] s=gems.Split(' ');
                    for (int j = 0; j <s.Length; j++) {
                        equipment.m_inlaidGemIdList.Add (short.Parse (s[j]));
                    }
                }
                JsonData attr = JsonMapper.ToObject (dt.Rows[i]["enchant_attr"].ToString ());
                equipment.m_enchantAttr = GetAttr (attr);
                res.Add (equipment);
            }
            return res;
        }
        public void UpdateItem (DDO_Item item) {
            string cmd;
            cmd = "update `item` set itemid=" + item.m_itemId + ",`charid`=" + item.m_characterId + ",`num`=" + item.m_num + ",`place`=\"" + item.m_place.ToString () + "\",`position`=" + item.m_position + " where realid=" + item.m_realId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteItemByRealId (long realId) {
            string cmd;
            cmd = "delete from `item` where realid=" + realId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public long InsertItem (DDO_Item item) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "insert into `item` values(null," + item.m_itemId + "," + item.m_characterId + "," + item.m_num + ",\"" + item.m_place.ToString () + "\"," + item.m_position + ");select last_insert_id();";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            if (ds.Tables[0].Rows.Count == 0) {
                throw new Exception ();
            }
            return int.Parse (ds.Tables[0].Rows[0]["last_insert_id()"].ToString ());
        }
        public void UpdateEquipmentInfo (DDO_EquipmentInfo eq) {
            string cmd;
            string gems;
            if (eq.m_inlaidGemIdList.Count != 0) {
                gems = eq.m_inlaidGemIdList[0].ToString ();
                for (int i = 0; i < eq.m_inlaidGemIdList.Count; i++) {
                    gems = gems + " " + eq.m_inlaidGemIdList[i].ToString ();
                }
            } else {
                gems = "";
            }
            string enchantAttr = GetString (eq.m_enchantAttr);
            cmd = "update `equipment` set `charid`=" + eq.m_characterId + ", strength_num=" + eq.m_strengthNum + ", gem_list=\"" + gems + "\",enchant_attr=\"" + enchantAttr + "\" where realid=" + eq.m_realId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteEquipmentInfoByRealId (long realId) {
            string cmd = "delete from `equipment` where `realid`=" + realId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void InsertEquipmentInfo (DDO_EquipmentInfo eq) {
            string cmd;
            string gems;
            if (eq.m_inlaidGemIdList.Count != 0) {
                gems = eq.m_inlaidGemIdList[0].ToString ();
                for (int i = 0; i < eq.m_inlaidGemIdList.Count; i++) {
                    gems = gems + " " + eq.m_inlaidGemIdList[i].ToString ();
                }
            } else {
                gems = "";
            }
            string enchantAttr = GetString (eq.m_enchantAttr);
            cmd = "insert into `equipment` values(null," + eq.m_characterId + "," + eq.m_strengthNum + "," + gems + ",\"" + enchantAttr + "\");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }



        //ENCHANTMENT
        public void UpdateEnchantmentInfo (DDO_EnchantmentInfo e) {
            string cmd = "update `enchantment` set `enchant_attr`=\"" + GetString (e.m_attrArr) + "\" where `realid`=" + e.m_realId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteEnchantmentInfoByRealId (long realId) {
            string cmd = "delete from `enchantment` where `realid`=" + realId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void InsertEnchantmentInfo (DDO_EnchantmentInfo enchantmentInfo) {
            string cmd = "insert into `enchantment` values(null," + enchantmentInfo.m_characterId + ",\"" + GetString (enchantmentInfo.m_attrArr) + "\");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public List<DDO_EnchantmentInfo> GetAllEnchantmentByCharacterId (int charId) {
            List<DDO_EnchantmentInfo> res = new List<DDO_EnchantmentInfo> ();
            string cmd = "select * from `enchantment` where `charid`=" + charId + ";";
            string database = "legend";
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++) {
                DDO_EnchantmentInfo e = new DDO_EnchantmentInfo ();
                e.m_realId = long.Parse (dt.Rows[i]["realid"].ToString ());
                e.m_characterId = int.Parse (dt.Rows[i]["charid"].ToString ());
                e.m_attrArr = GetAttr (dt.Rows[i]["enchant_attr"].ToString ());
                res.Add (e);
            }
            return res;
        }

        public List<DDO_Skill> GetSkillListByCharacterId (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from skill where charid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_Skill> res = new List<DDO_Skill> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                DDO_Skill skill = new DDO_Skill ();
                skill.m_skillId = short.Parse (dt.Rows[i]["skillid"].ToString ());
                skill.m_masterly = int.Parse (dt.Rows[i]["masterly"].ToString ());
                skill.m_skillLevel = short.Parse (dt.Rows[i]["level"].ToString ());
                res.Add (skill);
            }
            return res;
        }

        //SKILL
        public void UpdateSkill (DDO_Skill ddo) {
            int charId = ddo.m_characterId;
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from skill where charid=" + charId + " and skillid=" + ddo.m_skillId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count != 0) {
                cmd = "update skill set masterly=" + ddo.m_masterly + ", level=" + ddo.m_skillLevel + " where charid=" + charId + " and skillid=" + ddo.m_skillId + ";";
            } else {
                cmd = "insert into skill values(null," + ddo.m_skillId + "," + charId + "," + ddo.m_masterly + "," + ddo.m_skillLevel + ")";
            }
            pool.ExecuteSql (database, cmd);
        }
        public void InsertSkill (DDO_Skill skill) {
            string cmd;
            cmd = "insert into skill values(null," + skill.m_skillId + "," + skill.m_characterId + "," + skill.m_skillLevel + "," + skill.m_skillLevel + ");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        
        

        //CHARACTER
        public int CreateCharacter (int playerid,OccupationType occupation,String name) {
            string cmd;
            DataSet ds = new DataSet ();
            DataTable dt = new DataTable ();
            cmd = "insert into `character` values (null,"+playerid+",\"" + occupation.ToString () + "\",1,0,\"0 0\",\"0 0 0 0\",\""+name+"\");select last_insert_id();";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            dt = ds.Tables[0];
            return int.Parse (dt.Rows[0]["last_insert_id()"].ToString ());
        }
        
        public DDO_Character GetCharacterById (int characterId) {
            DDO_Character character = new DDO_Character ();
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `character` where characterid=" + characterId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                throw new Exception ();
            }
            character.m_currencyArr = new ValueTuple<CurrencyType, long>[2];
            String[] strings=dt.Rows[0]["currency"].ToString ().Split (' ') ;
            character.m_currencyArr[0] = new ValueTuple<CurrencyType, long> (CurrencyType.VIRTUAL, long.Parse (strings[0]));
            character.m_currencyArr[1] = new ValueTuple<CurrencyType, long> (CurrencyType.CHARGE, long.Parse (strings[1]));
            character.m_distributedMainAttrPointArr = new ValueTuple<ActorUnitMainAttributeType, short>[4];
            strings=dt.Rows[0]["giftpoints"].ToString ().Split (' ');
            character.m_distributedMainAttrPointArr[0] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.STRENGTH, short.Parse ( strings[0]));
            character.m_distributedMainAttrPointArr[1] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.AGILITY, short.Parse (strings[1]));
            character.m_distributedMainAttrPointArr[2] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.INTELLIGENCE, short.Parse (strings[2]));
            character.m_distributedMainAttrPointArr[3] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.SPIRIT, short.Parse (strings[3]));
            character.m_level = short.Parse (dt.Rows[0]["level"].ToString ());
            character.m_occupation = (OccupationType) Enum.Parse (typeof (OccupationType), dt.Rows[0]["occupation"].ToString ());
            character.m_experience = int.Parse (dt.Rows[0]["experience"].ToString ());
            character.m_characterId = int.Parse (dt.Rows[0]["characterid"].ToString ());
            character.m_name = dt.Rows[0]["name"].ToString ();
            character.m_playerId=int.Parse(dt.Rows[0]["playerid"].ToString());
            return character;
        }
        public void UpdateCharacter (DDO_Character charObj) {
            string cmd;
            string currencyArr = "\"" + charObj.m_currencyArr[0].Item2.ToString () + " " + charObj.m_currencyArr[1].Item2.ToString () + "\"";
            string giftPoints = "\"" + charObj.m_distributedMainAttrPointArr[0].Item2.ToString () + " " +
                charObj.m_distributedMainAttrPointArr[1].Item2.ToString () + " " +
                charObj.m_distributedMainAttrPointArr[2].Item2.ToString () + " " +
                charObj.m_distributedMainAttrPointArr[3].Item2.ToString () + "\"";
            cmd = "update `character` set `level`=" + charObj.m_level + ",experience=" +
                charObj.m_experience + ",currency=" + currencyArr + ",giftpoints=" + giftPoints + ",`name`=\"" + charObj.m_name + "\" where characterid=" + charObj.m_characterId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public DDO_Character[] GetCharacterByPlayerId(int playerid){
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `character` where playerid=" + playerid + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            DDO_Character[] res=new DDO_Character[dt.Rows.Count];
            for(int i=0;i<dt.Rows.Count;i++){
                res[i].m_currencyArr = new ValueTuple<CurrencyType, long>[2];
                String[] strings=dt.Rows[i]["currency"].ToString ().Split (' ') ;
                res[i].m_currencyArr[0] = new ValueTuple<CurrencyType, long> (CurrencyType.VIRTUAL, long.Parse (strings [0]));
                res[i].m_currencyArr[1] = new ValueTuple<CurrencyType, long> (CurrencyType.CHARGE, long.Parse (strings[1]));
                res[i].m_distributedMainAttrPointArr = new ValueTuple<ActorUnitMainAttributeType, short>[4];
                strings=dt.Rows[0]["giftpoints"].ToString ().Split (' ');
                res[i].m_distributedMainAttrPointArr[0] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.STRENGTH, short.Parse (strings[0]));
                res[i].m_distributedMainAttrPointArr[1] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.AGILITY, short.Parse (strings[1]));
                res[i].m_distributedMainAttrPointArr[2] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.INTELLIGENCE, short.Parse (strings[2]));
                res[i].m_distributedMainAttrPointArr[3] = new ValueTuple<ActorUnitMainAttributeType, short> (ActorUnitMainAttributeType.SPIRIT, short.Parse (strings[3]));
                res[i].m_level = short.Parse (dt.Rows[0]["level"].ToString ());
                res[i].m_occupation = (OccupationType) Enum.Parse (typeof (OccupationType), dt.Rows[i]["occupation"].ToString ());
                res[i].m_experience = int.Parse (dt.Rows[i]["experience"].ToString ());
                res[i].m_characterId = int.Parse (dt.Rows[i]["characterid"].ToString ());
                res[i].m_name = dt.Rows[i]["name"].ToString ();
                res[i].m_playerId=playerid;
            }
            return res;
        }


        //MISSION
        public List<DDO_Mission> GetMissionListByCharacterId (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from mission where charid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_Mission> missions = new List<DDO_Mission> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                DDO_Mission mission = new DDO_Mission ();
                mission.m_missionId = short.Parse (dt.Rows[i]["missionid"].ToString ());
                mission.m_characterId = short.Parse (dt.Rows[i]["charid"].ToString ());
                mission.m_missionTargetProgressList = new List<int> ();
                string[] targets = dt.Rows[i]["targets"].ToString ().Split (' ');
                mission.m_missionTargetProgressList = new List<int> ();
                if (targets[0] != "") {
                    for (int j = 0; j < targets.Length; j++) {
                        mission.m_missionTargetProgressList.Add (int.Parse (targets[j]));
                    }
                }
                mission.m_status = (MissionStatus) Enum.Parse (typeof (MissionStatus), dt.Rows[i]["status"].ToString ());
                missions.Add (mission);
            }
            return missions;
        }
        public void InsertMission (DDO_Mission ddo) {
            string cmd;
            string target;
            if (ddo.m_missionTargetProgressList.Count == 0) {
                target = "";
            } else {
                target = ddo.m_missionTargetProgressList[0].ToString ();
                for (int i = 1; i < ddo.m_missionTargetProgressList.Count; i++) {
                    target = target + " " + ddo.m_missionTargetProgressList[i].ToString ();
                }
            }
            string status = ddo.m_status.ToString ();
            cmd = "insert into mission values(null," + ddo.m_missionId + "," + ddo.m_characterId + ",\"" + target + "\",\"" + status + "\");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void UpdateMission (DDO_Mission ddo) {
            string cmd;
            string target;
            if (ddo.m_missionTargetProgressList.Count == 0) {
                target = "";
            } else {
                target = ddo.m_missionTargetProgressList[0].ToString ();
                for (int i = 1; i < ddo.m_missionTargetProgressList.Count; i++) {
                    target = target + " " + ddo.m_missionTargetProgressList[i].ToString ();
                }
            }
            string status = ddo.m_status.ToString ();
            cmd = "update mission set targets=\"" + target + "\",`status`=\"" + status + "\" where charid=" + ddo.m_characterId + " and missionid=" + ddo.m_missionId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteMission (short missionId, int charId) {
            string cmd;
            cmd = "delete from `mission` where charid=" + charId + " and missionid=" + missionId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);

        }



        //CHARACTER POSITION
        public void UpdateCharacterPosition (DDO_CharacterPosition cp) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "update character_position set `x`=\"" + cp.m_position.X + "\",`y`=\"" + cp.m_position.Y + "\" where charid=" + cp.m_characterId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void InsertCharacterPosition (DDO_CharacterPosition cp) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "insert into character_position values(" + cp.m_characterId + ",\"" + cp.m_position.X + "\",\"" + cp.m_position.Y + "\");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }

        public DDO_CharacterPosition GetCharacterPosition (int charId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from character_position where charid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                throw new Exception ();
            }
            float x = float.Parse (dt.Rows[0]["x"].ToString ());
            float y = float.Parse (dt.Rows[0]["y"].ToString ());
            DDO_CharacterPosition cp = new DDO_CharacterPosition (charId, new System.Numerics.Vector2 (x, y));
            return cp;
        }


        //USER
        public bool GetUserByUsername (string username, out DDO_User resUser) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `user` where user_name=\"" + username + "\";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            if (ds.Tables[0].Rows.Count == 0) {
                resUser = default (DDO_User);
                return false;
            }
            resUser = new DDO_User (int.Parse (ds.Tables[0].Rows[0]["userid"].ToString ()),username, ds.Tables[0].Rows[0]["password"].ToString (),ds.Tables[0].Rows[0]["question"].ToString(),ds.Tables[0].Rows[0]["answer"].ToString());
            return true;
        }
        public void UpdateUser (DDO_User ddo) {
            string cmd;
            cmd = "update user set `user_name`=\"" + ddo.m_username + "\",`password`=\"" + ddo.m_pwd + "\",`question`=\""+ddo.m_pwdProtectProblem+"\",`answer`=\""+ddo.m_pwdProtectAnswer+"\" where `userid`=" + ddo.m_playerId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public int InsertUser (DDO_User ddo) {
            try {
                string cmd;
                DataSet ds = new DataSet ();
                cmd = "insert into `user` values(null,\"" + ddo.m_username + "\",\"" + ddo.m_pwd + "\",\""+ddo.m_pwdProtectProblem+"\",\""+ddo.m_pwdProtectAnswer+"\");select last_insert_id();";
                string database = "legend";
                //Console.WriteLine(cmd);
                pool.ExecuteSql (database, cmd, ds);
                return int.Parse (ds.Tables[0].Rows[0]["last_insert_id()"].ToString ());
            } catch (Exception e) {
                Console.Write (e.StackTrace);
                return -1;
            }
        }


        ///VIPCARD
        public void InsertVipCard (DDO_VipCard vipCard) {
            string cmd;
            cmd = "insert into `vip` values(" + vipCard.m_playerId + "," + vipCard.m_vipLevel + ","+vipCard.m_chargeMoney+");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public DDO_VipCard GetVipCardByPlayerId (int playerId) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `vip` where `userid`="+playerId+";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                throw new Exception ();
            }
            DDO_VipCard res = new DDO_VipCard (int.Parse (dt.Rows[0]["userid"].ToString ()), int.Parse (dt.Rows[0]["vip_level"].ToString ()),long.Parse(dt.Rows[0]["charge_money"].ToString()));
            return res;
        }
        public void UpdateVipCard (DDO_VipCard vipCard) {
            string cmd;
            cmd = "update `vip` set `vip_level`=" + vipCard.m_vipLevel + ",`charge_money`="+vipCard.m_chargeMoney+" where `userid`=" + vipCard.m_playerId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }



        //COMBAT EFFECT
        public void InsertMixCombatEfct (DDO_CombatEfct mixCombatEfct) {
            string cmd = "insert into `combat_effect` values(" + mixCombatEfct.m_charId + "," + mixCombatEfct.m_combatEfct +",\""+mixCombatEfct.m_name+"\",\""+mixCombatEfct.m_ocp.ToString()+ "\");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void UpdateMixCombatEfct (DDO_CombatEfct mixCombatEfct) {
            string cmd = "update `combat_effect` set `combat`=" + mixCombatEfct.m_combatEfct + ",`name`=\""+mixCombatEfct.m_name+"\", `occupation`=\""+mixCombatEfct.m_ocp.ToString()+"\" where `charid`=" + mixCombatEfct.m_charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public DDO_CombatEfct[] GetAllMixCombatEfct () {
            string cmd = "select * from `combat_effect`;";
            string database = "legend";
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            DDO_CombatEfct[] res = new DDO_CombatEfct[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++) {
                res[i].m_charId = int.Parse (dt.Rows[i]["charid"].ToString ());
                res[i].m_combatEfct = int.Parse (dt.Rows[i]["combat"].ToString ());
                res[i].m_name=dt.Rows[i]["name"].ToString();
                res[i].m_ocp=(OccupationType)Enum.Parse(typeof(OccupationType),dt.Rows[i]["occupation"].ToString());
            }
            return res;
        }

        private ValueTuple<ActorUnitConcreteAttributeType, int>[] GetAttr (JsonData attr) {
            ValueTuple<ActorUnitConcreteAttributeType, int>[] res = new ValueTuple<ActorUnitConcreteAttributeType, int>[attr.Count];
            for (int j = 0; j < attr.Count; j++) {
                res[j] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                    ((ActorUnitConcreteAttributeType) Enum.Parse (typeof (ActorUnitConcreteAttributeType), attr[j].ToString ().Split (' ') [0]),
                        int.Parse (attr[j].ToString ().Split (' ') [1]));
            }
            return res;
        }
        private string GetString (ValueTuple<ActorUnitConcreteAttributeType, int>[] ps) {
            if (ps.Length == 0) {
                return new String ("");
            }
            String res = new String ("");
            res = "[\"" + ps[0].Item1.ToString () + "," + ps[0].Item2.ToString () + "\"";
            for (int i = 1; i < ps.Length; i++) {
                res = res + ",\"" + ps[0].Item1.ToString () + "," + ps[0].Item2.ToString () + "\"";
            }

            res = res + "]";
            return res;
        }

    }
}