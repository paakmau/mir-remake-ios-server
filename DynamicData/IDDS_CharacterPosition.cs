namespace MirRemakeBackend.DynamicData
{
    interface IDDS_CharacterPosition
    {
        void InsertCharacterPosition(DDO_CharacterPosition cp);
        void UpdateCharacterPosition(DDO_CharacterPosition cp);

        DDO_CharacterPosition GetCharacterPosition(int charId);

    }
}