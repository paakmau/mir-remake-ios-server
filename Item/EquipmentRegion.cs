using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class EquipmentRegion {
        Dictionary<EquipmentPosition, E_EquipmentItem> m_equipPositionAndEquipmentDict = new Dictionary<EquipmentPosition, E_EquipmentItem> ();
        List<E_EquipmentItem> m_ringList = new List<E_EquipmentItem> ();
        public List<E_Item> GetAllItem () {
            List<E_Item> res = new List<E_Item> ();
            var equipmentEn = m_equipPositionAndEquipmentDict.Values.GetEnumerator ();
            while (equipmentEn.MoveNext ())
                res.Add (equipmentEn.Current);
            for (int i=0; i<m_ringList.Count; i++)
                res.Add (m_ringList[i]);
            return res;
        }
        public List<E_EquipmentItem> GetAllEquipment () {
            var res = CollectionUtils.DictionaryToValueList (m_equipPositionAndEquipmentDict);
            res.AddRange (m_ringList);
            return res;
        }
        public E_EquipmentItem GetEquipmentByEquipPosition (EquipmentPosition ePos) {
            E_EquipmentItem res = null;
            m_equipPositionAndEquipmentDict.TryGetValue (ePos, out res);
            return res;
        }
    }
}