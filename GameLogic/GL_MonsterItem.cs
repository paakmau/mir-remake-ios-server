using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理地面物品刷新
    /// </summary>
    class GL_MonsterItem : GameLogicBase {
        public static GL_MonsterItem s_instance;
        public GL_MonsterItem (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyDropLegacy (E_Monster monObj) {
            var dropItemIdList = monObj.m_DropItemIdList;
        }
    }
}