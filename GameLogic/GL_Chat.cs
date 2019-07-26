using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理聊天
    /// </summary>
    class GL_Chat : GameLogicBase {
        public static GL_Chat s_instance;
        public GL_Chat (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandSendMessage (int netId, ChattingChanelType channel, string msg, int toNetId) {
            var senderCharObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            if (senderCharObj == null)
                return;
            var charEn = EM_Character.s_instance.GetCharacterEnumerator ();
            switch (channel) {
                case ChattingChanelType.PRIVATE:
                    while (charEn.MoveNext ()) {
                        if (charEn.Current.Key == toNetId) {
                            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (charEn.Current.Value.m_networkId, channel, senderCharObj.m_characterId, senderCharObj.m_name, msg));
                            break;
                        }
                    }
                    break;
                case ChattingChanelType.WORLD:
                    while (charEn.MoveNext ())
                        if (charEn.Current.Key != netId)
                            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (charEn.Current.Value.m_networkId, channel, senderCharObj.m_characterId, senderCharObj.m_name, msg));
                    break;
            }
        }
        public void NotifyBuyItemBagFullSendToMailBoxSendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "背包已满, 所购商品发送至邮箱"));
        }
        public void NotifyBuyItemBagFullSendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "背包已满, 无法购买"));
        }
        public void NotifyBuyItemCyErrSendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "购买方式不正确, 无法购买"));
        }
        public void NotifyBuyItemShortOfCySendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "货币不足, 无法购买"));
        }
        public void NotifyPickUpGroundItemBagFullSendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "背包已满, 无法拾取"));
        }
        public void NotifyMissionRewardBagFullSendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "背包已满, 任务报酬发送至邮箱"));
        }
        public void NotifyReceiveMailBagFullSendMessage (int netId) {
            m_networkService.SendServerCommand (SC_ApplyAllReceiveMessage.Instance (netId, ChattingChanelType.SYSTEM, -1, "", "背包已满, 邮件领取失败"));
        }
    }
}