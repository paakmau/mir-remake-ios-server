using System.Collections.Generic;
using MySql.Data;
using System.Data;
using LitJson;
using System;
namespace MirRemakeBackend.DynamicData {
    interface IDDS_Item {
        List<DDO_Item> GetBagByCharacterId (int charId);//done
        List<DDO_Item> GetStoreHouseByCharacterId (int charId);
        /// <summary>
        /// 获取角色身上的 (装备区内) 装备
        /// </summary>
        List<DDO_Item> GetEquipmentRegionByCharacterId (int charId);
        /// <summary>
        /// 获取属于该角色的所有装备 (包括背包仓库和装备区)
        /// </summary>
        List<DDO_Equipment> GetAllEquipmentByCharacterId (int charId);
        /// <summary>
        /// 直接更新一件物品
        /// </summary>
        void UpdateItem (DDO_Item item);
    }
    /*class IDDS_ItemImpl:IDDS_Item {

        private SqlConfig sqlConfig;
        public List<DDO_Item> GetBagByCharacterId(int charId) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from item where userid=" + charId+" and pos=1;";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["item"];
            dataRowCollection = dt.Rows;
            List<DDO_Item> res = new List<DDO_Item>();
            for(int i = 0; i < dataRowCollection.Count; i++) {
                DDO_Item item = new DDO_Item();
                item.m_num = short.Parse(dataRowCollection[i]["num"].ToString());
                item.m_itemId = short.Parse(dataRowCollection[i]["itemid"].ToString());
                item.m_realId = short.Parse(dataRowCollection[i]["realid"].ToString());
                res.Add(item);
            }
            return res;
        }
        public List<DDO_Item> GetStoreHouseByCharacterId(int charId) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from item where userid=" + charId + " and pos=0;";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["item"];
            dataRowCollection = dt.Rows;
            List<DDO_Item> res = new List<DDO_Item>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DDO_Item item = new DDO_Item();
                item.m_num = short.Parse(dataRowCollection[i]["num"].ToString());
                item.m_itemId = short.Parse(dataRowCollection[i]["itemid"].ToString());
                item.m_realId = short.Parse(dataRowCollection[i]["realid"].ToString());
                res.Add(item);
            }
            return res;
        }
        public List<DDO_Equipment> GetEquipmentRegionByCharacterId(int charId) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from equipment where userid=" + charId + " and pos=0;";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["equipment"];
            dataRowCollection = dt.Rows;
            List<DDO_Equipment> res = new List<DDO_Equipment>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DDO_Equipment equipment = new DDO_Equipment();
                equipment.m_strengthNum = byte.Parse(dataRowCollection[i]["strengthnum"].ToString());
                equipment.m_holeNum = short.Parse(dataRowCollection[i]["holenum"].ToString());
                equipment.m_realId = short.Parse(dataRowCollection[i]["realid"].ToString());
                string gems = dataRowCollection[i]["gemlist"].ToString();
                equipment.m_inlaidGemIdList = new List<short>();
                if (gems.Length != 0) {
                    for (int j = 0; j < gems.Split(' ').Length; j++) {
                        equipment.m_inlaidGemIdList.Add(short.Parse(gems.Split(' ')[j]));
                    }
                }
                JsonData attr = JsonMapper.ToObject(dataRowCollection[i]["enchantattr"].ToString());
                equipment.m_enchantAttr = getAttr(attr);
                res.Add(equipment); 
            }
            return res;
        }
        public List<DDO_Equipment> GetAllEquipmentByCharacterId(int charId) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from equipment where userid=" + charId + ";";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["equipment"];
            dataRowCollection = dt.Rows;
            List<DDO_Equipment> res = new List<DDO_Equipment>();
            for (int i = 0; i < dataRowCollection.Count; i++)
            {
                DDO_Equipment equipment = new DDO_Equipment();
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
                equipment.m_enchantAttr = getAttr(attr);
                res.Add(equipment);
            }
            return res;
        }
        private ValueTuple<ActorUnitConcreteAttributeType,int>[] getAttr(JsonData attr) {
            ValueTuple<ActorUnitConcreteAttributeType, int>[] res = new ValueTuple<ActorUnitConcreteAttributeType, int>[attr.Count];
            for (int j = 0; j < attr.Count; j++)
            {
                res[j] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                    ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType),attr[j].ToString().Split(' ')[0]),
                    int.Parse(attr[j].ToString().Split(' ')[1]));
            }
            return res;
        }
    }*/
}