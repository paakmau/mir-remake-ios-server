using System;
using System.Collections.Generic;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class EM_Mail : EntityManagerBase {
        private IDDS_Mail m_dds;
        private Dictionary<int, List<E_Mail>> m_mailDict = new Dictionary<int, List<E_Mail>> ();
        public EM_Mail (IDDS_Mail dds) { m_dds = dds; }
        public void InitCharacter (int netId, int charId) {
            if (m_mailDict.ContainsKey (netId)) return;
            List<DDO_Mail> ddoList = m_dds.GetAllMailByCharacterId (charId);
            List<E_Mail> mailList = new List<E_Mail> (ddoList.Count);
            for (int i=0; i<ddoList.Count; i++) {
                var mail = s_entityPool.m_mailPool.GetInstance ();
                mail.Reset (ddoList[i]);
                mailList.Add (mail);
            }
            m_mailDict.Add (netId, mailList);
        }
        public void RemoveCharacter (int netId) {
            List<E_Mail> mailList;
            if (!m_mailDict.TryGetValue (netId, out mailList)) return;
            for (int i=0; i<mailList.Count; i++)
                s_entityPool.m_mailPool.RecycleInstance (mailList[i]);
            m_mailDict.Remove (netId);
        }
        public IReadOnlyList<E_Mail> GetAllMailByNetId(int netId) {
            List<E_Mail> res;
            m_mailDict.TryGetValue (netId, out res);
            return res;
        }
        public void SendMail (int senderCharId, int recvNetId, int recvCharId, string title, string details) {
            E_Mail mail = s_entityPool.m_mailPool.GetInstance ();
            // mail.Reset (-1, senderCharId, recvCharId, DateTime.Now)
        }
    }
}