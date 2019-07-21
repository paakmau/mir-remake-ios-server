using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引场景中所有的Character  
    /// </summary>
    class EM_Character : EntityManagerBase {
        public static EM_Character s_instance;
        private DEM_Character m_dem;
        private IDDS_Character m_charDds;
        private IDDS_CharacterAttribute m_charAttrDds;
        private IDDS_CharacterPosition m_charPosDds;
        private IDDS_CharacterWallet m_charWalletDds;
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        public EM_Character (DEM_Character dem, IDDS_Character charDds, IDDS_CharacterAttribute charAttrDds, IDDS_CharacterWallet charWalletDds, IDDS_CharacterPosition charPosDds) {
            m_dem = dem;
            m_charDds = charDds;
            m_charAttrDds = charAttrDds;
            m_charWalletDds = charWalletDds;
            m_charPosDds = charPosDds;
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
            // 持久层获取
            DDO_Character charDdo;
            DDO_CharacterAttribute charAttrDdo;
            DDO_CharacterWallet charWalletDdo;
            DDO_CharacterPosition charPosDdo;
            if (!m_charDds.GetCharacterById (charId, out charDdo) ||
                !m_charAttrDds.GetCharacterAttributeByCharacterId (charId, out charAttrDdo) ||
                !m_charWalletDds.GetCharacterWalletByCharacterId (charId, out charWalletDdo) ||
                !m_charPosDds.GetCharacterPosition (charId, out charPosDdo)
            )
                return null;
            newChar = s_entityPool.m_characterPool.GetInstance ();
            DE_Character charDe;
            DE_Unit unitDe;
            DE_CharacterData charDataDe;
            m_dem.GetCharacterByOccupationAndLevel (charDdo.m_occupation, charAttrDdo.m_level, out charDe, out unitDe, out charDataDe);
            m_networkIdAndCharacterDict[netId] = newChar;
            newChar.Reset (netId, charDe, unitDe, charDataDe, charDdo, charAttrDdo, charWalletDdo, charPosDdo);
            return newChar;
        }
        /// <summary>
        /// 从场景中移除角色  
        /// </summary>
        /// <param name="netId"></param>
        public void RemoveCharacter (int netId) {
            E_Character charObj = null;
            if (!m_networkIdAndCharacterDict.TryGetValue (netId, out charObj))
                return;
            m_networkIdAndCharacterDict.Remove (netId);
            s_entityPool.m_characterPool.RecycleInstance (charObj);
        }
        public E_Character GetCharacterByNetworkId (int netId) {
            E_Character res = null;
            m_networkIdAndCharacterDict.TryGetValue (netId, out res);
            return res;
        }
        public int GetCharIdByNetworkId (int netId) {
            E_Character res = null;
            if (m_networkIdAndCharacterDict.TryGetValue (netId, out res))
                return res.m_characterId;
            return -1;
        }
        public Dictionary<int, E_Character>.Enumerator GetCharacterEnumerator () {
            return m_networkIdAndCharacterDict.GetEnumerator ();
        }
        public void SaveCharacterAttribute (E_Character charObj) {
            m_charAttrDds.UpdateCharacterAttribute (charObj.GetAttrDdo ());
        }
        public void SaveCharacterWallet (E_Character charObj) {
            m_charWalletDds.UpdateCharacterWallet (charObj.GetWalletDdo ());
        }
        public void SaveCharacterPosition (E_Character charObj) {
            m_charPosDds.UpdateCharacterPosition (charObj.GetPosDdo ());
        }
    }
}