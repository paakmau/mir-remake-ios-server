using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色快捷键
    /// </summary>
    class GL_Shortcut : GameLogicBase {
        public static GL_Shortcut s_instance;
        public GL_Shortcut (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyCreateCharacter (int charId) {
            EM_Shortcut.s_instance.CreateCharacter (charId);
        }
        public void NotifyInitCharacter (int netId, int charId) {
            IReadOnlyList < (byte, ShortcutType, long) > shortcutList;
            EM_Shortcut.s_instance.InitCharacter (charId, out shortcutList);
            // client
            var noList = new List<NO_Shortcut> (shortcutList.Count);
            for (int i=0; i<shortcutList.Count; i++)
                noList.Add (new NO_Shortcut (shortcutList[i].Item2, shortcutList[i].Item1, shortcutList[i].Item3));
            m_networkService.SendServerCommand (SC_InitSelfShortcut.Instance (netId, noList));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Wallet.s_instance.RemoveCharacter (netId);
        }
        public void CommandUpdateShortcut (int netId, byte pos, ShortcutType type, long data) {
            int charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            EM_Shortcut.s_instance.UpdateShortcut (charId, pos, type, data);
        }
    }
}