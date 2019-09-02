using System.Collections.Generic;
using System;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EM_Notice {
        public static EM_Notice s_instance;
        private IDDS_Notice m_dds;
        private const int c_noticePoolSize = 100;
        private ObjectPool<E_Notice> m_noticePool = new ObjectPool<E_Notice> (c_noticePoolSize);
        public EM_Notice (IDDS_Notice dds) { m_dds = dds; }
        public void ReleaseNotice (string title, string detail) {
            m_dds.InsertNotice (new DDO_Notice (-1, DateTime.Now, title, detail));
        }
        public void DeleteNotice (int id) {
            m_dds.DeleteNoticeById (id);
        }
        public List<E_Notice> GetAllNotice () {
            var ddoList = m_dds.GetAllNotice ();
            var res = new List<E_Notice> (ddoList.Count);
            for (int i=0; i<ddoList.Count; i++) {
                var notice = m_noticePool.GetInstance ();
                notice.Reset (ddoList[i]);
                res.Add (notice);
            }
            return res;
        }
    }
}