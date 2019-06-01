using System.Collections.Generic;
using System.Data;
using System;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Skill {
        List<DDO_Skill> GetSkillListByCharacterId (int charId);
        void UpdateSkill (DDO_Skill ddo);
    }
    /*class IDDS_SkillImpl:IDDS_Skill {
        private SqlConfig sqlConfig;
        public List<DDO_Skill> GetSkillListByCharacterId(int charId) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from skill where userid=" + charId + ";";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
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
        public void UpdateSkill(DDO_Skill ddo, int charId) {
            string cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRowCollection dataRowCollection;
            cmd = "select * from skill where userid=" + charId + " and skillid="+ddo.m_skillId+";";
            string database = "legend";
            SQLPool pool = new SQLPool(sqlConfig);
            pool.ExecuteSql(database, cmd, ds);
            dt = ds.Tables["skill"];
            dataRowCollection = dt.Rows;
            if (dataRowCollection.Count != 0) { 
                cmd="update skill set masterly="+ddo.m_masterly+", level="+ddo.m_skillLevel+ " where userid=" + charId + " and skillid=" + ddo.m_skillId + ";"
            }
            else{
                cmd = "insert into skill values(null," + ddo.m_skillId + "," + charId + "," + ddo.m_masterly + "," + ddo.m_skillLevel;
            }
            pool.ExecuteSql(database, cmd);
        }

    }*/
}