using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引Character的所学技能  
    /// </summary>
    class EM_Skill : EntityManagerBase {
        public static EM_Skill s_instance;
        private DEM_Skill m_dem;
        private Dictionary<int, Dictionary<short, E_Skill>> m_characterSkillDict = new Dictionary<int, Dictionary<short, E_Skill>> ();
        public EM_Skill (DEM_Skill dem) {
            m_dem = dem;
        }
        public IReadOnlyList<short> GetAllSkillIdListByOccupation (OccupationType ocp) {
            return m_dem.GetSkillIdListByOccupation (ocp);
        }
        public E_Skill[] InitCharacter (int netId, int charId, List<DDO_Skill> ddoList) {
            E_Skill[] res;
            Dictionary<short, E_Skill> charSkillDict;
            // 若角色已经加载
            if (m_characterSkillDict.TryGetValue (netId, out charSkillDict)) {
                res = new E_Skill[charSkillDict.Count];
                var en = charSkillDict.Values.GetEnumerator ();
                var i = 0;
                while (en.MoveNext ()) {
                    res[i] = en.Current;
                    i++;
                }
                return res;
            }
            res = new E_Skill[ddoList.Count];
            charSkillDict = new Dictionary<short, E_Skill> ();
            for (int i = 0; i < ddoList.Count; i++) {
                DE_Skill de;
                DE_SkillData dataDe;
                if (!m_dem.GetSkillByIdAndLevel (ddoList[i].m_skillId, ddoList[i].m_skillLevel, out de, out dataDe))
                    continue;
                E_Skill skillObj = s_entityPool.m_skillPool.GetInstance ();
                skillObj.Reset (de, dataDe, ddoList[i]);
                res[i] = skillObj;
                charSkillDict.Add (skillObj.m_SkillId, skillObj);
            }
            m_characterSkillDict[netId] = charSkillDict;
            return res;
        }
        public void RemoveCharacter (int netId) {
            Dictionary<short, E_Skill> skills = null;
            m_characterSkillDict.TryGetValue (netId, out skills);
            if (skills == null) return;
            m_characterSkillDict.Remove (netId);
            var en = skills.GetEnumerator ();
            while (en.MoveNext ())
                s_entityPool.m_skillPool.RecycleInstance (en.Current.Value);
        }
        public E_Skill GetCharacterSkillByIdAndNetworkId (short skillId, int netId) {
            Dictionary<short, E_Skill> learnedSkill = null;
            if (!m_characterSkillDict.TryGetValue (netId, out learnedSkill))
                return null;
            E_Skill res = null;
            learnedSkill.TryGetValue (skillId, out res);
            return res;
        }
    }
}