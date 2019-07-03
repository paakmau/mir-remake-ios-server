using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EntityManagerBase {
        protected static EntityPool s_entityPool = new EntityPool ();
    }

    class EntityPool {
        private const int c_characterPoolSize = 400;
        private const int c_skillPoolSize = 5000;
        private const int c_repositoryPoolSize = 800;
        private const int c_equipmentRegionPoolSize = 400;
        private const int c_statusPoolSize = 4000;
        private const int c_missionPoolSize = 1000;
        public ObjectPool<E_Character> m_characterPool = new ObjectPool<E_Character> (c_characterPoolSize);
        public ObjectPool<E_Skill> m_skillPool = new ObjectPool<E_Skill> (c_skillPoolSize);
        public ObjectPool<E_Repository> m_repositoryPool = new ObjectPool<E_Repository> (c_repositoryPoolSize);
        public ObjectPool<E_EquipmentRegion> m_equipmentRegionPool = new ObjectPool<E_EquipmentRegion> (c_equipmentRegionPoolSize);
        public ObjectPool<E_Mission> m_missionPool = new ObjectPool<E_Mission> (c_missionPoolSize);
    }
}