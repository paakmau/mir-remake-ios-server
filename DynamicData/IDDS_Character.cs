namespace MirRemakeBackend.DynamicData {
    interface IDDS_Character {
        /// <summary>
        /// 创建一个角色
        /// </summary>
        /// <returns>角色的id</returns>
        int CreateCharacter (int playerId, OccupationType occupation, string name);
        DDO_Character GetCharacterById (int characterId);
        // DDO_Character GetCharacterByName (string name);
        void UpdateCharacter (DDO_Character charObj);
        DDO_Character[] GetCharacterByPlayerId (int playerId);
    }
}