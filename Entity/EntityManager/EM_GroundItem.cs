using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.Entity {
    class EM_GroundItem : EntityManagerBase {
        public static EM_GroundItem s_instance;
        private List<E_GroundItem> m_groundItemList = new List<E_GroundItem> ();
        public void NotifyDropItem (E_Item item) {
            
        }
    }
}