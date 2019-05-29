using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 索引Character的所学技能  
    /// 索引Monster的技能  
    /// </summary>
    class EM_Skill : EntityManagerBase {
        public static EM_Skill s_instance;
        private DEM_Skill m_dem;
        private Dictionary<int, Dictionary<short, E_Skill>> m_networkIdAndCharacterSkillDict = new Dictionary<int, Dictionary<short, E_Skill>> ();
        private Dictionary<short, ValueTuple<DE_Skill, DE_SkillData>[]> m_monsterSkillDict = new Dictionary < short, (DE_Skill, DE_SkillData) [] > ();
        // 怪物技能冷却
        private Dictionary<int, List<ValueTuple<short, MyTimer.Time>>> m_monsterSkillCoolDownDict = new Dictionary<int, List<(short, MyTimer.Time)>> ();
        public EM_Skill (DEM_Skill dem) {
            m_dem = dem;
        }
        public void InitMonsterSkill (short monId, IReadOnlyList<ValueTuple<short, short>> skillIdAndLvList) {
            ValueTuple<DE_Skill, DE_SkillData>[] monSkillArr = new ValueTuple<DE_Skill, DE_SkillData>[skillIdAndLvList.Count];
            for (int i = 0; i < skillIdAndLvList.Count; i++) {
                DE_Skill skillDe;
                DE_SkillData skillDataDe;
                if (!m_dem.GetSkillByIdAndLevel (skillIdAndLvList[i].Item1, skillIdAndLvList[i].Item2, out skillDe, out skillDataDe))
                    continue;
                monSkillArr[i] = new ValueTuple<DE_Skill, DE_SkillData> (skillDe, skillDataDe);
            }
            m_monsterSkillDict[monId] = monSkillArr;
        }
        public void InitMonsterSkillCoolDown (int netId) {
            m_monsterSkillCoolDownDict[netId] = new List<(short, MyTimer.Time)> ();
        }
        public E_Skill[] InitCharacterSkill (int netId, int charId, List<DDO_Skill> ddoList) {
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
        public void RemoveCharacterSkill (int netId) {
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
        private bool IsSkillValid (int netId, short skillId) {
            List<ValueTuple<short, MyTimer.Time>> coolDownList = null;
            m_monsterSkillCoolDownDict.TryGetValue (netId, out coolDownList);
            if (coolDownList == null) return false;
            for (int i = 0; i < coolDownList.Count; i++)
                if (coolDownList[i].Item1 == skillId)
                    return false;
            return true;
        }
        public bool GetMonsterRandomValidSkill (int netId, short monId, out ValueTuple<DE_Skill, DE_SkillData> resSkill) {
            ValueTuple<DE_Skill, DE_SkillData>[] skillArr = null;
            m_monsterSkillDict.TryGetValue (monId, out skillArr);
            foreach (var skill in skillArr)
                if (IsSkillValid (netId, skill.Item1.m_skillId)) {
                    resSkill = skill;
                    return true;
                }
            resSkill = default;
            return false;
        }
        public void SetMonsterSkillCoolDown (int netId, ValueTuple<DE_Skill, DE_SkillData> skill) {
            List<ValueTuple<short, MyTimer.Time>> coolDownList = null;
            m_monsterSkillCoolDownDict.TryGetValue (netId, out coolDownList);
            coolDownList.Add (
                new ValueTuple<short, MyTimer.Time> (
                    skill.Item1.m_skillId, MyTimer.s_CurTime.Ticked (skill.Item2.m_coolDownTime)));
        }
        public List<ValueTuple<short, MyTimer.Time>> GetMonsterRawSkillCoolDownList (int netId) {
            List<ValueTuple<short, MyTimer.Time>> res = null;
            m_monsterSkillCoolDownDict.TryGetValue (netId, out res);
            return res;
        }
    }
}