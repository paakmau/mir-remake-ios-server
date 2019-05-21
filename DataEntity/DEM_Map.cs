using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 数据型Entity的容器  
    /// 地图  
    /// </summary>
    class DEM_Map {
        public static DEM_Map s_instance;
        private IReadOnlyList<KeyValuePair<short, Vector2>> m_monsterIdAndRespawnPositionList;
        public DEM_Map (IDS_Map mapDs) {
            var respawnPosArr = mapDs.GetAllMonsterRespawnPosition ();
            m_monsterIdAndRespawnPositionList = new List<KeyValuePair<short, Vector2>> (respawnPosArr);
        }
        public IReadOnlyList<KeyValuePair<short, Vector2>> GetAllMonsterIdAndRespawnPosition () {
            return m_monsterIdAndRespawnPositionList;
        }
    }
}