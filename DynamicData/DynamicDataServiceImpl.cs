using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using LitJson;
namespace MirRemakeBackend.DynamicData {
    class DynamicDataServiceImpl : IDDS_Item, IDDS_Skill, IDDS_Mission, IDDS_Character, IDDS_User, IDDS_CombatEfct, IDDS_CharacterVipCard, IDDS_CharacterPosition, IDDS_CharacterWallet, IDDS_CharacterAttribute, IDDS_Mail,IDDS_Notice, IDDS_Title,IDDS_MissionLog,IDDS_Shortcut,IDDS_Alliance {
        private SqlConfig sqlConfig;
        private SQLPool pool;
        private string database="legend";
        private string cmd;
        public DynamicDataServiceImpl () {
            sqlConfig = new SqlConfig ();
            sqlConfig.username = "root";
            sqlConfig.pwd = "root";
            sqlConfig.database="legend";
            sqlConfig.server = "localhost";
            pool = new SQLPool (sqlConfig);
        }
        // ITEM
        public List<DDO_Item> GetBagByCharacterId (int charId) {
            
            DataSet ds = new DataSet ();
            cmd = "select * from `item` where charid=" + charId + " and place=\"BAG\";";
            
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
            
            DataSet ds = new DataSet ();
            cmd = "select * from `item` where charid=" + charId + " and place=\"STORE_HOUSE\";";
            
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
            
            DataSet ds = new DataSet ();
            cmd = "select * from `item` where charid=" + charId + " and `place`=\"EQUIPMENT_REGION\";";
            
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
            
            DataSet ds = new DataSet ();
            cmd = "select * from `equipment` where charid=" + charId + ";";
            
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
                    string[] s = gems.Split (' ');
                    for (int j = 0; j < s.Length; j++) {
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
            
            cmd = "update `item` set itemid=" + item.m_itemId + ",`charid`=" + item.m_characterId + ",`num`=" + item.m_num + ",`place`=\"" + item.m_place.ToString () + "\",`position`=" + item.m_position + " where realid=" + item.m_realId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteItemByRealId (long realId) {
            
            cmd = "delete from `item` where realid=" + realId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public long InsertItem (DDO_Item item) {
            
            DataSet ds = new DataSet ();
            cmd = "insert into `item` values(null," + item.m_itemId + "," + item.m_characterId + "," + item.m_num + ",\"" + item.m_place.ToString () + "\"," + item.m_position + ");select last_insert_id();";
            
            pool.ExecuteSql (database, cmd, ds);
            if (ds.Tables[0].Rows.Count == 0) {
                return -1;
            }
            return long.Parse (ds.Tables[0].Rows[0]["last_insert_id()"].ToString ());
        }
        public void UpdateEquipmentInfo (DDO_EquipmentInfo eq) {
            
            string gems;
            if (eq.m_inlaidGemIdList.Count != 0) {
                gems = eq.m_inlaidGemIdList[0].ToString ();
                for (int i = 1; i < eq.m_inlaidGemIdList.Count; i++) {
                    gems = gems + " " + eq.m_inlaidGemIdList[i].ToString ();
                }
            } else {
                gems = "";
            }
            string enchantAttr = GetString (eq.m_enchantAttr);
            cmd = "update `equipment` set `charid`=" + eq.m_characterId + ", strength_num=" + eq.m_strengthNum + ", gem_list=\"" + gems + "\",enchant_attr=\"" + enchantAttr + "\" where realid=" + eq.m_realId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteEquipmentInfoByRealId (long realId) {
            cmd = "delete from `equipment` where `realid`=" + realId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public void InsertEquipmentInfo (DDO_EquipmentInfo eq) {
            
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
            cmd = "insert into `equipment` values("+eq.m_realId+"," + eq.m_characterId + "," + eq.m_strengthNum + ",\"" + gems + "\",\"" + enchantAttr + "\");";
            
            pool.ExecuteSql (database, cmd);
        }

        //ENCHANTMENT
        public void UpdateEnchantmentInfo (DDO_EnchantmentInfo e) {
            cmd = "update `enchantment` set `enchant_attr`=\"" + GetString (e.m_attrArr) + "\" where `realid`=" + e.m_realId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteEnchantmentInfoByRealId (long realId) {
            cmd = "delete from `enchantment` where `realid`=" + realId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public void InsertEnchantmentInfo (DDO_EnchantmentInfo enchantmentInfo) {
            cmd = "insert into `enchantment` values(null," + enchantmentInfo.m_characterId + ",\"" + GetString (enchantmentInfo.m_attrArr) + "\");";
            
            pool.ExecuteSql (database, cmd);
        }
        public List<DDO_EnchantmentInfo> GetAllEnchantmentByCharacterId (int charId) {
            List<DDO_EnchantmentInfo> res = new List<DDO_EnchantmentInfo> ();
            cmd = "select * from `enchantment` where `charid`=" + charId + ";";
            
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
            
            DataSet ds = new DataSet ();
            cmd = "select * from skill where charid=" + charId + ";";
            
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
            
            DataSet ds = new DataSet ();
            cmd = "select * from skill where charid=" + charId + " and skillid=" + ddo.m_skillId + ";";
            
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
            
            cmd = "insert into skill values(null," + skill.m_skillId + "," + skill.m_characterId + "," + skill.m_skillLevel + "," + skill.m_skillLevel + ");";
            
            pool.ExecuteSql (database, cmd);
        }

        //CHARACTER
        public int InsertCharacter (DDO_Character charDdo) {
            cmd = "insert into `character` values(null," + charDdo.m_playerId + ",\"" + charDdo.m_occupation.ToString () + "\",\"" + charDdo.m_name + "\");select last_insert_id();";
            
            DataSet ds = new DataSet ();
            try { pool.ExecuteSql (database, cmd, ds); } catch {return -1; }
            return int.Parse (ds.Tables[0].Rows[0]["last_insert_id()"].ToString ());
        }
        public bool DeleteCharacterById (int charId) {
            cmd = "delete from `character` where charid=" + charId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool UpdateCharacter (DDO_Character charDdo) {
            cmd = "update `character` set `occupation`=\"" + charDdo.m_occupation.ToString () + "\",`name`=\"" + charDdo.m_name + "\" where charid=" + charDdo.m_characterId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool GetCharacterById (int charid, out DDO_Character c) {
            cmd = "select * from `character` where charid=" + charid + ";";
            
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            if (ds.Tables[0].Rows.Count == 0) {
                c = default (DDO_Character);
                return false;
            }
            DataRow dr = ds.Tables[0].Rows[0];
            int id = int.Parse (dr["charid"].ToString ());
            int playerid = int.Parse (dr["playerid"].ToString ());
            OccupationType ocu = (OccupationType) Enum.Parse (typeof (OccupationType), dr["occupation"].ToString ());
            string name = dr["name"].ToString ();
            c = new DDO_Character (id, playerid, ocu, name);
            return true;

        }
        public DDO_Character[] GetCharacterByPlayerId (int playerId) {
            cmd = "select * from `character` where playerid=" + playerId + ";";
            
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            int x = ds.Tables[0].Rows.Count;
            DDO_Character[] res = new DDO_Character[x];
            for (int i = 0; i < x; i++) {
                DataRow dr = ds.Tables[0].Rows[i];
                int id = int.Parse (dr["charid"].ToString ());
                int playerid = int.Parse (dr["playerid"].ToString ());
                OccupationType ocu = (OccupationType) Enum.Parse (typeof (OccupationType), dr["occupation"].ToString ());
                string name = dr["name"].ToString ();
                res[i] = new DDO_Character (id, playerid, ocu, name);
            }
            return res;
        }

        public int[] GetAllCharacterId (){
            cmd="select `charid` from `character`;";
            
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            DataRowCollection drs=ds.Tables[0].Rows;
            int[] res=new int[drs.Count];
            for(int i=0;i<drs.Count;i++)
                res[i]=int.Parse(drs[i]["charid"].ToString());
            return res;
                
        }
        public int GetCharacterIdByName (string name){
            cmd=String.Format("select `charid` from `character` where `name`=\"{0}\";",name);
            
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            if(ds.Tables[0].Rows.Count==0){
                return -1;
            }
            return int.Parse(ds.Tables[0].Rows[0]["charid"].ToString());
        }
        
        
        //WALLET
        public bool InsertCharacterWallet (DDO_CharacterWallet w) {
            cmd = "insert into `wallet` values(" + w.m_characterId + "," + w.m_virtualCy + "," + w.m_chargeCy + ");";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool UpdateCharacterWallet (DDO_CharacterWallet w) {
            cmd = "update `wallet` set `virtual`=" + w.m_virtualCy + ",`charge`=" + w.m_chargeCy + " where charid=" + w.m_characterId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool DeleteCharacterWalletByCharacterId (int id) {
            cmd = "delete from `wallet` where `charid`=" + id + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool GetCharacterWalletByCharacterId (int id, out DDO_CharacterWallet w) {
            cmd = "select * from `wallet` where charid=" + id + ";";
            
            DataSet ds = new DataSet ();
            try {
                pool.ExecuteSql (database, cmd, ds);
                w = new DDO_CharacterWallet (id, int.Parse (ds.Tables[0].Rows[0]["virtual"].ToString ()), int.Parse (ds.Tables[0].Rows[0]["charge"].ToString ()));
                return true;
            } catch{
                w = default (DDO_CharacterWallet);
                return false;
            }
        }

        //ATTRIBUTES
        public bool InsertCharacterAttribute (DDO_CharacterAttribute charAttr) {
            string giftPoints = string.Format ("{0} {1} {2} {3}", charAttr.m_str, charAttr.m_intl, charAttr.m_sprt, charAttr.m_agl);
            cmd = "insert into `character_attribute` values(" + charAttr.m_characterId + "," + charAttr.m_level + "," + charAttr.m_experience + ",\"" + giftPoints + "\");";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool DeleteCharacterAttributeByCharacterId (int charId) {
            cmd = "delete from `character_attribute` where charid=" + charId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool UpdateCharacterAttribute (DDO_CharacterAttribute charAttr) {
            string giftPoints = string.Format ("{0} {1} {2} {3}", charAttr.m_str, charAttr.m_intl, charAttr.m_sprt, charAttr.m_agl);
            cmd = "update`character_attribute` set `level`=" + charAttr.m_level + ", `experience`=" + charAttr.m_experience + ", attributes=\"" + giftPoints + "\" where charid=" + charAttr.m_characterId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool GetCharacterAttributeByCharacterId (int charId, out DDO_CharacterAttribute resCharAttr) {
            cmd = "select * from `character_attribute` where charid=" + charId + ";";
            
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            if (ds.Tables[0].Rows.Count == 0) {
                resCharAttr = default (DDO_CharacterAttribute);
                return false;
            }
            DataRow dr = ds.Tables[0].Rows[0];
            short level = short.Parse (dr["level"].ToString ());
            int experience = int.Parse (dr["experience"].ToString ());
            ValueTuple<ActorUnitMainAttributeType, short>[] vt = new ValueTuple<ActorUnitMainAttributeType, short>[4];
            string[] strings = dr["attributes"].ToString ().Split (' ');
            short str = short.Parse (strings[0]);
            short intl = short.Parse (strings[1]);
            short sprt = short.Parse (strings[2]);
            short agl = short.Parse (strings[3]);
            resCharAttr = new DDO_CharacterAttribute (charId, level, experience, str, intl, sprt, agl);
            return true;
        }

        //MISSION
        public List<DDO_Mission> GetMissionListByCharacterId (int charId) {
            
            DataSet ds = new DataSet ();
            cmd = "select * from mission where charid=" + charId + ";";
            
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
            cmd = "insert into mission values(" + ddo.m_missionId + "," + ddo.m_characterId + ",\"" + target + "\",\"" + status + "\");";
            
            pool.ExecuteSql (database, cmd);
        }
        public void UpdateMission (DDO_Mission ddo) {
            
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
            
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteMission (short missionId, int charId) {
            
            cmd = "delete from `mission` where charid=" + charId + " and missionid=" + missionId + ";";
            
            pool.ExecuteSql (database, cmd);

        }

        public List<DDO_Mission> GetTitleMissionListByCharacterId (int charId){
            
            DataSet ds = new DataSet ();
            cmd = "select * from `title` where charid=" + charId + ";";
            
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            List<DDO_Mission> missions = new List<DDO_Mission> ();
            for (int i = 0; i < dt.Rows.Count; i++) {
                DDO_Mission mission = new DDO_Mission ();
                mission.m_missionId = short.Parse (dt.Rows[i]["titleid"].ToString ());
                mission.m_characterId = short.Parse (dt.Rows[i]["charid"].ToString ());
                mission.m_missionTargetProgressList = new List<int> ();
                string[] targets = dt.Rows[i]["target"].ToString ().Split (' ');
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
        public void InsertTitleMission (DDO_Mission ddo){
            string target;
            if (ddo.m_missionTargetProgressList.Count == 0) {
                target = "";
            } else {
                target = ddo.m_missionTargetProgressList[0].ToString ();
                for (int i = 1; i < ddo.m_missionTargetProgressList.Count; i++) {
                    target = target + " " + ddo.m_missionTargetProgressList[i].ToString ();
                }
            }
            cmd=String.Format("insert into `title` values({0},{1},\"{2}\",\"{3}\");",ddo.m_missionId,ddo.m_characterId,target,ddo.m_status.ToString());
            
            pool.ExecuteSql(database,cmd);
        }
        public void UpdateTitleMission (DDO_Mission ddo){
            
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
            cmd = String.Format("update `title` set `target`=\"{0}\",`status`=\"{1}\" where charid={2} and titleid={3};",target,ddo.m_status.ToString(),ddo.m_characterId,ddo.m_missionId);
            
            pool.ExecuteSql (database, cmd);
        }
        public void DeleteTitleMission (short missionId, int charId){
            
            cmd = "delete from `title` where charid=" + charId + " and titleid=" + missionId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        //CHARACTER POSITION
        public bool UpdateCharacterPosition (DDO_CharacterPosition cp) {
            
            DataSet ds = new DataSet ();
            cmd = "update character_position set `x`=\"" + cp.m_position.X + "\",`y`=\"" + cp.m_position.Y + "\" where charid=" + cp.m_characterId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool InsertCharacterPosition (DDO_CharacterPosition cp) {
            
            DataSet ds = new DataSet ();
            cmd = "insert into character_position values(" + cp.m_characterId + ",\"" + cp.m_position.X + "\",\"" + cp.m_position.Y + "\");";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool GetCharacterPosition (int charId, out DDO_CharacterPosition res) {
            
            DataSet ds = new DataSet ();
            cmd = "select * from character_position where charid=" + charId + ";";
            
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                res = default (DDO_CharacterPosition);
                return false;
            }
            float x = float.Parse (dt.Rows[0]["x"].ToString ());
            float y = float.Parse (dt.Rows[0]["y"].ToString ());
            res = new DDO_CharacterPosition (charId, new System.Numerics.Vector2 (x, y));
            return true;
        }
        public bool DeleteCharacterPositionByCharacterId (int charid) {
            
            DataSet ds = new DataSet ();
            cmd = "delete from character_position where `charid`=" + charid + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }

        //USER
        public bool GetUserByUsername (string username, out DDO_User resUser) {
            
            DataSet ds = new DataSet ();
            cmd = "select * from `user` where user_name=\"" + username + "\";";
            
            pool.ExecuteSql (database, cmd, ds);
            if (ds.Tables[0].Rows.Count == 0) {
                resUser = default (DDO_User);
                return false;
            }
            resUser = new DDO_User (int.Parse (ds.Tables[0].Rows[0]["userid"].ToString ()), username, ds.Tables[0].Rows[0]["password"].ToString (), ds.Tables[0].Rows[0]["question"].ToString (), ds.Tables[0].Rows[0]["answer"].ToString ());
            return true;
        }
        public bool UpdateUser (DDO_User ddo) {
            
            cmd = "update user set `user_name`=\"" + ddo.m_username + "\",`password`=\"" + ddo.m_pwd + "\",`question`=\"" + ddo.m_pwdProtectProblem + "\",`answer`=\"" + ddo.m_pwdProtectAnswer + "\" where `userid`=" + ddo.m_playerId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public int InsertUser (DDO_User ddo) {
            try {
                
                DataSet ds = new DataSet ();
                cmd = "insert into `user` values(null,\"" + ddo.m_username + "\",\"" + ddo.m_pwd + "\",\"" + ddo.m_pwdProtectProblem + "\",\"" + ddo.m_pwdProtectAnswer + "\");select last_insert_id();";
                
                //Console.WriteLine(cmd);
                pool.ExecuteSql (database, cmd, ds);
                return int.Parse (ds.Tables[0].Rows[0]["last_insert_id()"].ToString ());
            } catch{
                return -1;
            }
        }

        ///VIPCARD
        public bool InsertCharacterVipCard (DDO_CharacterVipCard vipCard) {
            
            cmd = "insert into `vip` values(" + vipCard.m_characterId + "," + vipCard.m_vipLevel + "," + vipCard.m_chargeMoney + ");";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;

        }
        public bool GetCharacterVipCardByCharacterId (int playerId, out DDO_CharacterVipCard card) {
            
            DataSet ds = new DataSet ();
            cmd = "select * from `vip` where `userid`=" + playerId + ";";
            
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                card = default (DDO_CharacterVipCard);
                return false;
            }
            card = new DDO_CharacterVipCard (int.Parse (dt.Rows[0]["userid"].ToString ()), int.Parse (dt.Rows[0]["vip_level"].ToString ()), int.Parse (dt.Rows[0]["charge_money"].ToString ()));
            return true;
        }
        public bool UpdateCharacterVipCard (DDO_CharacterVipCard vipCard) {
            
            cmd = "update `vip` set `vip_level`=" + vipCard.m_vipLevel + ",`charge_money`=" + vipCard.m_chargeMoney + " where `userid`=" + vipCard.m_characterId + ";";
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool DeleteCharacterVipCardByCharacterId (int characterid) {
            
            cmd = "delete from `vip` where `charid`=" + characterid + ";";
            
            pool.ExecuteSql (database, cmd);
            return true;
        }

        //COMBAT EFFECT
        public void InsertMixCombatEfct (DDO_CombatEfct mixCombatEfct) {
            cmd = "insert into `combat_effect` values(" + mixCombatEfct.m_charId + "," + mixCombatEfct.m_combatEfct + ",\"" + mixCombatEfct.m_name + "\",\"" + mixCombatEfct.m_ocp.ToString () + "\"," + mixCombatEfct.m_level + ");";
            
            pool.ExecuteSql (database, cmd);
        }
        public void UpdateMixCombatEfct (DDO_CombatEfct mixCombatEfct) {
            cmd = "update `combat_effect` set `combat`=" + mixCombatEfct.m_combatEfct + ",`name`=\"" + mixCombatEfct.m_name + "\", `occupation`=\"" + mixCombatEfct.m_ocp.ToString () + "\",level=" + mixCombatEfct.m_level + " where `charid`=" + mixCombatEfct.m_charId + ";";
            
            pool.ExecuteSql (database, cmd);
        }
        public DDO_CombatEfct[] GetAllMixCombatEfct () {
            cmd = "select * from `combat_effect`;";
            
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            DDO_CombatEfct[] res = new DDO_CombatEfct[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++) {
                res[i].m_charId = int.Parse (dt.Rows[i]["charid"].ToString ());
                res[i].m_combatEfct = int.Parse (dt.Rows[i]["combat"].ToString ());
                res[i].m_name = dt.Rows[i]["name"].ToString ();
                res[i].m_ocp = (OccupationType) Enum.Parse (typeof (OccupationType), dt.Rows[i]["occupation"].ToString ());
                res[i].m_level = short.Parse (dt.Rows[i]["level"].ToString ());
            }
            return res;
        }

        //MAIL
        public List<DDO_Mail> GetAllMailByReceiverCharacterId (int charId) {
            cmd = string.Format ("select * from `mail` where `receiverid`={0};", charId);
            
            DataSet ds = new DataSet ();
            pool.ExecuteSql (database, cmd, ds);
            List<DDO_Mail> res = new List<DDO_Mail> ();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++) {
                DDO_Mail mail = new DDO_Mail ();
                mail.m_receiverCharId = charId;
                mail.m_id = int.Parse (drs[i]["mailid"].ToString ());
                mail.m_detail = drs[i]["detail"].ToString ();
                mail.m_senderCharId = int.Parse (drs[i]["senderid"].ToString ());
                mail.m_title = drs[i]["title"].ToString ();
                mail.m_sendTime = Convert.ToDateTime (drs[i]["time"]);
                mail.m_senderName=drs[i]["sender_name"].ToString();
                mail.m_chargeCy=int.Parse(drs[i]["charge"].ToString());
                mail.m_virtualCy=int.Parse(drs[i]["virtual"].ToString());
                string str=drs[i]["item_array"].ToString ();
                if(str==""){
                    mail.m_itemIdAndNumArr=new (short,short)[0];
                }
                else{
                string[] strings = str.Split (',');
                mail.m_itemIdAndNumArr = new (short, short) [strings.Length];
                for (int j = 0; j < strings.Length; j++) {
                    string[] s = strings[j].Split (' ');
                    mail.m_itemIdAndNumArr[j].Item1 = short.Parse (s[0]);
                    mail.m_itemIdAndNumArr[j].Item2 = short.Parse (s[1]);
                }}
                string is_read = drs[i]["is_read"].ToString ();
                if (is_read == "0") mail.m_isRead = false;
                else mail.m_isRead = true;
                string is_received = drs[i]["is_received"].ToString ();
                if (is_read == "0") mail.m_isReceived = false;
                else mail.m_isReceived = true;
                res.Add (mail);
            }
            return res;

        }
        public bool DeleteMailById (int id) {
            cmd = string.Format ("delete from `mail` where `mailid`={0};", id);
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public int InsertMail (DDO_Mail mail) {
            string itemArray = "";
            DataSet ds=new DataSet();
            if (mail.m_itemIdAndNumArr.Length != 0) {
                itemArray = string.Format ("{0} {1}", mail.m_itemIdAndNumArr[0].Item1, mail.m_itemIdAndNumArr[0].Item2);
            }
            for (int i = 1; i < mail.m_itemIdAndNumArr.Length; i++) {
                itemArray = string.Format ("{0},{1} {2}", itemArray, mail.m_itemIdAndNumArr[i].Item1, mail.m_itemIdAndNumArr[i].Item2);
            }
            cmd = string.Format ("insert into `mail` values(null,{0},\"{1}\",{2},\"{3}\",\"{4}\",\"{5}\",{6},{7},\"{8}\",{9},{10});select last_insert_id();",
                mail.m_senderCharId, mail.m_senderName,mail.m_receiverCharId, mail.m_title, mail.m_detail, itemArray,mail.m_chargeCy ,mail.m_virtualCy, mail.m_sendTime.ToString ("yyyy-MM-dd HH:mm:ss"), mail.m_isRead?1 : 0, mail.m_isReceived?1 : 0);
            
            try { pool.ExecuteSql (database, cmd,ds); } catch { return -1; }
            return int.Parse(ds.Tables[0].Rows[0]["last_insert_id()"].ToString());
        }
        public bool UpdateMailRead (int id,bool isRead) {
            cmd = string.Format ("update `mail` set `is_read`={0} where `mailid`={1}",isRead?1:0,id);
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public bool UpdateMailReceived(int id,bool isReceived){
            cmd = string.Format ("update `mail` set `is_received`={0} where `mailid`={1}",isReceived?1:0,id);
            
            try { pool.ExecuteSql (database, cmd); } catch { return false; }
            return true;
        }
        public void DeleteMailBeforeCertainTime(DateTime time){
            cmd=string.Format("delete from `mail` where `time`<\"{0}\";",time.ToString("yyyy-MM-dd HH:mm:ss"));
            
            pool.ExecuteSql(database,cmd);
        }
        

        //NOTICE
        public List<DDO_Notice> GetAllNotice (){
            cmd="select * from `notice`;";
            
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            DataRowCollection drs=ds.Tables[0].Rows;
            List<DDO_Notice> res=new List<DDO_Notice>();
            for(int i=0;i<drs.Count;i++){
                DDO_Notice notice=new DDO_Notice();
                notice.m_time = Convert.ToDateTime (drs[i]["time"]);
                notice.m_title=drs[i]["title"].ToString();
                notice.m_detail=drs[i]["detail"].ToString();
                notice.m_id=int.Parse(drs[i]["noticeid"].ToString());
                res.Add(notice);
            }
            return res;
        }
        public int InsertNotice (DDO_Notice notice){
            string time=notice.m_time.ToString ("yyyy-MM-dd HH:mm:ss");
            cmd=String.Format("insert into `notice` values(null,\"{0}\",\"{1}\",\"{2}\");select last_insert_id();",time,notice.m_title,notice.m_detail);
            
            DataSet ds=new DataSet();
            try{pool.ExecuteSql(database,cmd,ds);}catch{return -1;}
            return int.Parse(ds.Tables[0].Rows[0]["last_insert_id()"].ToString());   
        }
        public bool DeleteNoticeById (int id){
            cmd=String.Format("delete from notice where noticeid={0};",id);
            
            try{pool.ExecuteSql(database,cmd);}catch{return false;};
            return true;
        }
        public void DeleteNoticeBeforeCertainTime (DateTime time){
            cmd=String.Format("delete from notice where `time`<\"{0}\";",time.ToString("yyyy-MM-dd HH:mm:ss"));
            
            pool.ExecuteSql(database,cmd);
        }


        //TITLE
        public bool InsertAttachedTitle (int charId, short misId){
            cmd=String.Format("insert into attached_title values({0},{1});",charId,misId);
            
            try{
                pool.ExecuteSql(database,cmd);
                return true;
            }
            catch{
                return false;
            }
        }
        public void UpdateAttachedTitle (int charId, short misId){
            cmd=String.Format("update attached_title set titleid={0} where charid={1};",misId,charId);
            
            pool.ExecuteSql(database,cmd);
        }
        public short GetAttachedTitle (int charId){
            cmd=String.Format("select titleid from attached_title where charid={0};",charId);
            
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            try{
                return short.Parse(ds.Tables[0].Rows[0]["titleid"].ToString());
            }
            catch{return -1;}
        }
        
        //MISSION LOG
        public List<DDO_MissionLog> GetMissionLogListByCharacterId (int charId){
            cmd=String.Format("select * from `mission_log` where charid={0};",charId);
            
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            List<DDO_MissionLog> missionLogs=new List<DDO_MissionLog>();
            DataRowCollection drc=ds.Tables[0].Rows;
            for(int i=0;i<drc.Count;i++){
                DDO_MissionLog ml=new DDO_MissionLog();
                ml.m_charId=charId;
                ml.m_misTarType=(MissionTargetType)Enum.Parse(typeof(MissionTargetType),drc[i]["target_type"].ToString());
                ml.m_parm1=int.Parse(drc[i]["parameter1"].ToString());
                ml.m_parm2=int.Parse(drc[i]["parameter2"].ToString());
                ml.m_parm3=int.Parse(drc[i]["parameter3"].ToString());
                missionLogs.Add(ml);
            }
            return missionLogs;
        }
        public void DeleteMissionLogByCharacterId (int charId){
            cmd=String.Format("delete from `mission_log` where charid={0};",charId);
            
            pool.ExecuteSql(database,cmd);
        }
        public void InsertMissionLog (DDO_MissionLog ddo){
            cmd=String.Format("insert into `mission_log` values({0},\"{1}\",{2},{3},{4});",
                ddo.m_charId,ddo.m_misTarType.ToString(),ddo.m_parm1,ddo.m_parm2,ddo.m_parm3);
            
            pool.ExecuteSql(database,cmd);
        }


        //SHORTCUT

        public List<DDO_Shortcut> GetShortcutsByCharId (int charId){
            cmd=String.Format("select * from shortcut where charid={0};",charId);
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            List<DDO_Shortcut> res=new List<DDO_Shortcut>();
            DataRowCollection drc = ds.Tables[0].Rows;
            for(int i=0;i<drc.Count;i++){
                DDO_Shortcut sc=new DDO_Shortcut();
                sc.m_charId=int.Parse(drc[i]["charid"].ToString());
                sc.m_position=byte.Parse(drc[i]["position"].ToString());
                sc.m_type=(ShortcutType)Enum.Parse(typeof(ShortcutType),drc[i]["type"].ToString());
                sc.m_data=int.Parse(drc[i]["data"].ToString());
                res.Add(sc);
            }
            return res;
        }
        public void InsertShortcut (DDO_Shortcut ddo){
            cmd=String.Format("insert into shortcut values({0},{1},\"{2}\",{3});",ddo.m_charId,ddo.m_position,ddo.m_type,ddo.m_data);
            pool.ExecuteSql(database,cmd);
        }
        public void UpdateShortcutByCharIdAndPosition (DDO_Shortcut ddo){
            cmd=String.Format("update shortcut set `type`=\"{0}\",`data`={1} where charid={2} and position={3};",ddo.m_type,ddo.m_data,ddo.m_charId,ddo.m_position);
            pool.ExecuteSql(database,cmd);
        }
        public void DeleteShortcutByCharId (int charId){
            cmd=String.Format("delete from shortcut where charid={0}",charId);
            pool.ExecuteSql(database,cmd);
        }


        //ALLIANCE

        public int InsertAlliance (DDO_Alliance ddo){
            cmd=String.Format("insert into alliance values(null,\"{0}\");select last_insert_id();",ddo.m_name);
            DataSet ds=new DataSet();
            try{
                pool.ExecuteSql(database,cmd,ds);
                DataRow dr=ds.Tables[0].Rows[0];
                return int.Parse(dr["last_insert_id()"].ToString());
            }catch{}
            return -1;
            
        }
        public void DeleteAllianceById (int id){
            cmd=String.Format("delete from alliance where allianceid={0}",id);
            pool.ExecuteSql(database,cmd);
        }
        public List<DDO_Alliance> GetAllAlliance (){
            cmd=String.Format("select * from alliance;");
            DataSet ds=new DataSet();
            List<DDO_Alliance> res=new List<DDO_Alliance>();
            pool.ExecuteSql(database,cmd,ds);
            DataRowCollection drs=ds.Tables[0].Rows;
            for(int i=0;i<drs.Count;i++){
                DDO_Alliance a=new DDO_Alliance();
                a.m_id=int.Parse(drs[i]["allianceid"].ToString());
                a.m_name=drs[i]["name"].ToString();
                res.Add(a);
            }
            return res;
        }


        //ALLIANCE APPLY
        public List<DDO_AllianceApply> GetApplyByAllianceId (int allianceId){
            cmd=String.Format("select * from alliance_apply where allianceid={0};",allianceId);
            DataSet ds=new DataSet();
            List<DDO_AllianceApply> res=new List<DDO_AllianceApply>();
            pool.ExecuteSql(database,cmd,ds);
            DataRowCollection drs=ds.Tables[0].Rows;
            for(int i=0;i<drs.Count;i++){
                DDO_AllianceApply aa=new DDO_AllianceApply();
                aa.m_id=int.Parse(drs[i]["applyid"].ToString());
                aa.m_charId=int.Parse(drs[i]["charid"].ToString());
                aa.m_allianceId=allianceId;
                res.Add(aa);
            }
            return res;
        }
        public bool DeleteApplyById (int id){
            cmd=String.Format("delete from alliance_apply where applyid={0};",id);
            try{
                pool.ExecuteSql(database,cmd);
                return true;
            }catch{return false;}
        }
        public int InsertApply (DDO_AllianceApply ddo){
            cmd=String.Format("insert into alliance_apply values(null,{0},{1});select last_insert_id();",ddo.m_charId,ddo.m_allianceId);
            DataSet ds=new DataSet();
            try{
                pool.ExecuteSql(database,cmd,ds);
                DataRow dr=ds.Tables[0].Rows[0];
                return int.Parse(dr["last_insert_id()"].ToString());
            }catch{}
            return -1;
        }

        // ALLIANCE MEMBER
        public List<DDO_AllianceMember> GetMemberByAllianceId (int allianceId){
            cmd=String.Format("select * from alliance_member where allianceid={0};",allianceId);
            DataSet ds=new DataSet();
            List<DDO_AllianceMember> res=new List<DDO_AllianceMember>();
            pool.ExecuteSql(database,cmd,ds);
            DataRowCollection drs=ds.Tables[0].Rows;
            for(int i=0;i<drs.Count;i++){
                DDO_AllianceMember aa=new DDO_AllianceMember();
                aa.m_charId=int.Parse(drs[i]["charid"].ToString());
                aa.m_allianceId=allianceId;
                aa.m_job=(AllianceJob)Enum.Parse(typeof(AllianceJob),drs[i]["alliance_job"].ToString());
                res.Add(aa);
            }
            return res;
        }
        public bool DeleteMemberByCharId (int charId){
            cmd=String.Format("delete from alliance_member where charid={0};",charId);
            try{
                pool.ExecuteSql(database,cmd);
                return true;
            }catch{return false;}
        }
        public void InsertMember (DDO_AllianceMember ddo){
            cmd=String.Format("insert into alliance_member values({0},{1},\"{2}\");",ddo.m_charId,ddo.m_allianceId,ddo.m_job);
            pool.ExecuteSql(database,cmd);
        }
        public void UpdateMember (DDO_AllianceMember ddo){
            cmd=String.Format("update alliance_member set alliance_job=\"{0}\" where charid={1} and allianceid={2}",ddo.m_job,ddo.m_charId,ddo.m_allianceId);
            pool.ExecuteSql(database,cmd);
        }

        




        private ValueTuple<ActorUnitConcreteAttributeType, int>[] GetAttr (JsonData attr) {
            ValueTuple<ActorUnitConcreteAttributeType, int>[] res = new ValueTuple<ActorUnitConcreteAttributeType, int>[attr.Count];
            for (int j = 0; j < attr.Count; j++) {
                string[]s=attr[j].ToString ().Split (' ');
                res[j] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                    ((ActorUnitConcreteAttributeType) Enum.Parse (typeof (ActorUnitConcreteAttributeType),  s[0]),
                        int.Parse (s[1]));
            }
            return res;
        }
        private string GetString (ValueTuple<ActorUnitConcreteAttributeType, int>[] ps) {
            if (ps.Length == 0) {
                return new String ("[]");
            }
            String res = new String ("");
            res = "[\"" + ps[0].Item1.ToString () + " " + ps[0].Item2.ToString () + "\"";
            for (int i = 1; i < ps.Length; i++) {
                res = res + ",\"" + ps[i].Item1.ToString () + " " + ps[i].Item2.ToString () + "\"";
            }

            res = res + "]";
            return res;
        }

    }
}