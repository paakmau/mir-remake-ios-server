using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 索引Monster的技能  
    /// </summary>
    class EM_MonsterSkill : EntityManagerBase {
        public static EM_MonsterSkill s_instance;
        // 每只怪物的技能
        private Dictionary<int, E_MonsterSkill[]> m_skillDict = new Dictionary<int, E_MonsterSkill[]> ();
        public EM_MonsterSkill (DEM_Skill dem, DEM_Unit unitDem) {
            // 实例化所有怪物技能
            var monEn = unitDem.GetAllMonsterEn ();
            while (monEn.MoveNext ()) {
                short monId = monEn.Current.Key;
                var skillIdAndLvList = monEn.Current.Value.Item2.m_skillIdAndLevelList;
                E_MonsterSkill[] monSkillArr = new E_MonsterSkill[skillIdAndLvList.Count];
                for (int i = 0; i < skillIdAndLvList.Count; i++) {
                    DE_Skill skillDe;
                    DE_SkillData skillDataDe;
                    if (!dem.GetSkillByIdAndLevel (skillIdAndLvList[i].Item1, skillIdAndLvList[i].Item2, out skillDe, out skillDataDe))
                        continue;
                    monSkillArr[i] = new E_MonsterSkill (skillIdAndLvList[i].Item1, skillIdAndLvList[i].Item2, skillDe, skillDataDe);
                }
                m_skillDict[monId] = monSkillArr;
            }
        }
        public bool GetRandomValidSkill (int netId, short monId, out E_MonsterSkill resSkill) {
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
        public E_MonsterSkill[] GetRawSkillArr (int netId) {
            E_MonsterSkill[] res = null;
            m_skillDict.TryGetValue (netId, out res);
            return res;
        }
    }
}