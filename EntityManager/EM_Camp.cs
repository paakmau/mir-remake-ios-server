
namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理阵营信息, 组队
    /// </summary>
    class EM_Camp : EntityManagerBase {
        public static EM_Camp s_instance;
        public CampType GetCampType (E_Unit self, E_Unit target) {
            if (self == target)
                return CampType.SELF;
            return CampType.ENEMY;
        }
    }
}