using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_BossDamage : GameLogicBase {
        public static GL_BossDamage s_instance;
        public GL_BossDamage (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyBossAttacked (E_Monster boss, int dmg) { }
    }
}