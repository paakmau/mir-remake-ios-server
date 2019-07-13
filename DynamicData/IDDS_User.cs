namespace MirRemakeBackend.DynamicData {
    interface IDDS_User {
        DDO_User GetUserByUsername (string username);
        void UpdateUser (DDO_User ddo);
        /// <summary>
        /// 创建一个用户  
        /// 返回 playerId  
        /// 若用户名重复返回 -1 
        /// </summary>
        int InsertUser (DDO_User ddo);
    }
}