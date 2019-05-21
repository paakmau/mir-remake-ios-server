using System.Collections.Generic;


namespace MirRemakeBackend {
    class GL_Login {
        private INetworkService m_networkService;
        public GL_Login (INetworkService networkService) {
            m_networkService = networkService;
            Messenger.AddListener<int, int>("NotifyInitCharacterId", NotifyInitCharacterId);
        }
        public void NotifyInitCharacterId (int netId, int charId) {
            E_Character newChar = EM_Character.s_instance.InitCharacter (netId, charId);
            

            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_level, newChar.m_experience, skillIdArr, skillLvArr, skillMasterlyArr));
        }
    }
}