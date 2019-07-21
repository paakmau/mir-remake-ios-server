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
                            m_networkService.SendServerCommand (SC_SendMessage.Instance (charEn.Current.Value.m_networkId, channel, senderCharObj.m_characterId, senderCharObj.m_name, msg));
                            break;
                        }
                    }
                    break;
                case ChattingChanelType.WORLD:
                    while (charEn.MoveNext ())
                        if (charEn.Current.Key != netId)
                            m_networkService.SendServerCommand (SC_SendMessage.Instance (charEn.Current.Value.m_networkId, channel, senderCharObj.m_characterId, senderCharObj.m_name, msg));
                    break;
            }
        }
    }
}