using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    class GL_Notice : GameLogicBase {
        public static GL_Notice s_instance;
        public GL_Notice (INetworkService ns) : base (ns) { }
        public override void Tick (float dt) {
            // TODO: 每隔24小时清理公告
        }
        public override void NetworkTick () { }
        public void CommandReleaseNotice (string title, string detail) {
            EM_Notice.s_instance.ReleaseNotice (title, detail);
        }
        public void CommandDeleteNotice (int id) {
            EM_Notice.s_instance.DeleteNotice (id);
        }
        public void CommandShowNotice () {
            var noticeList = EM_Notice.s_instance.GetAllNotice ();
        }
    }
}