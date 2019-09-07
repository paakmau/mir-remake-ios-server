using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity
{
    partial class EM_Item
    {
        private class ItemFactory {
            #region ItemInitializer
            interface IItemInitializer {
                ItemType m_ItemType { get; }
                void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num);
            }
            private class II_Empty : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.EMPTY; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    ((E_EmptyItem) resItem).Reset (de);
                }
            }
            private class II_Material : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.MATERIAL; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    ((E_MaterialItem) resItem).Reset (de, num);
                }
            }
            private class II_Consumable : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.CONSUMABLE; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    var conDe = dem.GetConsumableById (de.m_id);
                    ((E_ConsumableItem) resItem).Reset (de, conDe, num);
                }
            }
            private class II_Equipment : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.EQUIPMENT; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    var eqDe = dem.GetEquipmentById (de.m_id);
                    ((E_EquipmentItem) resItem).Reset (de, eqDe);
                }
            }
            private class II_Gem : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.GEM; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    var gemDe = dem.GetGemById (de.m_id);
                    ((E_GemItem) resItem).Reset (de, gemDe);
                }
            }
            private class II_Enchantment : IItemInitializer {
                public ItemType m_ItemType { get { return ItemType.ENCHANTMENT; } }
                public void Initialize (DEM_Item dem, E_Item resItem, DE_Item de, short num) {
                    ((E_EnchantmentItem) resItem).Reset (de);
                }
            }
            #endregion
            private DEM_Item m_dem;
            private Dictionary<ItemType, ObjectPool> m_poolDict = new Dictionary<ItemType, ObjectPool> ();
            private Dictionary<ItemType, IItemInitializer> m_itemInitializerDict = new Dictionary<ItemType, IItemInitializer> ();
            private const int c_poolSize = 2000;
            public ItemFactory (DEM_Item dem) {
                m_dem = dem;
                m_poolDict.Add (ItemType.EMPTY, new ObjectPool<E_EmptyItem> (c_poolSize));
                m_poolDict.Add (ItemType.MATERIAL, new ObjectPool<E_MaterialItem> (c_poolSize));
                m_poolDict.Add (ItemType.CONSUMABLE, new ObjectPool<E_ConsumableItem> (c_poolSize));
                m_poolDict.Add (ItemType.EQUIPMENT, new ObjectPool<E_EquipmentItem> (c_poolSize));
                m_poolDict.Add (ItemType.GEM, new ObjectPool<E_GemItem> (c_poolSize));
                m_poolDict.Add (ItemType.ENCHANTMENT, new ObjectPool<E_EnchantmentItem> (c_poolSize));
                // 实例化所有 IItemInitializer 的子类
                var baseType = typeof (IItemInitializer);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && baseType.IsAssignableFrom (p));
                foreach (var type in implTypes) {
                    IItemInitializer impl = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IItemInitializer;
                    m_itemInitializerDict.Add (impl.m_ItemType, impl);
                }
            }
            public void RecycleItem (E_Item item) {
                m_poolDict[item.m_Type].RecycleInstance (item);
            }
            public E_EmptyItem GetEmptyItemInstance () {
                return GetAndInitInstance (-1, 0) as E_EmptyItem;
            }
            public E_EnchantmentItem GetEnchantmentItemInstance () {
                return GetAndInitInstance (29000, 1) as E_EnchantmentItem;
            }
            /// <summary>
            /// 获得物品实例
            /// </summary>
            public E_Item GetAndInitInstance (short itemId, short num) {
                var de = m_dem.GetItemById (itemId);
                if (de == null) return null;
                var initializer = m_itemInitializerDict[de.m_type];
                var res = m_poolDict[de.m_type].GetInstanceObj () as E_Item;
                initializer.Initialize (m_dem, res, de, num);
                return res;
            }
        }
    }
}