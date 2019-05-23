using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 内存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class EntityManagerPool<T> where T : class, new () {
        private Stack<T> m_pool = new Stack<T> ();
        private int m_Size;
        public EntityManagerPool (int Size) {
            for (int i = 0; i < Size; i++)
                m_pool.Push (new T ());
            m_Size = Size;
        }
        public T GetInstance () {
            T res = null;
            if (m_pool.TryPop (out res))
                return res;
            return new T ();
        }
        public void RecycleInstance (T obj) {
            if (m_pool.Count < m_Size)
                m_pool.Push (obj);
        }
    }
    static class EntityManagerPoolInstance {
        private const int c_characterPoolSize = 400;
        private const int c_skillPoolSize = 5000;
        private const int c_repositoryPoolSize = 800;
        private const int c_equipmentRegionPoolSize = 400;
        private const int c_materialItemPoolSize = 100000;
        private const int c_consumableItemPoolSize = 100000;
        private const int c_equipmentItemPoolSize = 100000;
        public static EntityManagerPool<E_Character> s_characterPool = new EntityManagerPool<E_Character> (c_characterPoolSize);
        public static EntityManagerPool<E_Skill> s_skillPool = new EntityManagerPool<E_Skill> (c_skillPoolSize);
        public static EntityManagerPool<E_Repository> s_repositoryPool = new EntityManagerPool<E_Repository> (c_repositoryPoolSize);
        public static EntityManagerPool<E_EquipmentRegion> s_equipmentRegionPool = new EntityManagerPool<E_EquipmentRegion> (c_equipmentRegionPoolSize);
        public static EntityManagerPool<E_MaterialItem> s_materialItemPool = new EntityManagerPool<E_MaterialItem> (c_materialItemPoolSize);
        public static EntityManagerPool<E_ConsumableItem> s_consumableItemPool = new EntityManagerPool<E_ConsumableItem> (c_consumableItemPoolSize);
        public static EntityManagerPool<E_EquipmentItem> s_equipmentItemPool = new EntityManagerPool<E_EquipmentItem> (c_equipmentItemPoolSize);
    }
}