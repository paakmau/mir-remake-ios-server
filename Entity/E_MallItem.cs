using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_MallClass {
        private byte m_mallClassId;
        private string m_mallClassName;
        private List<E_MallItem> m_mallItemList;
        public E_MallClass (byte id, string name, List<E_MallItem> itemList) {
            m_mallClassId = id;
            m_mallClassName = name;
            m_mallItemList = itemList;
        }
        public NO_MallClass GetNo () {
            List<NO_MallItem> itemNoList = new List<NO_MallItem> (m_mallItemList.Count);
            for (int i = 0; i < m_mallItemList.Count; i++)
                itemNoList[i] = m_mallItemList[i].GetNo ();
            return new NO_MallClass (m_mallClassId, m_mallClassName, itemNoList);
        }
    }

    class E_MallItem {
        private DE_MallItem m_de;
        public int m_mallItemId;
        public short m_ItemId { get { return m_de.m_id; } }
        public long m_VirtualCyPrice { get { return m_de.m_virtualCyPrice; } }
        public long m_ChargeCyPrice { get { return m_de.m_chargeCyPrice; } }
        public E_MallItem (int mallItemId, DE_MallItem de) {
            m_mallItemId = mallItemId;
            m_de = de;
        }
        public NO_MallItem GetNo () {
            return new NO_MallItem (m_mallItemId, m_ItemId, m_VirtualCyPrice, m_ChargeCyPrice);
        }
    }
}