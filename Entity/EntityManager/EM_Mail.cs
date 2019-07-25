using System.Collections.Generic;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class EM_Mail : EntityManagerBase {
        private IDDS_Mail m_dds;
        private Dictionary<int, List<E_Mail>> m_mailDict = new Dictionary<int, List<E_Mail>> ();
        public EM_Mail (IDDS_Mail dds) { m_dds = dds; }
        public void InitCharacter (int netId, int charId) {
        }
    }
}