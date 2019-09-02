using System;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Notice {
        public int m_id;
        public DateTime m_time;
        public string m_title;
        public string m_detail;
        public E_Notice () { }
        public void Reset (DDO_Notice ddo) {
            m_id = ddo.m_id;
            m_time = ddo.m_time;
            m_title = ddo.m_title;
            m_detail = ddo.m_detail;
        }
    }
}