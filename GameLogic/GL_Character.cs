using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色升级, 属性点分配, 经验值获取等
    /// 以及具体属性等
    /// </summary>
    class GL_Character : GameLogicBase {
        private IDDS_Character m_characterDds;
        public GL_Character (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
            Messenger.AddListener<int, long> ("CommandApplyUseEquipmentItem", CommandApplyUseEquipmentItem);
            Messenger.AddListener<int, long> ("CommandApplyUseConsumableItem", CommandApplyUseConsumableItem);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        void CommandApplyUseEquipmentItem (int netId, long equipmentRealId) { }
        void CommandApplyUseConsumableItem (int netId, long consumableRealId) { }
    }
}