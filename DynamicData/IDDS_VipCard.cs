namespace MirRemakeBackend.DynamicData {
    interface IDDS_VipCard {
        void InsertVipCard (DDO_VipCard vipCard);
        DDO_VipCard GetVipCardByPlayerId (int playerId);
        void UpdateVipCard (DDO_VipCard vipCard);
    }
}