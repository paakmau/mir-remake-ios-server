using System.Collections.Generic;


namespace MirRemakeBackend {
    interface IDDS_Item {
        void DeleteItemByRealId (long realId);
        DDO_Equipment GetEquipmentByRealId (long realId);
        List<DDO_Item> GetAllItemInBagByCharacterId (int charId);
        List<DDO_Item> GetAllItemInStoreHouseByCharacterId (int charId);
        /// <summary>
        /// 获取角色身上的装备
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        List<DDO_Item> GetEquipedByCharacterId (int characterId);
    }
}