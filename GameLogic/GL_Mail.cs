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
            NO_Mail[] mailArr = new NO_Mail[mailBox.Count];
            for (int i = 0; i < mailBox.Count; i++)
                mailArr[i] = mailBox[i].GetNo ();
            m_networkService.SendServerCommand (SC_ApplySelfShowMailBox.Instance (netId, mailArr));
        }
        public void CommandApplyReadMail (int netId, int mailId) {
            var mail = EM_Mail.s_instance.GetMailByNetIdAndMailId (netId, mailId);
            if (mail == null) return;
            EM_Mail.s_instance.CharacterReadMail (netId, mail);
            m_networkService.SendServerCommand (SC_ApplySelfReadMail.Instance (netId, mailId));
        }
        public void CommandApplyReadAllMail (int netId) {
            var mailList = EM_Mail.s_instance.GetAllMailByNetId (netId);
            if (mailList == null) return;
            for (int i=0; i<mailList.Count; i++) {
                if (mailList[i].m_isRead) continue;
                EM_Mail.s_instance.CharacterReadMail (netId, mailList[i]);
                m_networkService.SendServerCommand (SC_ApplySelfReadMail.Instance (netId, mailList[i].m_id));
            }
        }
        public void CommandApplyReceiveMail (int netId, int mailId) {
            var mail = EM_Mail.s_instance.GetMailByNetIdAndMailId (netId, mailId);
            var charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            if (mail == null || charId == -1) return;
            if (mail.m_isReceived) return;
            var itemIdAndNumList = mail.m_itemIdAndNumList;
            EM_Mail.s_instance.CharacterReceiveMail (netId, mail);
            m_networkService.SendServerCommand (SC_ApplySelfReceiveMail.Instance (netId, mailId));
            GL_Item.s_instance.NotifyCharacterGainItems (netId, charId, itemIdAndNumList);
        }
        public void CommandApplyReceiveAllMail (int netId) {
            var mailList = EM_Mail.s_instance.GetAllMailByNetId (netId);
            var charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            if (mailList == null || charId == -1) return;
            for (int i=0; i<mailList.Count; i++) {
                if (mailList[i].m_isReceived) continue;
                var itemIdAndNumList = mailList[i].m_itemIdAndNumList;
                EM_Mail.s_instance.CharacterReceiveMail (netId, mailList[i]);
                GL_Item.s_instance.NotifyCharacterGainItems (netId, charId, itemIdAndNumList);
            }
        }
        public void NotifyInitCharacter (int netId, int charId) {
            EM_Mail.s_instance.InitCharacter (netId, charId);
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Mail.s_instance.RemoveCharacter (netId);
        }
        public void NotifySendMail (int senderCharId, string senderName, int recvNetId, int recvCharId, string title, string detail, List < (short, short) > itemIdAndNumList) {
            EM_Mail.s_instance.SendMail (senderCharId, senderName, recvNetId, recvCharId, title, detail, itemIdAndNumList);
        }
    }
}