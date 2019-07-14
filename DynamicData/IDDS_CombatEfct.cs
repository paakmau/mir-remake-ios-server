

namespace MirRemakeBackend.DynamicData {
    interface IDDS_CombatEfct {
        /// <summary> 总榜的所有角色战斗力 </summary>
        void InsertMixCombatEfct (DDO_CombatEfct mixCombatEfct);
        void UpdateMixCombatEfct (DDO_CombatEfct mixCombatEfct);
        DDO_CombatEfct[] GetAllMixCombatEfct ();
    }
}