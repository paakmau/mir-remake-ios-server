using System;
using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    class DynamicDataServiceTest : IDDS_Item {
        private long m_realIdCnt = 0;
        private int m_charIdCnt = 0;
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
        // public int CreateCharacter (OccupationType occupation) {
        //     Console.WriteLine ("CreateCharacter occupation = {0}", occupation);
        //     var charId = ++m_charIdCnt;
        //     return charId;
        // }
        // DDO_Character GetCharacterById (int characterId) {
        //     Console.WriteLine ("GetCharacterById charId = {0}", characterId);
        //     return new DDO_Character (
        //         characterId,
        //         1,
        //         OccupationType.MAGE,
        //         0,
        //         new (CurrencyType, long)[2] { });
        // }
        // void UpdateCharacter (DDO_Character charObj);
    }
}