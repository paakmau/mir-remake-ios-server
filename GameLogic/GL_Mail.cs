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
            for (int i = 0; i < mailList.Count; i++) {
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
            GL_Wallet.s_instance.NotifyUpdateVirtualCurrency (netId, charId, mail.m_virtualCy);
            GL_Wallet.s_instance.NotifyUpdateChargeCurrency (netId, charId, mail.m_chargeCy);
            GL_Item.s_instance.NotifyCharacterGainItems (netId, charId, mail.m_itemIdAndNumList);
            EM_Mail.s_instance.CharacterReceiveMail (netId, mail);
            m_networkService.SendServerCommand (SC_ApplySelfReceiveMail.Instance (netId, mailId));
        }
        public void CommandApplyReceiveAllMail (int netId) {
            var mailList = EM_Mail.s_instance.GetAllMailByNetId (netId);
            var charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            if (mailList == null || charId == -1) return;
            for (int i = 0; i < mailList.Count; i++) {
                if (mailList[i].m_isReceived) continue;
                GL_Wallet.s_instance.NotifyUpdateVirtualCurrency (netId, charId, mailList[i].m_virtualCy);
                GL_Wallet.s_instance.NotifyUpdateChargeCurrency (netId, charId, mailList[i].m_chargeCy);
                GL_Item.s_instance.NotifyCharacterGainItems (netId, charId, mailList[i].m_itemIdAndNumList);
                EM_Mail.s_instance.CharacterReceiveMail (netId, mailList[i]);
                m_networkService.SendServerCommand (SC_ApplySelfReceiveMail.Instance (netId, mailList[i].m_id));
            }
        }
        public void NotifyInitCharacter (int netId, int charId) {
            EM_Mail.s_instance.InitCharacter (netId, charId);
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Mail.s_instance.RemoveCharacter (netId);
        }
        public void NotifySendMallItem (int recvNetId, int recvCharId, List < (short, short) > itemIdAndNumList) {
            SendMail (-1, "系统商城", recvNetId, recvCharId, "商城物品", "背包容量不足，购买的物品发放至邮箱", itemIdAndNumList, 0, 0);
        }
        public void NotifySendMissionReward (int recvNetId, int recvCharId, List < (short, short) > itemIdAndNumList, long virtualCy) {
            SendMail (-1, "任务报酬", recvNetId, recvCharId, "任务报酬", "背包容量不足，奖励发放至邮箱", itemIdAndNumList, virtualCy, 0);
        }
        private void SendMail (int senderCharId, string senderName, int recvNetId, int recvCharId, string title, string detail, List < (short, short) > itemIdAndNumList, long virtualCy, long chargeCy) {
            EM_Mail.s_instance.SendMail (senderCharId, senderName, recvNetId, recvCharId, title, detail, itemIdAndNumList, virtualCy, chargeCy);
        }
    }
}