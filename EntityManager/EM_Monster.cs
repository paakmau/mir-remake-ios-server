using System.Collections.Generic;


namespace MirRemakeBackend {
    class EM_Monster {
        public static EM_Monster s_instance;
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        public EM_Monster () {
            var idAndPosList = DEM_Map.s_instance.GetAllMonsterIdAndRespawnPosition ();
            for (int i=0; i<idAndPosList.Count; i++) {
                DE_Monster monsterDe = DEM_Monster.s_instance.GetMonsterById (idAndPosList[i].Key);
                DE_Skill[] skillDeArr = new DE_Skill[monsterDe.m_skillIdAndLevelList.Count];
                for (int j=0; j<monsterDe.m_skillIdAndLevelList.Count; j++)
                    skillDeArr[j] = DEM_Skill.s_instance.GetSkillByIdAndLevel (
                        monsterDe.m_skillIdAndLevelList[j].Key,
                        monsterDe.m_skillIdAndLevelList[j].Value);
                E_Monster monster = new E_Monster (NetworkIdManager.s_instance.GetNewActorUnitNetworkId (), idAndPosList[i].Value, monsterDe, skillDeArr);
            }
        }
        public E_Monster GetMonsterByNetworkId (int netId) {
            E_Monster res = null;
            m_networkIdAndMonsterDict.TryGetValue (netId, out res);
            return res;
        }
    }
}