using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理消息
    /// </summary>
    class GL_MissionLog : GameLogicBase {
        public static GL_MissionLog s_instance;
        public GL_MissionLog (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            EM_MissionLog.s_instance.NextTick ();
        }
        public override void NetworkTick () { }
        // TODO: 每种Log单独写成方法
        public void NotifyLog (MissionTargetType type, int netId, int parm1 = 0, int parm2 = 0, int parm3 = 0) {
            var logs = EM_MissionLog.s_instance.GetRawLogsCurTick ();
            logs.Add (EM_MissionLog.s_instance.CreateLog (type, netId, parm1, parm2, parm3));
        }
    }
}