namespace MirRemakeBackend.DynamicData {
    interface IDDS_User {
        bool GetUserByUsername (string username, out DDO_User resUser);
        void UpdateUser (DDO_User ddo);
        /// <summary>
        /// 创建一个用户  
        /// 返回 playerId  
        /// 若用户名重复返回 -1 
        /// </summary>
        int InsertUser (DDO_User ddo);
    }
}