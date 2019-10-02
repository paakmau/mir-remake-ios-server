using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色快捷键
    /// </summary>
    class GL_Shortcut : GameLogicBase {
        public static GL_Wallet s_instance;
        public GL_Shortcut (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyInitCharacter (int netId, int charId) {
            var wallet = EM_Wallet.s_instance.InitCharacter (netId, charId);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (netId, wallet.Item1, wallet.Item2));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Wallet.s_instance.RemoveCharacter (netId);
        }
        public void CommandUpdateShortcut (int netId, byte position/* TODO: shortcut类型 */, long data) {
            
        }
    }
}