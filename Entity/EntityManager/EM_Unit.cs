using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 索引场景中所有的单位  
    /// 怪物不需要内存池因为每个怪物都需要Respawn且不会永久消失  
    /// </summary>
    class EM_Unit : EntityManagerBase {
        private class NetworkIdManager {
            private HashSet<int> m_unitNetIdSet = new HashSet<int> ();
            private int m_unitCnt = 0;
            public int AssignNetworkId () {
                // 分配NetworkId
                while (true) {
                    ++m_unitCnt;
                    if (!m_unitNetIdSet.Contains (m_unitCnt))
                        break;
                }
                m_unitNetIdSet.Add (m_unitCnt);
                return m_unitCnt;
            }
            public int[] AssignNetworkId (int num) {
                int[] res = new int[num];
                for (int i = 0; i < num; i++)
                    res[i] = AssignNetworkId ();
                return res;
            }
            public void RecycleNetworkId (int netId) {
                m_unitNetIdSet.Remove (netId);
            }
        }
        public static EM_Unit s_instance;
        private DEM_Unit m_dem;
        private IDDS_Character m_charDds;
        private IDDS_CharacterAttribute m_charAttrDds;
        private IDDS_CharacterPosition m_charPosDds;
        private IDDS_CharacterWallet m_charWalletDds;
        private NetworkIdManager m_networkIdManager = new NetworkIdManager ();
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        private Dictionary<int, E_Monster> m_networkIdAndBossDict = new Dictionary<int, E_Monster> ();
        public EM_Unit (DEM_Unit dem, IDDS_Character charDds, IDDS_CharacterAttribute charAttrDds, IDDS_CharacterWallet charWalletDds, IDDS_CharacterPosition charPosDds) {
            m_dem = dem;
            m_charDds = charDds;
            m_charAttrDds = charAttrDds;
            m_charWalletDds = charWalletDds;
            m_charPosDds = charPosDds;

            // 初始化所有的怪物
            var monIdAndPosList = m_dem.GetAllMonsterIdAndRespawnPosition ();
            var netIdArr = m_networkIdManager.AssignNetworkId (monIdAndPosList.Count);
            var res = new E_Monster[netIdArr.Length];
            // 实例化所有的怪物
            var idAndPosList = m_dem.GetAllMonsterIdAndRespawnPosition ();
            for (int i = 0; i < idAndPosList.Count; i++) {
                ValueTuple<DE_Unit, DE_MonsterData> deTuple;
                m_dem.GetMonsterById (idAndPosList[i].Item1, out deTuple);
                E_Monster monster = new E_Monster ();
                monster.Reset (netIdArr[i], idAndPosList[i].Item2, deTuple.Item1, deTuple.Item2);
                m_networkIdAndMonsterDict[netIdArr[i]] = monster;
                res[i] = monster;
                if (monster.m_MonsterType == MonsterType.BOSS || monster.m_MonsterType == MonsterType.FINAL_BOSS)
                    m_networkIdAndBossDict.Add (monster.m_networkId, monster);
            }
        }
        public int AssignNetworkId () {
            return m_networkIdManager.AssignNetworkId ();
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
            // 释放NetId
            m_networkIdManager.RecycleNetworkId (netId);
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
        public Dictionary<int, E_Monster>.Enumerator GetMonsterEn () {
            return m_networkIdAndMonsterDict.GetEnumerator ();
        }
        public Dictionary<int, E_Monster>.Enumerator GetBossEn () {
            return m_networkIdAndBossDict.GetEnumerator ();
        }
        public void SaveCharacter (E_Character charObj) {
            m_charDds.UpdateCharacter (charObj.GetDdo ());
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