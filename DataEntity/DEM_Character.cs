using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 数据型Entity的容器  
    /// 怪物  
    /// </summary>
    class DEM_Character {
        public static DEM_Character s_instance;
        private Dictionary<OccupationType, DE_Character[]> m_characterDict = new Dictionary<OccupationType, DE_Character[]> ();
        public DEM_Character (IDS_Character charDs) {
            var charDoAllLvArr = charDs.GetAllCharacter ();
            foreach (var charDoAllLv in charDoAllLvArr) {
                DE_Character[] charAllLv = new DE_Character[charDoAllLv.Length];
                for (int i=0; i<charDoAllLv.Length; i++)
                    charAllLv[i] = new DE_Character (charDoAllLv[i]);
                m_characterDict.Add (charDoAllLv[0].m_occupation, charAllLv);
            }
        }
        public DE_Character GetCharacterByOccupationAndLevel (OccupationType occupation, short level) {
            DE_Character[] charAllLv = null;
            if (!m_characterDict.TryGetValue (occupation, out charAllLv))
                return null;
            if (charAllLv.Length < level)
                return null;
            return charAllLv[level - 1];
        }
    }
}