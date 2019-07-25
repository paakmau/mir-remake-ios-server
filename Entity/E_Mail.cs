using System.Collections.Generic;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Mail {
        public int m_id;
        public int m_senderCharId;
        public int m_receiverCharId;
        public string m_title;
        public string m_detail;
        public List < (short, short) > m_itemIdAndNumList;
        public void Reset (DDO_Mail ddo) {
            m_id = ddo.m_id;
            m_senderCharId = ddo.m_senderCharId;
            m_receiverCharId = ddo.m_receiverCharId;
            m_title = ddo.m_title;
            m_detail = ddo.m_detail;
            m_itemIdAndNumList = new List < (short, short) > (ddo.m_itemIdAndNumArr);
        }
    }
}