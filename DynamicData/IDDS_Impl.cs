using System.Data;
using System;
using System.Net;
using LitJson;
using System.Collections.Generic;
namespace MirRemakeBackend.DynamicData { 
    class IDDS_Impl : IDDS_Item, IDDS_Skill, IDDS_Mission, IDDS_Character {
        private SqlConfig sqlConfig;
        private SQLPool pool;
        public IDDS_Impl() {
            sqlConfig = new SqlConfig();
            sqlConfig.username="root";
            sqlConfig.pwd="root";
            sqlConfig.database="legend";
            sqlConfig.server="localhost";
            pool = new SQLPool(sqlConfig);
        }
        public List<DDO_Item> GetBagByCharacterId(int charId)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from `item` where charid=" + charId + " and place=\"bag\";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables[0];
            dataRowCollection = dt.Rows;
            List<DDO_Item> res = new List<DDO_Item>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DataRow dr = dataRowCollection[i];
                short realid = short.Parse(dr["realid"].ToString());
                short itemid = short.Parse(dr["itemid"].ToString());
                short num = short.Parse(dr["num"].ToString());
                ItemPlace place = ItemPlace.BAG;
                int pos = int.Parse(dr["position"].ToString());
                res.Add(new DDO_Item(realid,itemid,charId,num,place,pos));
            }
            return res;
        }
        public List<DDO_Item> GetStoreHouseByCharacterId(int charId)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from `item` where charid=" + charId + " and place=\"STORE_HOUSE\";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables[0];
            dataRowCollection = dt.Rows;
            List<DDO_Item> res = new List<DDO_Item>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DataRow dr = dataRowCollection[i];
                short realid = short.Parse(dr["realid"].ToString());
                short itemid = short.Parse(dr["itemid"].ToString());
                short num = short.Parse(dr["num"].ToString());
                ItemPlace place = ItemPlace.STORE_HOUSE;
                int pos = int.Parse(dr["position"].ToString());
                res.Add(new DDO_Item(realid, itemid, charId, num, place, pos));
            }
            return res;
        }
        public List<DDO_Item> GetEquipmentRegionByCharacterId(int charId)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from `item` where charid=" + charId + " and pos=\"BAG\";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables[0];
            dataRowCollection = dt.Rows;
            List<DDO_Item> res = new List<DDO_Item>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DataRow dr = dataRowCollection[i];
                short realid = short.Parse(dr["realid"].ToString());
                short itemid = short.Parse(dr["itemid"].ToString());
                short num = short.Parse(dr["num"].ToString());
                ItemPlace place = ItemPlace.STORE_HOUSE;
                int pos = int.Parse(dr["position"].ToString());
                res.Add(new DDO_Item(realid, itemid, charId, num, place, pos));
            }
            return res;
        }
        public List<DDO_EquipmentInfo> GetAllEquipmentByCharacterId(int charId)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt;
            DataRowCollection dataRowCollection;
            cmd = "select * from `equipment` where charid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["equipment"];
            dataRowCollection = dt.Rows;
            List<DDO_EquipmentInfo> res = new List<DDO_EquipmentInfo>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DDO_EquipmentInfo equipment = new DDO_EquipmentInfo();
                equipment.m_strengthNum = byte.Parse(dataRowCollection[i]["strengthnum"].ToString());
                equipment.m_holeNum = short.Parse(dataRowCollection[i]["holenum"].ToString());
                equipment.m_realId = short.Parse(dataRowCollection[i]["realid"].ToString());
                string gems = dataRowCollection[i]["gemlist"].ToString();
                equipment.m_inlaidGemIdList = new List<short>();
                if (gems.Length != 0)
                {
                    for (int j = 0; j < gems.Split(' ').Length; j++)
                    {
                        equipment.m_inlaidGemIdList.Add(short.Parse(gems.Split(' ')[j]));
                    }
                }
                JsonData attr = JsonMapper.ToObject(dataRowCollection[i]["enchantattr"].ToString());
                equipment.m_enchantAttr = GetAttr(attr);
                res.Add(equipment);
            }
            return res;
        }
        public void UpdateItem(DDO_Item item) {
            string cmd;
            cmd = "update `item` set num="+item.m_num+",place="+item.m_place.ToString()+",position="+item.m_position+" where itemid="+item.m_itemId+" and realid="+item.m_realId+";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);
        }
        public void DeleteItemByRealId(long realId) {
            string cmd;
            cmd = "delete from `item` where realid=" + realId + ";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);
        }
        public long InsertItem(DDO_Item item) {
            string cmd;
            DataSet ds = new DataSet();
            cmd = "insert into `item` values(null,"+item.m_itemId+","+item.m_characterId+","+item.m_num+",\""+item.m_place.ToString()+"\","+item.m_position+");select last_insert_id()";
            string database = "legend";
            pool.ExecuteSql(database, cmd,ds);
            return int.Parse(ds.Tables["item"].Rows[0]["realid"].ToString());
        }
        public void InsertEquipmentInfo(DDO_EquipmentInfo eq) {
            string cmd;
            string gems;
            if (eq.m_inlaidGemIdList.Count != 0)
            {
                gems = eq.m_inlaidGemIdList[0].ToString();
                for(int i = 0; i < eq.m_inlaidGemIdList.Count; i++) {
                    gems = gems + " " + eq.m_inlaidGemIdList[i].ToString();
                }
            }
            else
            {
                gems = "";
            }
            string enchantAttr = GetString(eq.m_enchantAttr);
            cmd = "insert into `equipment` valus(null,"+eq.m_characterId+","+eq.m_strengthNum+","+gems+","+enchantAttr+","+eq.m_holeNum+";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);
        }


        public List<DDO_Skill> GetSkillListByCharacterId(int charId)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from skill where userid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["skill"];
            dataRowCollection = dt.Rows;
            List<DDO_Skill> res = new List<DDO_Skill>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DataRow dr = dataRowCollection[i];
                DDO_Skill skill = new DDO_Skill();
                skill.m_skillId = short.Parse(dr["skillid"].ToString());
                skill.m_masterly = int.Parse(dr["masterly"].ToString());
                skill.m_skillLevel = short.Parse(dr["level"].ToString());
                res.Add(skill);
            }
            return res;
        }
        public void UpdateSkill(DDO_Skill ddo)
        {
            int charId = ddo.m_characterId;
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from skill where userid=" + charId + " and skillid=" + ddo.m_skillId + ";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["skill"];
            dataRowCollection = dt.Rows;
            if (dataRowCollection.Count != 0)
            {
                cmd = "update skill set masterly=" + ddo.m_masterly + ", level=" + ddo.m_skillLevel + " where userid=" + charId + " and skillid=" + ddo.m_skillId + ";";
            }
            else
            {
                cmd = "insert into skill values(null," + ddo.m_skillId + "," + charId + "," + ddo.m_masterly + "," + ddo.m_skillLevel;
            }
            pool.ExecuteSql(database, cmd);
        }


        public int CreateCharacter(OccupationType occupation)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "insert into character values (null," + occupation.ToString() + ",1,0,\"0 0\",\"0 0 0 0\";select max(characterid)";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["character"];
            dataRowCollection = dt.Rows;
            return int.Parse(dataRowCollection[0]["characterid"].ToString());
        }
        public DDO_Character GetCharacterById(int characterId)
        {
            DDO_Character character = new DDO_Character();
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from `character` where characterid=" + characterId + ";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables[0];
            dataRowCollection = dt.Rows;
            DataRow dr = dataRowCollection[0];
            character.m_currencyArr = new ValueTuple<CurrencyType, long>[2];
            character.m_currencyArr[0] = new ValueTuple<CurrencyType, long>(CurrencyType.CHARGE,long.Parse(dr["currency"].ToString().Split(' ')[0]));
            character.m_currencyArr[1] = new ValueTuple<CurrencyType, long>(CurrencyType.VIRTUAL, long.Parse(dr["currency"].ToString().Split(' ')[1]));
            character.m_distributedMainAttrPointArr = new ValueTuple<ActorUnitMainAttributeType, short>[4];
            character.m_distributedMainAttrPointArr[0] = new ValueTuple<ActorUnitMainAttributeType,short>(ActorUnitMainAttributeType.STRENGTH,short.Parse(dr["giftpoints"].ToString().Split(' ')[0]));
            character.m_distributedMainAttrPointArr[1] = new ValueTuple<ActorUnitMainAttributeType, short>(ActorUnitMainAttributeType.AGILITY, short.Parse(dr["giftpoints"].ToString().Split(' ')[1]));
            character.m_distributedMainAttrPointArr[2] = new ValueTuple<ActorUnitMainAttributeType, short>(ActorUnitMainAttributeType.INTELLIGENCE, short.Parse(dr["giftpoints"].ToString().Split(' ')[2]));
            character.m_distributedMainAttrPointArr[3] = new ValueTuple<ActorUnitMainAttributeType, short>(ActorUnitMainAttributeType.SPIRIT, short.Parse(dr["giftpoints"].ToString().Split(' ')[3]));
            character.m_level = short.Parse(dr["level"].ToString());
            character.m_occupation = (OccupationType)Enum.Parse(typeof(OccupationType), dr["occupation"].ToString());
            character.m_experience = int.Parse(dr["experience"].ToString());
            character.m_characterId = int.Parse(dr["characterid"].ToString());
            return character;
        }
        public void UpdateCharacter(DDO_Character charObj) {
            string cmd;
            string currencyArr = "\"" + charObj.m_currencyArr[0].Item2.ToString()+","+charObj.m_currencyArr[1].Item2.ToString()+"\"";
            string giftPoints = "\"" + charObj.m_distributedMainAttrPointArr[0].Item2.ToString() + "," +
                charObj.m_distributedMainAttrPointArr[1].Item2.ToString() + "," +
                charObj.m_distributedMainAttrPointArr[2].Item2.ToString() + "," +
                charObj.m_distributedMainAttrPointArr[3].Item2.ToString() + "\"" ;
            cmd = "update `character` set characterid=" + charObj.m_characterId + ",occupation=\"" +charObj.m_occupation.ToString()+"\",level=" + charObj.m_level+",expericence="
                +charObj.m_experience+",currency="+currencyArr+",giftpoints="+giftPoints+" where characterid="+charObj.m_characterId+";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);
        }


        public List<DDO_Mission> GetMissionListByCharacterId(int charId)
        {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from mission where userid=" + charId + ";";
            string database = "legend";
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["mission"];
            dataRowCollection = dt.Rows;
            List<DDO_Mission> missions = new List<DDO_Mission>();
            for(int i = 0; i < dataRowCollection.Count; i++) {
                DDO_Mission mission = new DDO_Mission();
                mission.m_missionId = short.Parse(dataRowCollection[i]["missionid"].ToString());
                mission.m_missionTargetProgressList = new List<int>();
                string[] targets = dataRowCollection[i]["targets"].ToString().Split(' ');
                for(int j = 0; j < targets.Length; j++) {
                    mission.m_missionTargetProgressList.Add(int.Parse(targets[j]));
                }
            }
            return missions;
        }
        public void InsertMission(DDO_Mission ddo, int charId)
        {
            string cmd;
            string target = ddo.m_missionTargetProgressList[0].ToString();
            for (int i = 1; i < ddo.m_missionTargetProgressList.Count; i++)
            {
                target = target + " " + ddo.m_missionTargetProgressList[i].ToString();
            }
            cmd = "insert into mission values(null,"+ddo.m_missionId+",\""+target+"\","+charId+";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);
        }
        public void UpdateMission(DDO_Mission ddo, int charId) {
            string cmd;
            string target=ddo.m_missionTargetProgressList[0].ToString();
            for(int i = 1; i < ddo.m_missionTargetProgressList.Count; i++) {
                target = target + " " + ddo.m_missionTargetProgressList[i].ToString();
            }
            cmd = "update mission set targets=\""+target+"\" where userid="+charId+" and missionid="+ddo.m_missionId+";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);
        }
        public void DeleteMission(short missionId, int charId) {
            string cmd;
            cmd = "delete from item where userid=" + charId + " and missionid="+missionId+";";
            string database = "legend";
            pool.ExecuteSql(database, cmd);

        }


        private ValueTuple<ActorUnitConcreteAttributeType, int>[] GetAttr(JsonData attr)
        {
            ValueTuple<ActorUnitConcreteAttributeType, int>[] res = new ValueTuple<ActorUnitConcreteAttributeType, int>[attr.Count];
            for (int j = 0; j < attr.Count; j++)
            {
                res[j] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                    ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType), attr[j].ToString().Split(' ')[0]),
                    int.Parse(attr[j].ToString().Split(' ')[1]));
            }
            return res;
        }
        private string GetString(ValueTuple<ActorUnitConcreteAttributeType, int>[] ps) {
            if (ps.Length == 0) {
                return new String(""); 
            }
            String res = new String("");
            res ="[\""+ps[0].Item1.ToString()+","+ps[0].Item2.ToString()+"\"";
            for(int i = 1; i < ps.Length; i++) {
                res = res + ",\"" + ps[0].Item1.ToString() + "," + ps[0].Item2.ToString() + "\"";
            }

            res = res + "]";
            return res;
        }
    }
}