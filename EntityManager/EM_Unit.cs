using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    class NetworkIdManager {
        public static NetworkIdManager s_instance = new NetworkIdManager ();
        private HashSet<int> m_actorUnitNetIdSet = new HashSet<int> ();
        private HashSet<int> m_itemNetIdSet = new HashSet<int> ();
        private int m_actorUnitCnt = 0;
        private int m_itemCnt = 0;
        public int AssignNetworkId () {
            // 分配NetworkId
            while (true) {
                ++m_actorUnitCnt;
                if (!m_actorUnitNetIdSet.Contains (m_actorUnitCnt))
                    break;
            }
            m_actorUnitNetIdSet.Add (m_actorUnitCnt);
            return m_actorUnitCnt;
        }
        public int[] AssignNetworkId (int num) {
            int[] res = new int[num];
            for (int i = 0; i < num; i++)
                res[i] = AssignNetworkId ();
            return res;
        }
        public void RemoveNetworkId (int netId) {
            m_actorUnitNetIdSet.Remove (netId);
        }
    }
    /// <summary>
    /// 索引场景中所有的单位  
    /// 怪物不需要内存池因为每个怪物都需要Respawn且不会永久消失  
    /// </summary>
    class EM_Unit : EntityManagerBase {
        public static EM_Unit s_instance;
        private DEM_Unit m_dem;
        private Dictionary<int, E_Character> m_networkIdAndCharacterDict = new Dictionary<int, E_Character> ();
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        public EM_Unit (DEM_Unit dem) {
            m_dem = dem;
            // 实例化所有的怪物
            var idAndPosList = m_dem.GetAllMonsterIdAndRespawnPosition ();
            int[] netIdArr = NetworkIdManager.s_instance.AssignNetworkId (idAndPosList.Count);
            for (int i = 0; i < idAndPosList.Count; i++) {
                ValueTuple<DE_Unit, DE_MonsterData> deTuple;
                m_dem.GetMonsterById (idAndPosList[i].Item1, out deTuple);
                E_Monster monster = new E_Monster ();
                monster.Reset (netIdArr[i], idAndPosList[i].Item2, deTuple.Item1, deTuple.Item2);
                m_networkIdAndMonsterDict[netIdArr[i]] = monster;
            }
        }
        public int AssignCharacterNetworkId () {
            return NetworkIdManager.s_instance.AssignNetworkId ();
        }
        /// <summary>
        /// 从数据库读取角色信息  
        /// 并在场景中索引新接入的角色  
        /// 若场景中已存在该角色, 则直接返回
        /// </summary>
        public E_Character InitCharacter (int netId, int charId, DDO_Character charDdo) {
            E_Character newChar = null;
            if (m_networkIdAndCharacterDict.TryGetValue (netId, out newChar))
                return newChar;
            newChar = s_entityPool.m_characterPool.GetInstance ();
            ValueTuple<DE_Unit, DE_CharacterData> deTuple;
            m_dem.GetCharacterByOccupationAndLevel (charDdo.m_occupation, charDdo.m_level, out deTuple);
            m_networkIdAndCharacterDict[netId] = newChar;
            newChar.Reset (netId, charId, deTuple.Item1, deTuple.Item2, charDdo);
            return newChar;
        }
        /// <summary>
        /// 从场景中移除角色  
        /// </summary>
        /// <param name="netId"></param>
        public void RemoveCharacter (int netId) {
            E_Character charObj = null;
            if (m_networkIdAndCharacterDict.TryGetValue (netId, out charObj))
                return;
            s_entityPool.m_characterPool.RecycleInstance (charObj);
        }
        public E_Monster GetMonsterByNetworkId (int netId) {
            E_Monster res = null;
            m_networkIdAndMonsterDict.TryGetValue (netId, out res);
            return res;
        }
        public E_Character GetCharacterByNetworkId (int netId) {
            E_Character res = null;
            m_networkIdAndCharacterDict.TryGetValue (netId, out res);
            return res;
        }
        public Dictionary<int, E_Character>.Enumerator GetCharacterEnumerator () {
            return m_networkIdAndCharacterDict.GetEnumerator ();
        }
        public Dictionary<int, E_Monster>.Enumerator GetMonsterEn () {
            return m_networkIdAndMonsterDict.GetEnumerator ();
        }
    }
}