using System.Collections.Generic;


namespace MirRemakeBackend {
    interface DDS_Item {
        List<DDO_Item> GetAllItemInBagByPlayerId (int playerId);
        List<DDO_Item> GetAllItemInStoreHouseByPlayerId (int playerId);
        void DeleteItemByRealId (long realId);
        DDO_EquipmentInfo GetEquipmentInfoByRealId (long realId);
    }
}