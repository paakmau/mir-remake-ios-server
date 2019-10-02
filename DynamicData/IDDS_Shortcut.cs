using System.Collections.Generic;


namespace MirRemakeBackend.DynamicData {
    interface IDDS_Shortcut {
        List<DDO_Shortcut> GetShortcutsByCharId (int charId);
        void InsertShortcut (DDO_Shortcut ddo);
        void UpdateShortcutByCharIdAndPosition (DDO_Shortcut ddo);
        void DeleteShortcutByCharId (int charId);
    }
}