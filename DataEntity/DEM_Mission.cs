using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Mission {
        private Dictionary<short, DE_Mission> m_misDict = new Dictionary<short, DE_Mission> ();
        private Dictionary<short, DE_MissionTargetTalkToNpc> m_misTarTalkToNpcDict = new Dictionary<short, DE_MissionTargetTalkToNpc> ();
        private Dictionary<short, DE_MissionTargetKillMonster> m_misTarKillMonDict = new Dictionary<short, DE_MissionTargetKillMonster> ();
        private Dictionary<short, DE_MissionTargetGainItem> m_misTarGainItemDict = new Dictionary<short, DE_MissionTargetGainItem> ();
        private Dictionary<short, DE_MissionTargetLevelUpSkill> m_misTarLevelUpSkillDict = new Dictionary<short, DE_MissionTargetLevelUpSkill> ();
        public DEM_Mission (IDS_Mission ds) {
            var doArr = ds.GetAllMission ();
            foreach (var mDo in doArr)
                m_misDict.Add (mDo.m_id, new DE_Mission (mDo));
            var misDataDos = ds.GetAllMissionDatas ();
            foreach (var killMonDo in misDataDos.Item1)
                m_misTarKillMonDict.Add (killMonDo.m_id, new DE_MissionTargetKillMonster (killMonDo));
            foreach (var gainItemDo in misDataDos.Item2)
                m_misTarGainItemDict.Add (gainItemDo.m_id, new DE_MissionTargetGainItem (gainItemDo));
            foreach (var levelUpSkillDo in misDataDos.Item3)
                m_misTarLevelUpSkillDict.Add (levelUpSkillDo.m_id, new DE_MissionTargetLevelUpSkill (levelUpSkillDo));
            foreach (var talkToNpcDo in misDataDos.Item4)
                m_misTarTalkToNpcDict.Add (talkToNpcDo.m_id, new DE_MissionTargetTalkToNpc (talkToNpcDo));
        }
        public DE_Mission GetMissionById (short missionId) {
            DE_Mission res;
            m_misDict.TryGetValue (missionId, out res);
            return res;
        }
        public DE_MissionTargetTalkToNpc GetMissionTargetTalkToNpcById (short tarId) {
            DE_MissionTargetTalkToNpc res;
            m_misTarTalkToNpcDict.TryGetValue (tarId, out res);
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