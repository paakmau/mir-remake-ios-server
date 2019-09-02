using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Notice : GameLogicBase {
        public static GL_Notice s_instance;
        public GL_Notice (INetworkService ns) : base (ns) { }
        public override void Tick (float dt) {
            // TODO: 每隔24小时清理公告
        }
        public override void NetworkTick () { }
        public void NotifyReleaseNotice (string title, string detail) {
            EM_Notice.s_instance.ReleaseNotice (title, detail);
        }
        public void NotifyDeleteNotice (int id) {
            EM_Notice.s_instance.DeleteNotice (id);
        }
        public void CommandShowNotice (int netId) {
            var noticeList = EM_Notice.s_instance.GetAllNotice ();
            List<NO_Notice> noList = new List<NO_Notice> (noticeList.Count);
            for (int i=0; i<noticeList.Count; i++)
                noList.Add (noticeList[i].GetNo ());
            m_networkService.SendServerCommand(SC_ApplyShowNotice.Instance (netId, noList));
        }
    }
}