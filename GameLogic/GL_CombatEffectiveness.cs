using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理战斗力计算 与 获取
    /// </summary>
    class GL_CombatEffectiveness : GameLogicBase {
        public static GL_CombatEffectiveness s_instance;
        public GL_CombatEffectiveness (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandGetEffectivenessChange (int netId) {
            // TODO: 客户端要求获取战斗力
        }
        public void NotifyCombatEffectivenessChange (E_Character unit) {
            // TODO: 计算战斗力变化 并 储存
        }
    }
}