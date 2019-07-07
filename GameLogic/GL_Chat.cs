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
        public void CommandSendMessage (int netId, ChattingChanelType channel, string msg, int toCharId) {

        }
    }
}