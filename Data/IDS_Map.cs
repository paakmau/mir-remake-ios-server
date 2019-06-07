using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using LitJson;
namespace MirRemakeBackend.Data {
    interface IDS_Map {
        /// <summary>
        /// 获取所有怪物的刷新位置
        /// </summary>
        /// <returns>
        /// 键值对   
        /// Key: MonsterId  
        /// Value: 刷新位置
        /// </returns>
        ValueTuple<short, Vector2>[] GetAllMonsterRespawnPosition ();
    }
    class DS_MapImpl : IDS_Map {
        public ValueTuple<short, Vector2>[] GetAllMonsterRespawnPosition () {
            string jsonFile = File.ReadAllText ("Data/D_Map.json");
            JsonData data = JsonMapper.ToObject (jsonFile);
            ValueTuple<short, Vector2>[] positions = new ValueTuple<short, Vector2>[data.Count];
            for (int i = 0; i < data.Count; i++) {
                Vector2 vector = new Vector2 (float.Parse (data[i]["x"].ToString ()), float.Parse (data[i]["y"].ToString ()));
                positions[i] = new ValueTuple<short, Vector2> (short.Parse (data[i]["ID"].ToString ()), vector);
            }
            return positions;
        }
    }
}