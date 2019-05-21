using System.Collections.Generic;

namespace MirRemakeBackend {
    class SM_Skill {
        private IDDS_Skill m_skillDynamicDataService;
        private IDS_Skill m_skillDataService;
        private Dictionary<int, Dictionary<short, short>> m_playerNetworkIdAndSkillIdLevelDict = new Dictionary<int, Dictionary<short, short>> ();
        public SM_Skill () { }
        /// <summary>
        /// 根据角色networkId与技能id获取该角色的对应技能
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public E_Skill GetSkillByIdAndNetworkId (short skillId, int netId) {
            return EM_Skill.GetSkillByIdAndNetworkId (skillId, netId);
        }
        public E_Skill[] InitMonsterSkill (int netId, KeyValuePair<short, short>[] skillIdAndLevelArr) {
            E_Skill[] res = new E_Skill[skillIdAndLevelArr.Length];
            for (int i=0; i<skillIdAndLevelArr.Length; i++) {
                var skillDo = m_skillDataService.GetSkillByIdAndLevel (skillIdAndLevelArr[i].Key, skillIdAndLevelArr[i].Value);
                res[i] = new E_Skill(skillDo, netId);
            }
            EM_Skill.LoadUnitSkillArr (netId, res);
            return res;
        }
        /// <summary>
        /// 初始化角色的技能
        /// 依赖数据库读取
        /// 将角色所有技能存入EM中
        /// 返回角色的所有技能
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public List<E_Skill> InitCharacterSkill (int netId, int charId) {
            List<E_Skill> res = new List<E_Skill> ();
            var skillDdoList = m_skillDynamicDataService.GetSkillListByCharacterId (charId);
            for (int i=0; i<skillDdoList.Count; i++) {
                DO_Skill dataObj = m_skillDataService.GetSkillByIdAndLevel (skillDdoList[i].m_skillId, skillDdoList[i].m_skillLevel);
                res.Add (new E_Skill (dataObj, skillDdoList[i], netId));
            }
            EM_Skill.LoadUnitSkillList (netId, res);
            return res;
        }
    }
}