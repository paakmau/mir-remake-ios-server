using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

/// <summary>
/// 处理进入游戏之前的逻辑
/// TODO: 重构 (有缘), 把创建角色逻辑放到 EM 层里
/// </summary>
namespace MirRemakeBackend.GameLogic {
    class GL_User : GameLogicBase {
        public static GL_User s_instance;
        private const string c_version = "2.05";
        private const string c_downloadUrl = "https://cloud.189.cn/t/yemU3ejaqYBv";
        private INetworkService m_netService;

        public GL_User (INetworkService ns) : base (ns) {
            m_netService = ns;
        }

        public override void Tick (float dT) { }

        public override void NetworkTick () { }

        public void CommandConnect (int netId) {
            m_netService.SendServerCommand (SC_InitSelfNetworkId.Instance (netId, c_version, c_downloadUrl));
        }

        public void CommandDisconnect (int netId) {
            GL_CharacterInit.s_instance.CommandRemoveCharacter (netId);
        }

        public void CommandRegister (int netId, string username, string pwd, string pwdProtectProblem, string pwdProtectAnswer) {
            var playId = EM_User.s_instance.CreateUser (username, pwd, pwdProtectProblem, pwdProtectAnswer);
            if (playId == -1)
                m_netService.SendServerCommand (SC_InitSelfRegister.Instance (netId, false));
            else
                m_netService.SendServerCommand (SC_InitSelfRegister.Instance (netId, true));
        }

        public void CommandLogin (int netId, string username, string pwd) {
            int playerId = EM_User.s_instance.Login (username, pwd);
            if (playerId != -1) {
                // 获取角色列表
                var charList = EM_Character.s_instance.GetAllCharacterByPlayerId (playerId);
                List<NO_LoginCharacter> loginCharNoList = new List<NO_LoginCharacter> (charList.Count);
                foreach (var charDdo in charList) {
                    short lv = EM_Character.s_instance.GetLevelByCharacterId (charDdo.m_characterId);
                    if (lv == -1) continue;
                    loginCharNoList.Add (new NO_LoginCharacter (charDdo.m_characterId, charDdo.m_occupation, charDdo.m_name, lv));
                }
                m_netService.SendServerCommand (SC_InitSelfLogin.Instance (netId, true, playerId, loginCharNoList));
            } else
                m_netService.SendServerCommand (SC_InitSelfLogin.Instance (netId, false, -1, new List<NO_LoginCharacter> ()));
        }
        public void CommandModifyPassword (int netId, string username, string oldPwd, string newPwd) {
            int playerId = EM_User.s_instance.Login (username, oldPwd);
            if (playerId != -1) {
                EM_User.s_instance.ModifyPassword (username, newPwd);
                m_netService.SendServerCommand (SC_InitSelfModifyPassword.Instance (netId, true));
            } else
                m_netService.SendServerCommand (SC_InitSelfModifyPassword.Instance (netId, false));
        }
        public void CommandGetPwdProtectProblem (int netId, string username) {
            string problem = EM_User.s_instance.GetPasswordProtectionProblem (username);
            if (problem != null)
                m_netService.SendServerCommand (SC_InitSelfGetPasswordProtectProblem.Instance (netId, true, problem));
            else
                m_netService.SendServerCommand (SC_InitSelfGetPasswordProtectProblem.Instance (netId, false, string.Empty));
        }
        public void CommandFindPassword (int netId, string username, string pwdProtectAnswer, string newPwd) {
            bool success = EM_User.s_instance.FindPassword (username, pwdProtectAnswer, newPwd);
            if (success)
                m_netService.SendServerCommand (SC_InitSelfModifyPassword.Instance (netId, true));
            else
                m_netService.SendServerCommand (SC_InitSelfModifyPassword.Instance (netId, false));
        }
        public void CommandCreateCharacter (int netId, int playerId, OccupationType ocp, string name) {
            GL_CharacterInit.s_instance.NotifyCreateCharacter (netId, playerId, ocp, name);
        }
    }
}