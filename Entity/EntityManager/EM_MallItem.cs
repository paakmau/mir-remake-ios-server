
namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理商城物品
    /// </summary>
    class EM_MallItem : EntityManagerBase {
        private class MallItemIdManager {
            short m_mallItemIdCnt = 0;
            
        }
        public static EM_Camp s_instance;
        public CampType GetCampType (E_Unit self, E_Unit target) {
            if (self == target)
                return CampType.SELF;
            return CampType.ENEMY;
        }
    }
}