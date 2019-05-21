using System.Collections.Generic;


namespace MirRemakeBackend {
    /// <summary>
    /// 索引场景中所有的角色  
    /// </summary>
    class EM_Character {
        public static EM_Character s_instance;
        private IDDS_Character m_characterDds;
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        public EM_Character (IDDS_Character charDds) {
            m_characterDds = charDds;
        }
        /// <summary>
        /// 从数据库读取角色信息  
        /// 并在场景中索引新接入的角色  
        /// 若场景中已存在该角色, 则直接返回
        /// </summary>
        public E_Character InitCharacter (int netId, int charId) {
            E_Character newChar = null;
            if (m_networkIdAndCharacterDict.TryGetValue (netId, out newChar))
                return newChar;
            newChar = EntityManagerPoolInstance.s_characterPool.GetInstance ();
            DDO_Character charDdo = m_characterDds.GetCharacterById (charId);
            DE_Character charDe = DEM_Character.s_instance.GetCharacterByOccupationAndLevel (charDdo.m_occupation, charDdo.m_level);
            m_networkIdAndCharacterDict[netId] = newChar;
            newChar.Reset (netId, charId, charDe, charDdo);
            return newChar;
        }
        /// <summary>
        /// 从场景中移除角色  
        /// </summary>
        /// <param name="netId"></param>
        public void RemoveCharacterByNetworkId (int netId) {
            E_Character charObj = null;
            if (m_networkIdAndCharacterDict.TryGetValue (netId, out charObj))
                return;
            EntityManagerPoolInstance.s_characterPool.RecycleInstance (charObj);
        }
    }
}