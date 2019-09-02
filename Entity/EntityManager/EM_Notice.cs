using System;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class EM_Notice {
        public static EM_Notice s_instance;
        private IDDS_Notice m_dds;
        public EM_Notice (IDDS_Notice dds) { m_dds = dds; }
        public void ReleaseNotice (string title, string detail) {
            m_dds.InsertNotice (new DDO_Notice (-1, DateTime.Now, title, detail));
        }
        public void DeleteNotice (int id) {
            m_dds.DeleteNoticeById (id);
        }
    }
}