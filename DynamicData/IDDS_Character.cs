namespace MirRemakeBackend.DynamicData {
    interface IDDS_Character {
        /// <summary>
        /// 创建一个角色
        /// </summary>
        /// <returns>角色的id, 若因为重名或其他原因无法创建返回 -1</returns>
        int CreateCharacter (int playerId, OccupationType occupation, string name);
        DDO_Character GetCharacterById (int characterId);
        void UpdateCharacter (DDO_Character charObj);
        DDO_Character[] GetCharacterByPlayerId (int playerId);
    }
}