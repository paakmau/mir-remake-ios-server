using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色金钱
    /// </summary>
    class GL_Wallet : GameLogicBase {
        public static GL_Wallet s_instance;
        public GL_Wallet (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandGainCurrency (int netId, CurrencyType type, long dC) {
            var charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            if (charId == -1) return;
            NotifyUpdateCurrency (netId, charId, type, dC);
        }
        public void NotifyInitCharacter (int netId, int charId) {
            var wallet = EM_Wallet.s_instance.InitCharacter (netId, charId);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (netId, wallet.Item1, wallet.Item2));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Wallet.s_instance.RemoveCharacter (netId);
        }
        public void NotifyUpdateCurrency (int netId, int charId, CurrencyType type, long dC) {
            // 实例 与 数据
            if (type == CurrencyType.CHARGE)
                NotifyUpdateChargeCurrency (netId, charId, dC);
            else
                NotifyUpdateVirtualCurrency (netId, charId, dC);
        }
        public void NotifyUpdateCurrency (int charId, CurrencyType type, long dC) {
            // 实例 与 数据
            if (type == CurrencyType.CHARGE)
                EM_Wallet.s_instance.CharacterUpdateChargeCy (charId, dC);
            else
                EM_Wallet.s_instance.CharacterUpdateVirtualCy (charId, dC);
        }
        public void NotifyUpdateVirtualCurrency (int netId, int charId, long dC) {
            if (dC == 0) return;
            var wallet = EM_Wallet.s_instance.CharacterUpdateVirtualCy (netId, charId, dC);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                netId, wallet.Item1, wallet.Item2));
        }
        public void NotifyUpdateChargeCurrency (int netId, int charId, long dC) {
            if (dC == 0) return;
            var wallet = EM_Wallet.s_instance.CharacterUpdateChargeCy (netId, charId, dC);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                netId, wallet.Item1, wallet.Item2));
        }
    }
}