using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    partial class EM_Item {
        private class ItemDynamicDataHelper {
            #region ItemDealer
            private class ItemInfoDdoCollections {
                private Dictionary<long, DDO_EquipmentInfo> m_eqInfoDict = new Dictionary<long, DDO_EquipmentInfo> ();
                private Dictionary<long, DDO_EnchantmentInfo> m_ecmtInfoDict = new Dictionary<long, DDO_EnchantmentInfo> ();
                public void Reset (List<DDO_EquipmentInfo> eqInfoDdoList, List<DDO_EnchantmentInfo> ecmtInfoDdoList) {
                    m_eqInfoDict.Clear ();
                    for (int i = 0; i < eqInfoDdoList.Count; i++)
                        m_eqInfoDict.Add (eqInfoDdoList[i].m_realId, eqInfoDdoList[i]);
                    for (int i = 0; i < ecmtInfoDdoList.Count; i++)
                        m_ecmtInfoDict.Add (ecmtInfoDdoList[i].m_realId, ecmtInfoDdoList[i]);
                }
                public bool TryGetEquipment (long realId, out DDO_EquipmentInfo resInfo) {
                    return m_eqInfoDict.TryGetValue (realId, out resInfo);
                }
                public bool TryGetEnchantment (long realId, out DDO_EnchantmentInfo resInfo) {
                    return m_ecmtInfoDict.TryGetValue (realId, out resInfo);
                }
            }
            private interface IItemDealer {
                ItemType m_ItemType { get; }
                long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos);
                void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos);
                void Delete (IDDS_Item dds, E_Item item);
                void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem);
            }
            private class II_Empty : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Material : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Consumable : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Equipment : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    var realId = dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                    item.ResetRealId (realId);
                    dds.InsertEquipmentInfo (((E_EquipmentItem) item).GetEquipmentInfoDdo (charId));
                    return realId;
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                    dds.UpdateEquipmentInfo ((item as E_EquipmentItem).GetEquipmentInfoDdo (charId));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                    dds.DeleteEquipmentInfoByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) {
                    resItem.ResetRealId (realId);
                    DDO_EquipmentInfo eqDdo;
                    if (!collct.TryGetEquipment (realId, out eqDdo)) {
                        (resItem as E_EquipmentItem).ResetEquipmentData (0, new (ActorUnitConcreteAttributeType, int) [0], new List<short> (), new List<DE_GemData> ());
                        return;
                    }
                    var gemList = new List<DE_GemData> (eqDdo.m_inlaidGemIdList.Count);
                    for (int i = 0; i < eqDdo.m_inlaidGemIdList.Count; i++)
                        gemList.Add (dem.GetGemById (eqDdo.m_inlaidGemIdList[i]));
                    (resItem as E_EquipmentItem).ResetEquipmentData (eqDdo.m_strengthNum, eqDdo.m_enchantAttr, eqDdo.m_inlaidGemIdList, gemList);
                }
            }
            private class II_Gem : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    return dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) { resItem.ResetRealId (realId); }
            }
            private class II_Enchantment : IItemDealer {
                public ItemType m_ItemType { get { return ItemType.ENCHANTMENT; } }
                public long Insert (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    var realId = dds.InsertItem (item.GetItemDdo (charId, ip, pos));
                    item.ResetRealId (realId);
                    dds.InsertEnchantmentInfo ((item as E_EnchantmentItem).GetEnchantmentDdoInfo (charId));
                    return realId;
                }
                public void Save (IDDS_Item dds, E_Item item, int charId, ItemPlace ip, short pos) {
                    dds.UpdateItem (item.GetItemDdo (charId, ip, pos));
                    dds.UpdateEnchantmentInfo ((item as E_EnchantmentItem).GetEnchantmentDdoInfo (charId));
                }
                public void Delete (IDDS_Item dds, E_Item item) {
                    dds.DeleteItemByRealId (item.m_realId);
                    dds.DeleteEnchantmentInfoByRealId (item.m_realId);
                }
                public void ResetInfo (DEM_Item dem, ItemInfoDdoCollections collct, long realId, E_Item resItem) {
                    resItem.ResetRealId (realId);
                    DDO_EnchantmentInfo info;
                    if (!collct.TryGetEnchantment (realId, out info)) {
                        (resItem as E_EnchantmentItem).ResetEnchantmentData (new List < (ActorUnitConcreteAttributeType, int) > ());
                        return;
                    }
                    (resItem as E_EnchantmentItem).ResetEnchantmentData (new List < (ActorUnitConcreteAttributeType, int) > (info.m_attrArr));
                }
            }
            #endregion
            private DEM_Item m_dem;
            private IDDS_Item m_dds;
            private ItemFactory m_fact;
            private ItemInfoDdoCollections m_itemInfoDdoCollections = new ItemInfoDdoCollections ();
            private Dictionary<ItemType, IItemDealer> m_dealerDict = new Dictionary<ItemType, IItemDealer> ();
            public ItemDynamicDataHelper (DEM_Item dem, IDDS_Item dds, ItemFactory fact) {
                m_dem = dem;
                m_dds = dds;
                m_fact = fact;
                // 实例化所有 IItemDealer 的子类
                var baseType = typeof (IItemDealer);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemDealer impl = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IItemDealer;
                    m_dealerDict.Add (impl.m_ItemType, impl);
                }
            }
            public void InsertSlot (int charId, ItemPlace ip, short pos) {
                m_dds.InsertItem (new DDO_Item (-1, -1, charId, 0, ip, pos));
            }
            /// <summary>
            /// 返回realId  
            /// 同时item的realId也会被更新
            /// </summary>
            public long Insert (E_Item item, int charId, ItemPlace ip, short pos) {
                var res = m_dealerDict[item.m_Type].Insert (m_dds, item, charId, ip, pos);
                item.ResetRealId (res);
                return res;
            }
            public void Save (E_Item item, int charId, ItemPlace ip, short pos) {
                m_dealerDict[item.m_Type].Save (m_dds, item, charId, ip, pos);
            }
            public void Delete (E_Item item) {
                m_dealerDict[item.m_Type].Delete (m_dds, item);
            }
            /// <summary>
            /// 在加载角色初始信息时, 获得物品实例, 有 RealId 与 ItemInfo
            /// </summary>
            public bool GetAndResetCharacterItemInstance (int charId, out E_Item[] resBag, out E_Item[] resStoreHouse, out E_Item[] resEquiped) {
                resBag = null;
                resStoreHouse = null;
                resEquiped = null;
                var eqInfoList = m_dds.GetAllEquipmentByCharacterId (charId);
                var ecmtInfoList = m_dds.GetAllEnchantmentByCharacterId (charId);
                var bagList = m_dds.GetBagByCharacterId (charId);
                var storeHouseList = m_dds.GetStoreHouseByCharacterId (charId);
                var equipedList = m_dds.GetEquipmentRegionByCharacterId (charId);
                if (eqInfoList == null || bagList == null || storeHouseList == null || equipedList == null)
                    return false;
                // 排序
                bagList.Sort ((a, b) => { return a.m_position - b.m_position; });
                storeHouseList.Sort ((a, b) => { return a.m_position - b.m_position; });
                equipedList.Sort ((a, b) => { return a.m_position - b.m_position; });
                // 获取实例
                m_itemInfoDdoCollections.Reset (eqInfoList, ecmtInfoList);
                resBag = GetAndResetInstanceArr (bagList, m_itemInfoDdoCollections);
                resStoreHouse = GetAndResetInstanceArr (storeHouseList, m_itemInfoDdoCollections);
                resEquiped = GetAndResetInstanceArr (equipedList, m_itemInfoDdoCollections);
                return true;
            }
            private E_Item[] GetAndResetInstanceArr (List<DDO_Item> itemList, ItemInfoDdoCollections iidc) {
                E_Item[] res = new E_Item[itemList.Count];
                for (int i = 0; i < itemList.Count; i++) {
                    var itemObj = m_fact.GetAndInitInstance (itemList[i].m_itemId, itemList[i].m_num);
                    if (itemObj == null)
                        itemObj = m_fact.GetEmptyItemInstance ();
                    m_dealerDict[itemObj.m_Type].ResetInfo (m_dem, iidc, itemList[i].m_realId, itemObj);
                    res[i] = itemObj;
                }
                return res;
            }
        }
    }
}