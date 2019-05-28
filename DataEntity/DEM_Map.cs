using System;
using System.Numerics;
using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 地图  
    /// </summary>
    class DEM_Map {
        public static DEM_Map s_instance;
        private IReadOnlyList<ValueTuple<short, Vector2>> m_monsterIdAndRespawnPositionList;
        public DEM_Map (IDS_Map mapDs) {
            var respawnPosArr = mapDs.GetAllMonsterRespawnPosition ();
            m_monsterIdAndRespawnPositionList = new List<ValueTuple<short, Vector2>> (respawnPosArr);
        }
        public IReadOnlyList<ValueTuple<short, Vector2>> GetAllMonsterIdAndRespawnPosition () {
            return m_monsterIdAndRespawnPositionList;
        }
    }
}