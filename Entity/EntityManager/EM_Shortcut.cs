using System.Collections.Generic;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理快捷栏的读写
    /// </summary>
    class EM_Shortcut {
        public static EM_Shortcut s_instance;
        private IDDS_Shortcut m_dds;
        public EM_Shortcut (IDDS_Shortcut dds) {
            m_dds = dds;
        }
        public void InitCharacter (int charId, out IReadOnlyList<(byte, ShortcutType, long)> shortcuts) {
            shortcuts = null;
            var sList = m_dds.GetShortcutsByCharId (charId);
            if (sList == null) return;
            var res = new List<(byte, ShortcutType, long)> (sList.Count);
            for (int i=0; i<sList.Count; i++)
                res.Add ((sList[i].m_position, sList[i].m_type, sList[i].m_data));
            shortcuts = res;
        }
        public void UpdateShortcut (int charId, byte pos, ShortcutType type, long data) {
            m_dds.UpdateShortcutByCharIdAndPosition (new DDO_Shortcut (charId, pos, type, data));
        }
    }
}