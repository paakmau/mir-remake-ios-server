using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理聊天
    /// </summary>
    class GL_Console : GameLogicBase {
        public static GL_Chat s_instance;
        public GL_Console (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandGainCurrencyByName (int consoleNetId, string name, CurrencyType type, long dC) {
            var charId = EM_Character.s_instance.GetCharIdByName (name);
            if (charId == -1) {
                m_networkService.SendServerCommand (SC_ConsoleFail.Instance (consoleNetId, "角色名有误"));
                return;
            }
            var netId = EM_Character.s_instance.GetNetIdByCharId (charId);
            if (netId != -1)
                GL_Wallet.s_instance.NotifyUpdateCurrency (netId, charId, type, dC);
            else
                GL_Wallet.s_instance.NotifyUpdateCurrency (charId, type, dC);
            m_networkService.SendServerCommand (SC_ConsoleSuccess.Instance (consoleNetId));
        }
    }
}