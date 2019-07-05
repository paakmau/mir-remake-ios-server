using System;
using System.IO;
using System.Numerics;
using LitJson;

namespace MirRemakeBackend.Data {
    interface IDS_GroundItemMap {
        /// <summary>
        /// 获取所有地面物品的刷新位置  
        /// Key: ItemId  
        /// Value: 刷新位置  
        /// </summary>
        ValueTuple<short, Vector2>[] GetAllGroundItemRespawnPosition ();
    }
    // TODO:
}