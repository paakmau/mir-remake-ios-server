using System;
using System.Collections.Generic;
using System.Data;
using LitJson;
using MySql.Data;
namespace MirRemakeBackend.DynamicData {
    interface IDDS_Item {
        List<DDO_Item> GetBagByCharacterId (int charId); //done
        List<DDO_Item> GetStoreHouseByCharacterId (int charId);
        /// <summary>
        /// 获取角色身上的 (装备区内) 装备
        /// </summary>
        List<DDO_Item> GetEquipmentRegionByCharacterId (int charId);
        /// <summary>
        /// 获取属于该角色的所有装备的信息 (无论储存库)
        /// </summary>
        List<DDO_EquipmentInfo> GetAllEquipmentByCharacterId (int charId);
        /// <summary>
        /// 更新一件物品  
        /// </summary>
        void UpdateItem (DDO_Item item);
        /// <summary>
        /// 删除一件物品  
        /// </summary>
        void DeleteItemByRealId (long realId);
        /// <summary>
        /// 插入一个新的Item  
        /// 返回这个Item的RealId (即数据库分配的主键)  
        /// </summary>
        long InsertItem (DDO_Item item);
        /// <summary>
        /// 插入装备信息  
        /// 无须返回Id  
        /// </summary>
        /// <param name="eq"></param>
        void InsertEquipmentInfo (DDO_EquipmentInfo eq);
        /// <summary>
        /// 删除一件装备
        /// </summary>
        void DeleteEquipmentInfoByRealId (long realId);
    }
}