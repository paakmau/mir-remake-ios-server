using System.Collections.Generic;
using System;

namespace MirRemakeBackend.DynamicData {
    class DynamicDataServiceTest : IDDS_Item {
        private long m_realIdCnt = 0;
        public List<DDO_Item> GetBagByCharacterId (int charId) {
            Console.WriteLine ("GetBagByCharacterId charId = {0}", charId);
            return new List<DDO_Item> ();
        }
        public List<DDO_Item> GetStoreHouseByCharacterId (int charId) {
            Console.WriteLine ("GetStoreHouseByCharacterId charId = {0}", charId);
            return new List<DDO_Item> ();
        }
        public List<DDO_Item> GetEquipmentRegionByCharacterId (int charId) {
            Console.WriteLine ("GetEquipmentRegionByCharacterId charId = {0}", charId);
            return new List<DDO_Item> ();
        }
        public List<DDO_EquipmentInfo> GetAllEquipmentByCharacterId (int charId) {
            Console.WriteLine ("GetAllEquipmentByCharacterId charId = {0}", charId);
            return new List<DDO_EquipmentInfo> ();
        }
        public void UpdateItem (DDO_Item item) {
            Console.WriteLine ("UpdateItem realId = {0}", item.m_realId);
        }
        public void DeleteItemByRealId (long realId) {
            Console.WriteLine ("DeleteItemByRealId realId = {0}", realId);
        }
        public long InsertItem (DDO_Item item) {
            var realId = ++m_realIdCnt;
            Console.WriteLine ("InsertItem realId = {0}", realId);
            return realId;
        }
        public void InsertEquipmentInfo (DDO_EquipmentInfo eq) {
            Console.WriteLine ("InsertEquipmentInfo realId = {0}", eq.m_realId);
        }
        public void UpdateEquipmentInfo (DDO_EquipmentInfo eq) {
            Console.WriteLine ("UpdateEquipmentInfo realId = {0}", eq.m_realId);
        }
        public void DeleteEquipmentInfoByRealId (long realId) {
            Console.WriteLine ("UpdateEquipmentInfo realId = {0}", realId);
        }
    }
}