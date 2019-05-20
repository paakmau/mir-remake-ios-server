using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    interface IDS_Monster {
        DO_Monster GetMonsterById (short monsterId);
        /// <summary>
        /// 获取所有怪物的刷新位置
        /// </summary>
        /// <returns>
        /// 键值对   
        /// Key: MonsterId  
        /// Value: 刷新位置
        /// </returns>
        KeyValuePair<short, Vector2>[] GetAllMonsterSpawnPosition ();
    }
}