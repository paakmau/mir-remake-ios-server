using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引Monster的技能  
    /// </summary>
    class EM_MonsterSkill {
        public static EM_MonsterSkill s_instance;
        // 每只怪物的技能
        private Dictionary<int, E_MonsterSkill[]> m_skillDict = new Dictionary<int, E_MonsterSkill[]> ();
        public void SetMonsterSkill (short monId, E_MonsterSkill[] skillArr) {
            m_skillDict[monId] = skillArr;
        }
        public bool GetRandomValidSkill (short monId, out E_MonsterSkill resSkill) {
            E_MonsterSkill[] skillArr = null;
            m_skillDict.TryGetValue (monId, out skillArr);
            foreach (var skill in skillArr)
                if (!skill.m_IsCoolingDown) {
                    resSkill = skill;
                    return true;
                }
            resSkill = null;
            return false;
        }
    }
}