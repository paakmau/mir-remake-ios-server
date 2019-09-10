using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Mission {
        private Dictionary<short, DE_Mission> m_misDict = new Dictionary<short, DE_Mission> ();
        private Dictionary<short, DE_Title> m_titleDict = new Dictionary<short, DE_Title> ();
        private Dictionary<short, DE_MissionTargetKillMonster> m_misTarKillMonDict = new Dictionary<short, DE_MissionTargetKillMonster> ();
        private Dictionary<short, DE_MissionTargetGainItem> m_misTarGainItemDict = new Dictionary<short, DE_MissionTargetGainItem> ();
        private Dictionary<short, DE_MissionTargetLevelUpSkill> m_misTarLevelUpSkillDict = new Dictionary<short, DE_MissionTargetLevelUpSkill> ();

        public DEM_Mission (IDS_Mission ds) {
            var doArr = ds.GetAllMission ();
            foreach (var mDo in doArr)
                m_misDict.Add (mDo.m_id, new DE_Mission (mDo));

            var titleDoArr = ds.GetAllTitleMission ();
            foreach (var tmDo in titleDoArr)
                m_misDict.Add (tmDo.m_id, new DE_Mission (tmDo));

            var titleAttr = ds.GetAllTitleIDAndAttributes ();
            foreach (var ta in titleAttr)
                m_titleDict.Add (ta.Item1, new DE_Title(ta.Item2));

            var misTarKillMon = ds.GetAllMissionTargetKillMonster ();
            foreach (var killMonDo in misTarKillMon)
                m_misTarKillMonDict.Add (killMonDo.m_id, new DE_MissionTargetKillMonster (killMonDo));

            var misTarGainItem = ds.GetAllMissionTargetGainItem ();
            foreach (var gainItemDo in misTarGainItem)
                m_misTarGainItemDict.Add (gainItemDo.m_id, new DE_MissionTargetGainItem (gainItemDo));

            var misTarLvUpSkill = ds.GetAllMissionTargetLevelUpSkill ();
            foreach (var levelUpSkillDo in misTarLvUpSkill)
                m_misTarLevelUpSkillDict.Add (levelUpSkillDo.m_id, new DE_MissionTargetLevelUpSkill (levelUpSkillDo));
        }
        public DE_Mission GetMissionById (short missionId) {
            DE_Mission res;
            m_misDict.TryGetValue (missionId, out res);
            return res;
        }
        public DE_Title GetTitleById (short misId) {
            DE_Title res;
            m_titleDict.TryGetValue (misId, out res);
            return res;
        }
        public DE_MissionTargetKillMonster GetMissionTargetKillMonster (short tarId) {
            DE_MissionTargetKillMonster res;
            m_misTarKillMonDict.TryGetValue (tarId, out res);
            return res;
        }
        public DE_MissionTargetGainItem GetMissionTargetGainItem (short tarId) {
            DE_MissionTargetGainItem res;
            m_misTarGainItemDict.TryGetValue (tarId, out res);
            return res;
        }
        public DE_MissionTargetLevelUpSkill GetMissionTargetLevelUpSkill (short tarId) {
            DE_MissionTargetLevelUpSkill res;
            m_misTarLevelUpSkillDict.TryGetValue (tarId, out res);
            return res;
        }
    }
}