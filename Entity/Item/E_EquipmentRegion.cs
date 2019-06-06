using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    class E_EquipmentRegion {
        Dictionary<EquipmentPosition, E_EquipmentItem> m_equipPositionAndEquipmentDict = new Dictionary<EquipmentPosition, E_EquipmentItem> ();
        public void Reset (E_Item[] itemArr) {
            m_equipPositionAndEquipmentDict.Clear ();
            foreach (var item in itemArr)
                m_equipPositionAndEquipmentDict.Add (((E_EquipmentItem) item).m_EquipmentPosition, (E_EquipmentItem) item);
        }
        public NO_Repository GetNo () {
            List<NO_Item> itemList = new List<NO_Item> (m_equipPositionAndEquipmentDict.Count);
            List<NO_EquipmentItemInfo> eqInfoList = new List<NO_EquipmentItemInfo> (m_equipPositionAndEquipmentDict.Count);
            var en = m_equipPositionAndEquipmentDict.Values.GetEnumerator ();
            while (en.MoveNext ()) {
                itemList.Add (en.Current.GetItemNo ());
                eqInfoList.Add (en.Current.GetEquipmentInfoNo ());
            }
            return new NO_Repository (itemList, eqInfoList);
        }
        public List<E_Item> GetAllItemList () {
            List<E_Item> res = new List<E_Item> ();
            var equipmentEn = m_equipPositionAndEquipmentDict.Values.GetEnumerator ();
            while (equipmentEn.MoveNext ())
                res.Add (equipmentEn.Current);
            return res;
        }
        public Dictionary<EquipmentPosition, E_EquipmentItem>.Enumerator GetEquipedEn () {
            return m_equipPositionAndEquipmentDict.GetEnumerator ();
        }
        public E_EquipmentItem GetEquipmentByEquipPosition (EquipmentPosition ePos) {
            E_EquipmentItem res = null;
            m_equipPositionAndEquipmentDict.TryGetValue (ePos, out res);
            return res;
        }
        public E_EquipmentItem PutOnEquipment (E_EquipmentItem eq) {
            E_EquipmentItem res = GetEquipmentByEquipPosition (eq.m_EquipmentPosition);
            m_equipPositionAndEquipmentDict[eq.m_EquipmentPosition] = res;
            return res;
        }
    }
}