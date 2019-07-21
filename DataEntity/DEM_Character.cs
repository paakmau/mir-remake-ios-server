using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 怪物与角色  
    /// </summary>
    class DEM_Character {
        private Dictionary<OccupationType, DE_Character> m_characterDict = new Dictionary<OccupationType, DE_Character> ();
        public DEM_Character (IDS_Character charDs) {
            var charDoAllLvArr = charDs.GetAllCharacter ();
            foreach (var charDoAllLv in charDoAllLvArr) {
                short charMxLv = (short) charDoAllLv.Length;
                DE_Unit[] unitDeArr = new DE_Unit[charMxLv];
                DE_CharacterData[] charDataDeArr = new DE_CharacterData[charMxLv];
                for (int i = 0; i < charMxLv; i++) {
                    unitDeArr[i] = new DE_Unit (charDoAllLv[i]);
                    charDataDeArr[i] = new DE_CharacterData (charDoAllLv[i]);
                }
                DE_Character charDe = new DE_Character (charDoAllLv[0].m_occupation, charMxLv, unitDeArr, charDataDeArr);
                m_characterDict.Add (charDoAllLv[0].m_occupation, charDe);
            }
        }
        public bool GetCharacterByOccupationAndLevel (OccupationType occupation, short level, out DE_Character resCharDe, out DE_Unit resUnitDe, out DE_CharacterData resCharDataDe) {
            resUnitDe = default (DE_Unit);
            resCharDe = default (DE_Character);
            resCharDataDe = default (DE_CharacterData);
            DE_Character charDe = null;
            if (!m_characterDict.TryGetValue (occupation, out charDe))
                return false;
            if (charDe.m_characterMaxLevel < level)
                return false;
            resCharDe = charDe;
            resUnitDe = charDe.m_unitAllLevel[level - 1];
            resCharDataDe = charDe.m_characterDataAllLevel[level - 1];
            return true;
        }
    }
}