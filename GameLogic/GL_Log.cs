using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理消息
    /// </summary>
    class GL_Log : GameLogicBase {
        public static GL_Log s_instance;
        public GL_Log (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            EM_Log.s_instance.NextTick ();
        }
        public override void NetworkTick () { }
        public void NotifyLog (GameLogType type, int parm1 = 0, int parm2 = 0, int parm3 = 0) {
            var logs = EM_Log.s_instance.GetRawLogsCurTick ();
            logs.Add (EM_Log.s_instance.CreateLog (type, parm1, parm2, parm3));
        }
    }
}