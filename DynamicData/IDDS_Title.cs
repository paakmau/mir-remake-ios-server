
namespace MirRemakeBackend.DynamicData {
    interface IDDS_Title {
        bool InsertAttachedTitle (int charId, short misId);
        void UpdateAttachedTitle (int charId, short misId);
        short GetAttachedTitle (int charId);
    }
}