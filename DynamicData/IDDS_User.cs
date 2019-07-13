namespace MirRemakeBackend.DynamicData {
    interface IDDS_User {
        DDO_User GetUserByUsername (string username);
        void UpdateUser (DDO_User ddo);
        void InsertUser (DDO_User ddo);
    }
}