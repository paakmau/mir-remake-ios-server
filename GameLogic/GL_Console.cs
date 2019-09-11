using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理聊天
    /// </summary>
    class GL_Console : GameLogicBase {
        public static GL_Console s_instance;
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
        public void CommandReleaseNotice (int netId, string title, string detail) {
            GL_Notice.s_instance.NotifyReleaseNotice (title, detail);
            m_networkService.SendServerCommand (SC_ConsoleSuccess.Instance (netId));
        }
        public void CommandDeleteNotice (int netId, int id) {
            GL_Notice.s_instance.NotifyDeleteNotice (id);
            m_networkService.SendServerCommand (SC_ConsoleSuccess.Instance (netId));
        }
        public void CommandChargeMoney (int netId, string charName, int money) {
            var charId = EM_Character.s_instance.GetCharIdByName (charName);
            if (charId == -1) {
                m_networkService.SendServerCommand (SC_ConsoleFail.Instance (netId, "未知错误"));
                return;
            }
            GL_Wallet.s_instance.NotifyChargeMoney (netId, charId, money);
            m_networkService.SendServerCommand (SC_ConsoleSuccess.Instance (netId));
        }
    }
}