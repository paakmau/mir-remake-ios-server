using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引Character的所学技能  
    /// 索引Monster的技能  
    /// </summary>
    class EM_Skill : EntityManagerBase {
        public static EM_Skill s_instance;
        private DEM_Skill m_dem;
        private Dictionary<int, Dictionary<short, E_Skill>> m_networkIdAndCharacterSkillDict = new Dictionary<int, Dictionary<short, E_Skill>> ();
        public EM_Skill (DEM_Skill dem) {
            m_dem = dem;
        }
        public E_Skill[] InitCharacter (int netId, int charId, List<DDO_Skill> ddoList) {
            E_Skill[] res = new E_Skill[ddoList.Count];
            Dictionary<short, E_Skill> charSkillDict = new Dictionary<short, E_Skill> ();
            for (int i = 0; i < ddoList.Count; i++) {
                DE_Skill de;
                DE_SkillData dataDe;
                if (!m_dem.GetSkillByIdAndLevel (ddoList[i].m_skillId, ddoList[i].m_skillLevel, out de, out dataDe))
                    continue;
                E_Skill skillObj = s_entityPool.m_skillPool.GetInstance ();
                skillObj.Reset (de, dataDe, ddoList[i]);
                res[i] = skillObj;
            }
            m_networkIdAndCharacterSkillDict[netId] = charSkillDict;
            return res;
        }
        public void RemoveCharacter (int netId) {
            Dictionary<short, E_Skill> skills = null;
            m_networkIdAndCharacterSkillDict.TryGetValue (netId, out skills);
            if (skills == null) return;
            m_networkIdAndCharacterSkillDict.Remove (netId);
            var en = skills.GetEnumerator ();
            while (en.MoveNext ())
                s_entityPool.m_skillPool.RecycleInstance (en.Current.Value);
        }
        public E_Skill GetCharacterSkillByIdAndNetworkId (short skillId, int netId) {
            Dictionary<short, E_Skill> learnedSkill = null;
            if (!m_networkIdAndCharacterSkillDict.TryGetValue (netId, out learnedSkill))
                return null;
            E_Skill res = null;
            learnedSkill.TryGetValue (skillId, out res);
            return res;
        }
    }
}