using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 怪物与角色  
    /// </summary>
    class DEM_Monster {
        private Dictionary<short, ValueTuple<DE_Unit, DE_MonsterData>> m_monsterDict = new Dictionary<short, ValueTuple<DE_Unit, DE_MonsterData>> ();
        private IReadOnlyList<ValueTuple<short, Vector2>> m_monsterIdAndRespawnPositionList;
        public DEM_Monster (IDS_Monster monDs, IDS_MonsterMap mapDs) {
            var monsterDoArr = monDs.GetAllMonster ();
            foreach (var monsterDo in monsterDoArr)
                m_monsterDict.Add (monsterDo.m_monsterId, new ValueTuple<DE_Unit, DE_MonsterData> (new DE_Unit (monsterDo), new DE_MonsterData (monsterDo)));

            // 处理怪物刷新位置
            var respawnPosArr = mapDs.GetAllMonsterRespawnPosition ();
            m_monsterIdAndRespawnPositionList = new List<ValueTuple<short, Vector2>> (respawnPosArr);
        }
        public bool GetMonsterById (short monsterId, out ValueTuple<DE_Unit, DE_MonsterData> res) {
            return m_monsterDict.TryGetValue (monsterId, out res);
        }
        public Dictionary<short, ValueTuple<DE_Unit, DE_MonsterData>>.Enumerator GetAllMonsterEn () {
            return m_monsterDict.GetEnumerator ();
        }
        public IReadOnlyList<ValueTuple<short, Vector2>> GetAllMonsterIdAndRespawnPosition () {
            return m_monsterIdAndRespawnPositionList;
        }
    }
}