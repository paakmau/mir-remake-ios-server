using System.Collections.Generic;


namespace MirRemakeBackend {
    /// <summary>
    /// 索引场景中的怪物
    /// 不需要内存池因为每个怪物都需要Respawn且不会消失
    /// </summary>
    class EM_Monster {
        public static EM_Monster s_instance;
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        public EM_Monster () {
            var idAndPosList = DEM_Map.s_instance.GetAllMonsterIdAndRespawnPosition ();
            for (int i=0; i<idAndPosList.Count; i++) {
                DE_Monster monsterDe = DEM_Monster.s_instance.GetMonsterById (idAndPosList[i].Key);
                E_Monster monster = new E_Monster (NetworkIdManager.s_instance.GetNewActorUnitNetworkId (), idAndPosList[i].Value, monsterDe);
            }
        }
        public E_Monster GetMonsterByNetworkId (int netId) {
            E_Monster res = null;
            m_networkIdAndMonsterDict.TryGetValue (netId, out res);
            return res;
        }
    }
}