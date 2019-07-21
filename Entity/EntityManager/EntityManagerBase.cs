using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EntityManagerBase {
        protected static EntityPool s_entityPool = new EntityPool ();
    }

    class EntityPool {
        private const int c_characterPoolSize = 400;
        private const int c_skillPoolSize = 5000;
        private const int c_repositoryPoolSize = 400;
        private const int c_groundItemPoolSize = 2000;
        private const int c_marketItemPoolSize = 2000;
        private const int c_marketPoolSize = 200;
        public ObjectPool<E_Character> m_characterPool = new ObjectPool<E_Character> (c_characterPoolSize);
        public ObjectPool<E_Skill> m_skillPool = new ObjectPool<E_Skill> (c_skillPoolSize);
        public ObjectPool<E_Bag> m_bagPool = new ObjectPool<E_Bag> (c_repositoryPoolSize);
        public ObjectPool<E_StoreHouse> m_storeHousePool = new ObjectPool<E_StoreHouse> (c_repositoryPoolSize);
        public ObjectPool<E_EquipmentRegion> m_equipmentRegionPool = new ObjectPool<E_EquipmentRegion> (c_repositoryPoolSize);
        public ObjectPool<E_GroundItem> m_groundItemPool = new ObjectPool<E_GroundItem> (c_groundItemPoolSize);
        public ObjectPool<E_MarketItem> m_marketItemPool = new ObjectPool<E_MarketItem> (c_marketItemPoolSize);
        public ObjectPool<E_Market> m_marketPool = new ObjectPool<E_Market> (c_marketPoolSize);
    }
}