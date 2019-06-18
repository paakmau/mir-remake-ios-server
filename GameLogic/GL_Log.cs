using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色
    /// 技能释放, 移动等
    /// 属性点分配
    /// </summary>
    /// </summary>
    class GL_Log : GameLogicBase {
        public static GL_Log s_instance;
        public GL_Log (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyLog (GameLogType type, int parm1, int parm2, int parm3) {
            var logs = EM_Log.s_instance.GetRawLogsCurTick ();
            logs.Add (EM_Log.s_instance.CreateLog (type, parm1, parm2, parm3));
        }
    }
}