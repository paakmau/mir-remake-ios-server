using System;
using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EM_Mail {
        public static EM_Mail s_instance;
        private IDDS_Mail m_dds;
        private const int c_mailPoolSize = 2000;
        public ObjectPool<E_Mail> m_mailPool = new ObjectPool<E_Mail> (c_mailPoolSize);
        private Dictionary<int, List<E_Mail>> m_mailDict = new Dictionary<int, List<E_Mail>> ();
        public EM_Mail (IDDS_Mail dds) { m_dds = dds; }
        public void InitCharacter (int netId, int charId) {
            if (m_mailDict.ContainsKey (netId)) return;
            List<DDO_Mail> ddoList = m_dds.GetAllMailByReceiverCharacterId (charId);
            List<E_Mail> mailList = new List<E_Mail> (ddoList.Count);
            for (int i = 0; i < ddoList.Count; i++) {
                var mail = m_mailPool.GetInstance ();
                mail.Reset (ddoList[i]);
                mailList.Add (mail);
            }
            m_mailDict.Add (netId, mailList);
        }
        public void RemoveCharacter (int netId) {
            List<E_Mail> mailList;
            if (!m_mailDict.TryGetValue (netId, out mailList)) return;
            for (int i = 0; i < mailList.Count; i++)
                m_mailPool.RecycleInstance (mailList[i]);
            m_mailDict.Remove (netId);
        }
        public IReadOnlyList<E_Mail> GetAllMailByNetId (int netId) {
            List<E_Mail> res;
            m_mailDict.TryGetValue (netId, out res);
            return res;
        }
        public E_Mail GetMailByNetIdAndMailId (int netId, int mailId) {
            var mailList = GetAllMailByNetId (netId);
            if (mailList == null) return null;
            for (int i = 0; i < mailList.Count; i++)
                if (mailList[i].m_id == mailId)
                    return mailList[i];
            return null;
        }
        public void CharacterReadMail (int netId, E_Mail mail) {
            if (mail.m_isRead) return;
            mail.m_isRead = true;
            m_dds.UpdateMailRead (mail.m_id, mail.m_isRead);
        }
        public void CharacterReceiveMail (int netId, E_Mail mail) {
            if (mail.m_isReceived) return;
            mail.m_isReceived = true;
            m_dds.UpdateMailReceived (mail.m_id, mail.m_isReceived);
        }
        public void SendMail (int senderCharId, string senderName, int recvNetId, int recvCharId, string title, string detail, IReadOnlyList < (short, short) > itemIdAndNumList, long virtualCy, long chargeCy) {
            E_Mail mail = m_mailPool.GetInstance ();
            mail.Reset (-1, senderCharId, senderName, recvCharId, DateTime.Now, title, detail, itemIdAndNumList, virtualCy, chargeCy, false, false);
            mail.m_id = m_dds.InsertMail (mail.GetDdo ());

            List<E_Mail> recvMailBox;
            if (m_mailDict.TryGetValue (recvNetId, out recvMailBox)) {
                recvMailBox.Add (mail);
            } else {
                m_mailPool.RecycleInstance (mail);
            }
        }
    }
}