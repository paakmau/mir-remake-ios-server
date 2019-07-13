using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Item {
        List<DDO_Item> GetBagByCharacterId (int charId);
        List<DDO_Item> GetStoreHouseByCharacterId (int charId);
        /// <summary>
        /// 获取角色身上的 (装备区内) 装备
        /// </summary>
        List<DDO_Item> GetEquipmentRegionByCharacterId (int charId);
        /// <summary> 获取属于该角色的所有装备的信息 (无论储存库) </summary>
        List<DDO_EquipmentInfo> GetAllEquipmentByCharacterId (int charId);
        /// <summary> 获取属于该角色的所有附魔符的信息 (无论储存库) </summary>
        List<DDO_EnchantmentInfo> GetAllEnchantmentByCharacterId (int charId);
        void UpdateItem (DDO_Item item);
        void DeleteItemByRealId (long realId);
        /// <summary>
        /// 插入一个新的Item  
        /// 返回这个Item的RealId (即数据库分配的主键)  
        /// </summary>
        long InsertItem (DDO_Item item);
        void InsertEquipmentInfo (DDO_EquipmentInfo eq);
        void UpdateEquipmentInfo (DDO_EquipmentInfo eq);
        void DeleteEquipmentInfoByRealId (long realId);
        void UpdateEnchantmentInfo (DDO_EnchantmentInfo enchantmentInfo);
        void InsertEnchantmentInfo (DDO_EnchantmentInfo enchantmentInfo);
        void DeleteEnchantmentInfoByRealId (long realId);
    }
}