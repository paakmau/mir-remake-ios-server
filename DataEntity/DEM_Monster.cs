using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 数据型Entity的容器  
    /// 怪物  
    /// </summary>
    class DEM_Monster {
        public static DEM_Monster s_instance;
        private Dictionary<short, DE_Monster> m_monsterDict = new Dictionary<short, DE_Monster> ();
        public DEM_Monster (IDS_Monster monsterDs) {
            var monsterDoArr = monsterDs.GetAllMonster ();
            foreach (var monsterDo in monsterDoArr)
                m_monsterDict.Add (monsterDo.m_monsterId, new DE_Monster (monsterDo));
        }
        public DE_Monster GetMonsterById (short monsterId) {
            DE_Monster res = null;
            m_monsterDict.TryGetValue (monsterId, out res);
            return res;
        }
        public Dictionary<short, DE_Monster>.Enumerator GetAllMonsterEn () {
            return m_monsterDict.GetEnumerator ();
        }
    }
}