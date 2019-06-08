using System.Numerics;
using System;
using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 怪物与角色  
    /// </summary>
    class DEM_Unit {
        private Dictionary<short, ValueTuple<DE_Unit, DE_MonsterData>> m_monsterDict = new Dictionary<short, ValueTuple<DE_Unit, DE_MonsterData>> ();
        private Dictionary<OccupationType, ValueTuple<DE_Unit, DE_CharacterData>[]> m_characterDict = new Dictionary<OccupationType, ValueTuple<DE_Unit, DE_CharacterData>[]> ();
        private IReadOnlyList<ValueTuple<short, Vector2>> m_monsterIdAndRespawnPositionList;
        public DEM_Unit (IDS_Monster monDs, IDS_Character charDs, IDS_Map mapDs) {
            var monsterDoArr = monDs.GetAllMonster ();
            var charDoAllLvArr = charDs.GetAllCharacter ();
            foreach (var monsterDo in monsterDoArr)
                m_monsterDict.Add (monsterDo.m_monsterId, new ValueTuple<DE_Unit, DE_MonsterData> (new DE_Unit (monsterDo), new DE_MonsterData (monsterDo)));
            foreach (var charDoAllLv in charDoAllLvArr) {
                ValueTuple<DE_Unit, DE_CharacterData>[] charAllLv = new ValueTuple<DE_Unit, DE_CharacterData>[charDoAllLv.Length];
                for (int i = 0; i < charDoAllLv.Length; i++)
                    charAllLv[i] = new ValueTuple<DE_Unit, DE_CharacterData> (new DE_Unit (charDoAllLv[i]), new DE_CharacterData (charDoAllLv[i]));
                m_characterDict.Add (charDoAllLv[0].m_occupation, charAllLv);
            }
            // 处理怪物刷新位置
            var respawnPosArr = mapDs.GetAllMonsterRespawnPosition ();
            m_monsterIdAndRespawnPositionList = new List<ValueTuple<short, Vector2>> (respawnPosArr);
        }
        public bool GetCharacterByOccupationAndLevel (OccupationType occupation, short level, out ValueTuple<DE_Unit, DE_CharacterData> res) {
            res = default(ValueTuple<DE_Unit, DE_CharacterData>);
            ValueTuple<DE_Unit, DE_CharacterData>[] charAllLv = null;
            if (!m_characterDict.TryGetValue (occupation, out charAllLv))
                return false;
            if (charAllLv.Length < level)
                return false;
            res = charAllLv[level - 1];
            return true;
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
        public int GetMonsterNum () {
            return m_monsterIdAndRespawnPositionList.Count;
        }
    }
}