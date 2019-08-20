namespace MirRemakeBackend.DynamicData {
    interface IDDS_Character {
        /// <summary>
        /// 创建一个角色
        /// </summary>
        /// <returns>角色的id, 若因为重名或其他原因无法创建返回 -1</returns>
        int InsertCharacter (DDO_Character charDdo);
        bool DeleteCharacterById (int charId);
        bool UpdateCharacter (DDO_Character charDdo);
        bool GetCharacterById (int characterId, out DDO_Character resCharDdo);
        DDO_Character[] GetCharacterByPlayerId (int playerId);
        int[] GetAllCharacterId ();
        int GetCharacterIdByName (string name);
    }
    interface IDDS_CharacterAttribute {
        bool InsertCharacterAttribute (DDO_CharacterAttribute charAttr);
        bool DeleteCharacterAttributeByCharacterId (int charId);
        bool UpdateCharacterAttribute (DDO_CharacterAttribute charAttr);
        bool GetCharacterAttributeByCharacterId (int charId, out DDO_CharacterAttribute resCharAttr);
    }
    interface IDDS_CharacterPosition {
        bool InsertCharacterPosition (DDO_CharacterPosition cp);
        bool DeleteCharacterPositionByCharacterId (int charId);
        bool UpdateCharacterPosition (DDO_CharacterPosition cp);
        bool GetCharacterPosition (int charId, out DDO_CharacterPosition resCharPos);
    }
    interface IDDS_CharacterWallet {
        bool InsertCharacterWallet (DDO_CharacterWallet wallet);
        bool DeleteCharacterWalletByCharacterId (int charId);
        bool UpdateCharacterWallet (DDO_CharacterWallet wallet);
        bool GetCharacterWalletByCharacterId (int charId, out DDO_CharacterWallet resCharWallet);
    }
    interface IDDS_CharacterVipCard {
        bool InsertCharacterVipCard (DDO_CharacterVipCard vipCard);
        bool DeleteCharacterVipCardByCharacterId (int charId);
        bool UpdateCharacterVipCard (DDO_CharacterVipCard vipCard);
        bool GetCharacterVipCardByCharacterId (int charId, out DDO_CharacterVipCard resCharVipCard);
    }
}