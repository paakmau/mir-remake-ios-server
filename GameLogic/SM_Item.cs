using System.Collections.Generic;

namespace MirRemakeBackend {
    class SM_Item {
        private IDDS_Item m_itemDynamicDataService;
        private IDS_Item m_itemDataServive;
        public void InitCharacterItems (
            int netId,
            int charId,
            out List<E_Item> itemInBagList,
            out List<E_Item> itemInStoreHouseList,
            out List<E_Item> equipedList) {
            var itemInBagDdoList = m_itemDynamicDataService.GetAllItemInBagByCharacterId (charId);
            var itemInStoreHouseDdoList = m_itemDynamicDataService.GetAllItemInStoreHouseByCharacterId (charId);
            var equipedDdoList = m_itemDynamicDataService.GetEquipedByCharacterId (charId);
            itemInBagList = FromDdoList (itemInBagDdoList, netId);
            itemInStoreHouseList = FromDdoList (itemInStoreHouseDdoList, netId);
            equipedList = FromDdoList (equipedDdoList, netId);
            EM_Item.LoadItemList (itemInBagList);
            EM_Item.LoadItemList (itemInStoreHouseList);
            EM_Item.LoadItemList (equipedList);
        }
        private List<E_Item> FromDdoList (List<DDO_Item> itemDdoList, int holderNetId) {
            List<E_Item> res = new List<E_Item> ();
            for (int i=0; i<itemDdoList.Count; i++) {
                E_Item item = FromDdo (itemDdoList[i], holderNetId);
                res.Add (item);
            }
            return res;
        }
        private E_Item FromDdo (DDO_Item itemDdo, int holderNetId) {
            DO_Item itemDo = m_itemDataServive.GetItemById (itemDdo.m_itemId);
            switch (itemDo.m_type) {
                case ItemType.CONSUMABLE:
                    DO_ConsumableInfo cInfo = m_itemDataServive.GetConsumableInfoById (itemDdo.m_itemId);
                    DO_Effect effectDo = cInfo.m_effect;
                    E_Effect effect = new E_Effect (effectDo, holderNetId);
                    return new E_ConsumableItem (itemDdo.m_realId, itemDdo.m_itemId, itemDdo.m_num, effect);
                case ItemType.EQUIPMENT:
                    DO_EquipmentInfo equipmentDo = m_itemDataServive.GetEquipmentInfoById (itemDdo.m_itemId);
                    DDO_Equipment equipmentDdo = m_itemDynamicDataService.GetEquipmentByRealId (itemDdo.m_realId);
                    return new E_EquipmentItem (itemDdo.m_realId, itemDdo.m_itemId, equipmentDo, equipmentDdo);
                case ItemType.MATERIAL:
                    return new E_MaterialItem (itemDdo.m_realId, itemDdo.m_itemId, itemDdo.m_num);
            }
            return null;
        }
    }
}