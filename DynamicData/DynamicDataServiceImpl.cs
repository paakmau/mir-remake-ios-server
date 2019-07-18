using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using LitJson;
namespace MirRemakeBackend.DynamicData {
    class DynamicDataServiceImpl : IDDS_Item, IDDS_Skill, IDDS_Mission, IDDS_Character, IDDS_User, IDDS_CombatEfct,IDDS_CharacterVipCard,IDDS_CharacterPosition,IDDS_CharacterWallet,IDDS_CharacterAttribute {
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
        public int InsertCharacter (DDO_Character charDdo){
            string cmd="insert into `character` values(null,"+charDdo.m_playerId+",\""+charDdo.m_occupation.ToString()+"\",\""+charDdo.m_name+"\");select last_insert_id();";
            string database="legend";
            DataSet ds=new DataSet();
            try{pool.ExecuteSql(database,cmd,ds);}catch(Exception e){Console.WriteLine(e.StackTrace);return -1;}
            return int.Parse(ds.Tables[0].Rows[0]["last_insert_id()"].ToString());
        }
        public bool DeleteCharacterById (int charId){
            string cmd="delete from `character` where charid="+charId+";";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool UpdateCharacter (DDO_Character charDdo){
            string cmd="update `character` set `occupation`=\""+charDdo.m_occupation.ToString()+"\",`name`=\""+charDdo.m_name+"\" where charid="+charDdo.m_characterId+";";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool GetCharacterById (int charid,out DDO_Character c){
            string cmd="select * from `character` where charid="+charid+";";
            string database="legend";
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            if(ds.Tables[0].Rows.Count==0){
                c=default(DDO_Character);return false;
            }
            DataRow dr=ds.Tables[0].Rows[0];
            int id=int.Parse(dr["charid"].ToString());
            int playerid=int.Parse(dr["playerid"].ToString());
            OccupationType ocu=(OccupationType)Enum.Parse(typeof(OccupationType),dr["occupation"].ToString());
            string name=dr["name"].ToString();
            c=new DDO_Character(id,playerid,ocu,name);
            return true;

        }
        public DDO_Character[] GetCharacterByPlayerId (int playerId){
            string cmd="select * from `character` where playerid="+playerId+";";
            string database="legend";
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            int x=ds.Tables[0].Rows.Count;
            DDO_Character[] res=new DDO_Character[x];
            for(int i=0;i<x;i++){
                DataRow dr=ds.Tables[0].Rows[i];
                int id=int.Parse(dr["charid"].ToString());
                int playerid=int.Parse(dr["playerid"].ToString());
                OccupationType ocu=(OccupationType)Enum.Parse(typeof(OccupationType),dr["occupation"].ToString());
                string name=dr["name"].ToString();
                res[x]=new DDO_Character(id,playerid,ocu,name);
            }
            return res;
        }


        //WALLET
        public bool InsertCharacterWallet(DDO_CharacterWallet w){
            string cmd="insert into `wallet` values("+w.m_characterId+","+w.m_virtualCy+","+w.m_chargeCy+");";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool UpdateCharacterWallet(DDO_CharacterWallet w){
            string cmd="update `wallet` set `virtual`="+w.m_virtualCy+",`charge`="+w.m_chargeCy+" where charid="+w.m_characterId+";";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool DeleteCharacterWalletByCharacterId(int id){
            string cmd="delete from `wallet` where `charid`="+id+";";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool GetCharacterWalletByCharacterId(int id,out DDO_CharacterWallet w){
            string cmd="select * from `wallet` where charid="+id+";";
            string database="legend";
            DataSet ds=new DataSet();
            try{
                pool.ExecuteSql(database,cmd,ds);
                w=new DDO_CharacterWallet(id,int.Parse(ds.Tables[0].Rows[0]["virtual"].ToString()),int.Parse(ds.Tables[0].Rows[0]["charge"].ToString()));
                return true;
            }catch(Exception e){
                Console.WriteLine(e.StackTrace);
                w=default(DDO_CharacterWallet);
                return false;
            }
        }

        //ATTRIBUTES
        public bool InsertCharacterAttribute (DDO_CharacterAttribute charAttr){
            string giftPoints=charAttr.m_distributedMainAttrPointArr[0].Item2.ToString();
            for(int i=1;i<4;i++){
                giftPoints=giftPoints+" "+charAttr.m_distributedMainAttrPointArr[i].Item2.ToString();
            }
            string cmd="insert into `character_attribute` values("+charAttr.m_characterId+","+charAttr.m_level+","+charAttr.m_experience+",\""+giftPoints+"\");";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool DeleteCharacterAttributeByCharacterId (int charId){
            string cmd="delete from `character_attribute` where charid="+charId+";";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool UpdateCharacterAttribute (DDO_CharacterAttribute charAttr){
            string giftPoints=charAttr.m_distributedMainAttrPointArr[0].Item2.ToString();
            for(int i=1;i<4;i++){
                giftPoints=giftPoints+" "+charAttr.m_distributedMainAttrPointArr[i].Item2.ToString();
            }
            string cmd="update`character_attribute` set `level`="+charAttr.m_level+", `experience`="+charAttr.m_experience+", attributes=\""+giftPoints+"\" where charid="+charAttr.m_characterId+";";
            string database="legend";
            try{pool.ExecuteSql(database,cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool GetCharacterAttributeByCharacterId (int charId, out DDO_CharacterAttribute resCharAttr){
            string cmd="select * from `character_attribute` where charid="+charId+";";
            string database="legend";
            DataSet ds=new DataSet();
            pool.ExecuteSql(database,cmd,ds);
            if(ds.Tables[0].Rows.Count==0){
                resCharAttr=default(DDO_CharacterAttribute);
                return false;
            }
            DataRow dr=ds.Tables[0].Rows[0];
            short level=short.Parse(dr["level"].ToString());
            int experience=int.Parse(dr["experience"].ToString());
            ValueTuple<ActorUnitMainAttributeType,short>[] vt=new ValueTuple<ActorUnitMainAttributeType,short>[4]; 
            string[] strings=dr["attributes"].ToString().Split(' ');
            vt[0].Item1=ActorUnitMainAttributeType.STRENGTH;
            vt[1].Item1=ActorUnitMainAttributeType.AGILITY;
            vt[2].Item1=ActorUnitMainAttributeType.INTELLIGENCE;
            vt[3].Item1=ActorUnitMainAttributeType.SPIRIT;
            for(int i=0;i<4;i++){                    
                vt[i].Item2=short.Parse(strings[i]);
            }
            resCharAttr=new DDO_CharacterAttribute(charId,level,experience,vt);
            return false;
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
        public bool UpdateCharacterPosition (DDO_CharacterPosition cp) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "update character_position set `x`=\"" + cp.m_position.X + "\",`y`=\"" + cp.m_position.Y + "\" where charid=" + cp.m_characterId + ";";
            string database = "legend";
            try{pool.ExecuteSql (database, cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool InsertCharacterPosition (DDO_CharacterPosition cp) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "insert into character_position values(" + cp.m_characterId + ",\"" + cp.m_position.X + "\",\"" + cp.m_position.Y + "\");";
            string database = "legend";
            try{pool.ExecuteSql (database, cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool GetCharacterPosition (int charId,out DDO_CharacterPosition res) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from character_position where charid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                res=default(DDO_CharacterPosition);
                return false;
            }
            float x = float.Parse (dt.Rows[0]["x"].ToString ());
            float y = float.Parse (dt.Rows[0]["y"].ToString ());
            res = new DDO_CharacterPosition (charId, new System.Numerics.Vector2 (x, y));
            return true;
        }
        public bool DeleteCharacterPositionByCharacterId(int charid){
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "delete from character_position where `charid`=" +charid+ ";";
            string database = "legend";
            try{pool.ExecuteSql (database, cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
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
        public bool UpdateUser (DDO_User ddo) {
            string cmd;
            cmd = "update user set `user_name`=\"" + ddo.m_username + "\",`password`=\"" + ddo.m_pwd + "\",`question`=\""+ddo.m_pwdProtectProblem+"\",`answer`=\""+ddo.m_pwdProtectAnswer+"\" where `userid`=" + ddo.m_playerId + ";";
            string database = "legend";
            try{pool.ExecuteSql (database, cmd);} catch (Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
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
        public bool InsertCharacterVipCard (DDO_CharacterVipCard vipCard) {
            string cmd;
            cmd = "insert into `vip` values(" + vipCard.m_characterId + "," + vipCard.m_vipLevel + ","+vipCard.m_chargeMoney+");";
            string database = "legend";
            try{pool.ExecuteSql (database, cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;

        }
        public bool GetCharacterVipCardByCharacterId (int playerId,out DDO_CharacterVipCard card) {
            string cmd;
            DataSet ds = new DataSet ();
            cmd = "select * from `vip` where `userid`="+playerId+";";
            string database = "legend";
            pool.ExecuteSql (database, cmd, ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0) {
                card=default(DDO_CharacterVipCard);
                return false;
            }
            card= new DDO_CharacterVipCard (int.Parse (dt.Rows[0]["userid"].ToString ()), int.Parse (dt.Rows[0]["vip_level"].ToString ()),long.Parse(dt.Rows[0]["charge_money"].ToString()));
            return true;
        }
        public bool UpdateCharacterVipCard (DDO_CharacterVipCard vipCard) {
            string cmd;
            cmd = "update `vip` set `vip_level`=" + vipCard.m_vipLevel + ",`charge_money`="+vipCard.m_chargeMoney+" where `userid`=" + vipCard.m_characterId + ";";
            string database = "legend";
            try{pool.ExecuteSql (database, cmd);}catch(Exception e){Console.WriteLine(e.StackTrace);return false;}
            return true;
        }
        public bool DeleteCharacterVipCardByCharacterId(int characterid){
            string cmd;
            cmd = "delete from `vip` where `charid`="+characterid+";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
            return true;
        }



        //COMBAT EFFECT
        public void InsertMixCombatEfct (DDO_CombatEfct mixCombatEfct) {
            string cmd = "insert into `combat_effect` values(" + mixCombatEfct.m_charId + "," + mixCombatEfct.m_combatEfct +",\""+mixCombatEfct.m_name+"\",\""+mixCombatEfct.m_ocp.ToString()+ "\","+mixCombatEfct.m_level+");";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public void UpdateMixCombatEfct (DDO_CombatEfct mixCombatEfct) {
            string cmd = "update `combat_effect` set `combat`=" + mixCombatEfct.m_combatEfct + ",`name`=\""+mixCombatEfct.m_name+"\", `occupation`=\""+mixCombatEfct.m_ocp.ToString()+"\",level="+mixCombatEfct.m_level+" where `charid`=" + mixCombatEfct.m_charId + ";";
            string database = "legend";
            pool.ExecuteSql (database, cmd);
        }
        public DDO_CombatEfct[] GetAllMixCombatEfct (){
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
                res[i].m_level=short.Parse(dt.Rows[i]["level"].ToString());
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