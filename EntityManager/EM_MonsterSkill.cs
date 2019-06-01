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
    class EM_MonsterSkill : EntityManagerBase {
        public static EM_MonsterSkill s_instance;
        private Dictionary<short, ValueTuple<DE_Skill, DE_SkillData>[]> m_monsterSkillDict = new Dictionary < short, (DE_Skill, DE_SkillData) [] > ();
        // 怪物技能冷却
        private Dictionary<int, List<ValueTuple<short, MyTimer.Time>>> m_monsterSkillCoolDownDict = new Dictionary<int, List<(short, MyTimer.Time)>> ();
        public EM_MonsterSkill (DEM_Skill dem, DEM_ActorUnit unitDem) {
            // 实例化所有怪物技能
            var monEn = unitDem.GetAllMonsterEn ();
            while (monEn.MoveNext ()) {
                short monId = monEn.Current.Key;
                var skillIdAndLvList = monEn.Current.Value.Item2.m_skillIdAndLevelList;
                ValueTuple<DE_Skill, DE_SkillData>[] monSkillArr = new ValueTuple<DE_Skill, DE_SkillData>[skillIdAndLvList.Count];
                for (int i = 0; i < skillIdAndLvList.Count; i++) {
                    DE_Skill skillDe;
                    DE_SkillData skillDataDe;
                    if (!dem.GetSkillByIdAndLevel (skillIdAndLvList[i].Item1, skillIdAndLvList[i].Item2, out skillDe, out skillDataDe))
                        continue;
                    monSkillArr[i] = new ValueTuple<DE_Skill, DE_SkillData> (skillDe, skillDataDe);
                }
                m_monsterSkillDict[monId] = monSkillArr;
            }
        }
        public void InitSkillCoolDown (int netId) {
            m_monsterSkillCoolDownDict[netId] = new List<(short, MyTimer.Time)> ();
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