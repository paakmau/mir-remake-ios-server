using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MirRemakeBackend {
    /// <summary>
    /// 索引Character的所学技能  
    /// 索引Monster的技能  
    /// </summary>
    class EM_Skill {
        public static EM_Skill s_instance;
        private IDDS_Skill m_skillDds;
        private Dictionary<int, Dictionary<short, E_Skill>> m_networkIdAndCharacterSkillDict = new Dictionary<int, Dictionary<short, E_Skill>> ();
        private Dictionary<short, IReadOnlyList<E_Skill>> m_monsterIdAndSKillListDict = new Dictionary<short, IReadOnlyList<E_Skill>> ();
        public EM_Skill (IDDS_Skill skillDds) {
            m_skillDds = skillDds;
            // 建立怪物技能索引
            var monsterEn = DEM_Monster.s_instance.GetAllMonsterEn ();
            while (monsterEn.MoveNext ()) {
                short monsterId = monsterEn.Current.Key;
                var skillIdAndLvList = monsterEn.Current.Value.m_skillIdAndLevelList;
                E_Skill[] skillArr = new E_Skill[skillIdAndLvList.Count];
                for (int i=0; i<skillIdAndLvList.Count; i++) {
                    DE_Skill skillDe;
                    DE_SkillData skillDataDe;
                    if (!DEM_Skill.s_instance.GetSkillByIdAndLevel (skillIdAndLvList[i].Key, skillIdAndLvList[i].Value, out skillDe, out skillDataDe))
                        continue;
                    E_Skill skillObj = EntityManagerPoolInstance.s_skillPool.GetInstance ();
                    skillObj.Reset (skillDe, skillDataDe);
                }
            }
        }
        public E_Skill[] InitCharacterSkill (int netId, int charId) {
            List<DDO_Skill> skillDdoList = m_skillDds.GetSkillListByCharacterId (charId);
            E_Skill[] res = new E_Skill[skillDdoList.Count];
            for (int i=0; i<skillDdoList.Count; i++) {
            }
            return res;
        }
        public E_Skill GetCharacterSkillByIdAndNetworkId (short skillId, int netId) {
            Dictionary<short, E_Skill> learnedSkill = null;
            if (!m_networkIdAndCharacterSkillDict.TryGetValue (netId, out learnedSkill))
                return null;
            E_Skill res = null;
            learnedSkill.TryGetValue (skillId, out res);
            return res;
        }
        public IReadOnlyList<E_Skill> GetMonsterSkillListByMonsterId (short monsterId) {
            IReadOnlyList<E_Skill> res = null;
            m_monsterIdAndSKillListDict.TryGetValue (monsterId, out res);
            return res;
        }
    }
}