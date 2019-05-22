using System.Collections.Generic;
using System.Collections.ObjectModel;
using MirRemakeBackend.Entity;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 索引Character的所学技能  
    /// 索引Monster的技能  
    /// </summary>
    class EM_Skill {
        public static EM_Skill s_instance;
        private Dictionary<int, Dictionary<short, E_Skill>> m_networkIdAndCharacterSkillDict = new Dictionary<int, Dictionary<short, E_Skill>> ();
        private Dictionary<short, IReadOnlyList<E_Skill>> m_monsterIdAndSKillListDict = new Dictionary<short, IReadOnlyList<E_Skill>> ();
        public EM_Skill () {
            // 建立怪物技能索引
            var monsterEn = DEM_ActorUnit.s_instance.GetAllMonsterEn ();
            while (monsterEn.MoveNext ()) {
                short monsterId = monsterEn.Current.Key;
                var skillIdAndLvList = monsterEn.Current.Value.Item2.m_skillIdAndLevelList;
                E_Skill[] skillArr = new E_Skill[skillIdAndLvList.Count];
                for (int i = 0; i < skillIdAndLvList.Count; i++) {
                    DE_Skill skillDe;
                    DE_SkillData skillDataDe;
                    if (!DEM_Skill.s_instance.GetSkillByIdAndLevel (skillIdAndLvList[i].Key, skillIdAndLvList[i].Value, out skillDe, out skillDataDe))
                        continue;
                    E_Skill skillObj = EntityManagerPoolInstance.s_skillPool.GetInstance ();
                    skillObj.Reset (skillDe, skillDataDe);
                }
            }
        }
        public E_Skill[] InitCharacterSkill (int netId, int charId, List<DDO_Skill> ddoList) {
            E_Skill[] res = new E_Skill[ddoList.Count];
            for (int i = 0; i < ddoList.Count; i++) {
                DE_Skill de;
                DE_SkillData dataDe;
                if (!DEM_Skill.s_instance.GetSkillByIdAndLevel (ddoList[i].m_skillId, ddoList[i].m_skillLevel, out de, out dataDe))
                    continue;
                E_Skill skillObj = EntityManagerPoolInstance.s_skillPool.GetInstance ();
                skillObj.Reset (de, dataDe, ddoList[i]);
                res[i] = skillObj;
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