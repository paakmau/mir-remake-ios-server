using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理地面物品刷新
    /// </summary>
    class GL_GroundItem : GameLogicBase {
        public static GL_GroundItem s_instance;
        public GL_GroundItem (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyAddItem (E_Item item) {
            // TODO: 
        }
    }
}