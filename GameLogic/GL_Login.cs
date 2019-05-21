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
            E_Skill[] skillArr = EM_Skill.s_instance.InitCharacterSkill (netId, charId);
            short[] skillIdArr = new short[skillArr.Length];
            short[] skillLvArr = new short[skillArr.Length];
            int[] skillMasterlyArr = new int[skillArr.Length];
            for (int i=0; i<skillArr.Length; i++) {
                skillIdArr[i] = skillArr[i].m_id;
                skillLvArr[i] = skillArr[i].m_level;
                skillMasterlyArr[i] = skillArr[i].m_masterly;
            }

            m_networkService.SendServerCommand (new SC_InitSelfInfo (new List<int> { netId }, newChar.m_level, newChar.m_experience, skillIdArr, skillLvArr, skillMasterlyArr));
        }
    }
}