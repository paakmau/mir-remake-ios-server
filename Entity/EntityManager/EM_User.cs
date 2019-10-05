using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理用户
    /// </summary>
    class EM_User {
        public static EM_User s_instance;
        IDDS_User m_dds;

        public EM_User (IDDS_User dds) {
            m_dds = dds;
        }

        public int CreateUser (string username, string pwd, string pwdProtectProblem, string pwdProtectAnswer) {
            return m_dds.InsertUser (new DDO_User (-1, username, pwd, pwdProtectProblem, pwdProtectAnswer));
        }

        public int Login (string username, string pwd) {
            DDO_User userDdo;
            var hasUser = m_dds.GetUserByUsername (username, out userDdo);
            return (hasUser && userDdo.m_pwd == pwd) ? userDdo.m_playerId : -1;
        }

        public void ModifyPassword (string username, string newPwd) {
            DDO_User ddo;
            m_dds.GetUserByUsername (username, out ddo);
            ddo.m_pwd = newPwd;
            m_dds.UpdateUser (ddo);
        }

        public string GetPasswordProtectionProblem (string username) {
            DDO_User ddo;
            if (m_dds.GetUserByUsername (username, out ddo))
                return ddo.m_pwdProtectProblem;
            return null;
        }

        public bool FindPassword (string username, string answer, string newPwd) {
            DDO_User userDdo;
            if (m_dds.GetUserByUsername (username, out userDdo))
                if (userDdo.m_pwdProtectAnswer == answer) {
                    userDdo.m_pwd = newPwd;
                    m_dds.UpdateUser (userDdo);
                    return true;
                }
            return false;
        }
    }
}