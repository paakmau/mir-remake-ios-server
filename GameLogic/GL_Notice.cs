using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Notice : GameLogicBase {
        public static GL_Notice s_instance;
        public GL_Notice (INetworkService ns) : base (ns) { }
        public override void Tick (float dt) { }
        public override void NetworkTick () { }
        public void CommandReleaseNotice (string title, string detail) {
            EM_Notice.s_instance.ReleaseNotice (title, detail);
        }
    }
}