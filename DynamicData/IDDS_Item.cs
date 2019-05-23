using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Item {
        List<DDO_Item> GetBagByCharacterId (int charId);
        List<DDO_Item> GetStoreHouseByCharacterId (int charId);
        /// <summary>
        /// 获取角色身上的 (装备区内) 装备
        /// </summary>
        List<DDO_Item> GetEquipmentRegionByCharacterId (int charId);
        /// <summary>
        /// 获取属于该角色的所有装备 (包括背包仓库和装备区)
        /// </summary>
        List<DDO_Equipment> GetAllEquipmentByCharacterId (int charId);
    }
}