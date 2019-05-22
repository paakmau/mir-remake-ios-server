using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 控制Monster的AI
    /// </summary>
    class GL_MonsterAction : GameLogicBase {
        public GL_MonsterAction (INetworkService netService) : base (netService) { }
        public override void Tick(float dT) { }
        public override void NetworkTick() { }
    }
}