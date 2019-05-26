using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.EntityManager {
    class EntityPool {
        private const int c_characterPoolSize = 400;
        private const int c_skillPoolSize = 5000;
        private const int c_repositoryPoolSize = 800;
        private const int c_equipmentRegionPoolSize = 400;
        private const int c_materialItemPoolSize = 100000;
        private const int c_consumableItemPoolSize = 100000;
        private const int c_equipmentItemPoolSize = 100000;
        private const int c_gemItemPoolSize = 100000;
        private const int c_statusPoolSize = 4000;
        public ObjectPool<E_Character> m_characterPool = new ObjectPool<E_Character> (c_characterPoolSize);
        public ObjectPool<E_Skill> m_skillPool = new ObjectPool<E_Skill> (c_skillPoolSize);
        public ObjectPool<E_Repository> m_repositoryPool = new ObjectPool<E_Repository> (c_repositoryPoolSize);
        public ObjectPool<E_EquipmentRegion> m_equipmentRegionPool = new ObjectPool<E_EquipmentRegion> (c_equipmentRegionPoolSize);
        public ObjectPool<E_MaterialItem> m_materialItemPool = new ObjectPool<E_MaterialItem> (c_materialItemPoolSize);
        public ObjectPool<E_ConsumableItem> m_consumableItemPool = new ObjectPool<E_ConsumableItem> (c_consumableItemPoolSize);
        public ObjectPool<E_EquipmentItem> m_equipmentItemPool = new ObjectPool<E_EquipmentItem> (c_equipmentItemPoolSize);
        public ObjectPool<E_GemItem> m_gemItemPool = new ObjectPool<E_GemItem> (c_gemItemPoolSize);
        public ObjectPool<E_Status> m_statusPool = new ObjectPool<E_Status> (c_statusPoolSize);
    }
}