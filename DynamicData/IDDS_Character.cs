using System.Collections.Generic;
using System;
using System.Data;
namespace MirRemakeBackend.DynamicData {
    interface IDDS_Character {
        /// <summary>
        /// 创建一个角色
        /// </summary>
        /// <returns>角色的id</returns>
        int CreateCharacter (OccupationType occupation);
        DDO_Character GetCharacterById (int characterId);
        
    }
    //SELECT LAST_INSERT_ID()

    /*class IDDS_CharacterImpl : IDDS_Character { 
        /*public int CreateCharacter(OccupationType occupation) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "insert into character values (null,"+occupation.ToString()+",1,0,0";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
            pool.ExecuteSql(database, cmd);
            cmd = "select last_insert_id()";
            pool.ExecuteSql(database, cmd,ds);
            dt = ds.Tables["character"];
            dataRowCollection = dt.Rows;
            return int.Parse(dataRowCollection[0]["characterid"].ToString());
        }
        public DDO_Character GetCharacterById(int characterId) {
            return null;
        }
    }*/
}