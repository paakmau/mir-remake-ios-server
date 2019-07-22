using System.Collections.Generic;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 怪物不需要内存池因为每个怪物都需要Respawn且不会永久消失  
    /// </summary>
    class EM_Monster : EntityManagerBase {
        public static EM_Monster s_instance;
        private DEM_Monster m_dem;
        private Dictionary<int, E_Monster> m_networkIdAndMonsterDict = new Dictionary<int, E_Monster> ();
        public EM_Monster (DEM_Monster dem) {
            m_dem = dem;
        }
        public void AddMonster (E_Monster monster) {
            m_networkIdAndMonsterDict.Add (monster.m_networkId, monster);
        }
        public Dictionary<int, E_Monster>.Enumerator GetMonsterEn () {
            return m_networkIdAndMonsterDict.GetEnumerator ();
        }
    }
}