using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Mail : GameLogicBase {
        public static GL_Mail s_instance;
        public GL_Mail (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandApplyShowMailBox (int netId) {
            var mailBox = EM_Mail.s_instance.GetAllMailByNetId (netId);
            if (mailBox == null) return;
            // TODO:
        }
        public void CommandApplyReceiveMail (int netId, int mailId) {
            var mail = EM_Mail.s_instance.GetMailByNetIdAndMailId (netId, mailId);
            if (mail == null) return;
            // TODO:
        }
        public void NotifyInitCharacter (int netId, int charId) {
            EM_Mail.s_instance.InitCharacter (netId, charId);
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Mail.s_instance.RemoveCharacter (netId);
        }
        public void NotifySendMail (int senderNetId, int recvNetId, int recvCharId, string title, string detail, List < (short, short) > itemIdAndNum) {
            EM_Mail.s_instance.SendMail (senderNetId, recvNetId, recvCharId, title, detail, itemIdAndNum);
        }
    }
}