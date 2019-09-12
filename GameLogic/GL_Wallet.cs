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
            NotifyUpdateCurrencyOnline (netId, charId, type, dC);
        }
        public void NotifyInitCharacter (int netId, int charId) {
            var wallet = EM_Wallet.s_instance.InitCharacter (netId, charId);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (netId, wallet.Item1, wallet.Item2));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Wallet.s_instance.RemoveCharacter (netId);
        }
        public void NotifyChargeMoney (int charId, int money) {
            var netId = EM_Character.s_instance.GetNetIdByCharId (charId);
            int totalChargedmoney;
            // 更新 VipCard
            if (netId != -1)
                totalChargedmoney = EM_Wallet.s_instance.UpdateTotalChargedMoneyOnline (netId, charId, money);
            else
                totalChargedmoney = EM_Wallet.s_instance.UpdateTotalChargedMoneyOffline (charId, money);
            // 更新 wallet
            if (netId != -1)
                NotifyUpdateChargeCurrencyOnline (netId, charId, money * 100L);
            else
                EM_Wallet.s_instance.CharacterUpdateChargeCyOffline (charId, money * 100L);
            // 通知任务
            // TODO: 若角色不在线, 可以考虑记录Log到数据库, 或临时读取 Mission
            GL_MissionLog.s_instance.NotifyLogChargeAdequatelyOnline (netId, totalChargedmoney);
        }
        public void NotifyUpdateCurrencyOnline (int netId, int charId, CurrencyType type, long dC) {
            // 实例 与 数据
            if (type == CurrencyType.CHARGE)
                NotifyUpdateChargeCurrencyOnline (netId, charId, dC);
            else
                NotifyUpdateVirtualCurrencyOnline (netId, charId, dC);
        }
        public void NotifyUpdateCurrencyOffline (int charId, CurrencyType type, long dC) {
            // 实例 与 数据
            if (type == CurrencyType.CHARGE)
                EM_Wallet.s_instance.CharacterUpdateChargeCyOffline (charId, dC);
            else
                EM_Wallet.s_instance.CharacterUpdateVirtualCyOffline (charId, dC);
        }
        public void NotifyUpdateVirtualCurrencyOnline (int netId, int charId, long dC) {
            if (dC == 0) return;
            var wallet = EM_Wallet.s_instance.CharacterUpdateVirtualCyOnline (netId, charId, dC);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                netId, wallet.Item1, wallet.Item2));
        }
        public void NotifyUpdateChargeCurrencyOnline (int netId, int charId, long dC) {
            if (dC == 0) return;
            var wallet = EM_Wallet.s_instance.CharacterUpdateChargeCyOnline (netId, charId, dC);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                netId, wallet.Item1, wallet.Item2));
        }
    }
}