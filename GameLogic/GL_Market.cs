
using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Market : GameLogicBase {
        public static GL_Market s_instance;
        public GL_Market (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
    }
}