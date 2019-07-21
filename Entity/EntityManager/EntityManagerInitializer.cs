using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    static class EntityManagerInitializer {
        public static void Init (DEM_Skill skillDem, DEM_Monster monDem) {
            // 初始化所有的怪物
            var monIdAndPosList = monDem.GetAllMonsterIdAndRespawnPosition ();
            var netIdArr = NetworkIdManager.s_instance.AssignNetworkId (monIdAndPosList.Count);
            var idAndPosList = monDem.GetAllMonsterIdAndRespawnPosition ();
            for (int i = 0; i < idAndPosList.Count; i++) {
                // 实例化 monster
                (DE_Unit, DE_MonsterData) deTuple;
                monDem.GetMonsterById (idAndPosList[i].Item1, out deTuple);
                E_Monster monster = new E_Monster ();
                monster.Reset (netIdArr[i], idAndPosList[i].Item2, deTuple.Item1, deTuple.Item2);

                // monster
                EM_Monster.s_instance.AddMonster (monster);
                // 视野
                EM_Sight.s_instance.InitMonster (monster);
                // boss
                if (monster.m_MonsterType == MonsterType.BOSS || monster.m_MonsterType == MonsterType.FINAL_BOSS)
                    EM_BossDamage.s_instance.AddBoss (monster.m_networkId);
            }

            // 所有怪物技能
            var monEn = monDem.GetAllMonsterEn ();
            while (monEn.MoveNext ()) {
                short monId = monEn.Current.Key;
                var skillIdAndLvList = monEn.Current.Value.Item2.m_skillIdAndLevelList;
                E_MonsterSkill[] monSkillArr = new E_MonsterSkill[skillIdAndLvList.Count];
                for (int i = 0; i < skillIdAndLvList.Count; i++) {
                    DE_Skill skillDe;
                    DE_SkillData skillDataDe;
                    if (!skillDem.GetSkillByIdAndLevel (skillIdAndLvList[i].Item1, skillIdAndLvList[i].Item2, out skillDe, out skillDataDe))
                        continue;
                    monSkillArr[i] = new E_MonsterSkill (skillIdAndLvList[i].Item2, skillDe, skillDataDe);
                }
                EM_MonsterSkill.s_instance.SetMonsterSkill (monId, monSkillArr);
            }
        }
    }
}