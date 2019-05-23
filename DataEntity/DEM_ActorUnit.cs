using System;
using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 怪物与角色  
    /// </summary>
    class DEM_ActorUnit {
        public static DEM_ActorUnit s_instance;
        private Dictionary<short, Tuple<DE_ActorUnit, DE_Monster>> m_monsterDict = new Dictionary<short, Tuple<DE_ActorUnit, DE_Monster>> ();
        private Dictionary<OccupationType, Tuple<DE_ActorUnit, DE_Character>[]> m_characterDict = new Dictionary<OccupationType, Tuple<DE_ActorUnit, DE_Character>[]> ();
        public DEM_ActorUnit (IDS_Monster monDs, IDS_Character charDs) {
            var monsterDoArr = monDs.GetAllMonster ();
            var charDoAllLvArr = charDs.GetAllCharacter ();
            foreach (var monsterDo in monsterDoArr)
                m_monsterDict.Add (monsterDo.m_monsterId, new Tuple<DE_ActorUnit, DE_Monster> (new DE_ActorUnit (monsterDo), new DE_Monster (monsterDo)));
            foreach (var charDoAllLv in charDoAllLvArr) {
                Tuple<DE_ActorUnit, DE_Character>[] charAllLv = new Tuple<DE_ActorUnit, DE_Character>[charDoAllLv.Length];
                for (int i = 0; i < charDoAllLv.Length; i++)
                    charAllLv[i] = new Tuple<DE_ActorUnit, DE_Character> (new DE_ActorUnit (charDoAllLv[i]), new DE_Character (charDoAllLv[i]));
                m_characterDict.Add (charDoAllLv[0].m_occupation, charAllLv);
            }
        }
        public Tuple<DE_ActorUnit, DE_Character> GetCharacterByOccupationAndLevel (OccupationType occupation, short level) {
                Tuple<DE_ActorUnit, DE_Character>[] charAllLv = null;
                if (!m_characterDict.TryGetValue (occupation, out charAllLv))
                    return null;
                if (charAllLv.Length < level)
                    return null;
                return charAllLv[level - 1];
            }
        public Tuple<DE_ActorUnit, DE_Monster> GetMonsterById (short monsterId) {
                Tuple<DE_ActorUnit, DE_Monster> res = null;
                m_monsterDict.TryGetValue (monsterId, out res);
                return res;
            }
        public Dictionary<short, Tuple<DE_ActorUnit, DE_Monster>>.Enumerator GetAllMonsterEn () {
            return m_monsterDict.GetEnumerator ();
        }
    }
}