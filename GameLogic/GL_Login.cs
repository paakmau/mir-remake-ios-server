using System.Collections.Generic;

using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Login {
        private IDDS_Character m_characterDds;
        private INetworkService m_networkService;
        public GL_Login (IDDS_Character charDds, INetworkService networkService) {
            m_characterDds = charDds;
            m_networkService = networkService;
            Messenger.AddListener ("CommandAddCharacter", CommandAddCharacter);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, int>("CommandInitCharacterId", CommandInitCharacterId);
        }
        public void CommandAddCharacter () {
            int netId = EM_ActorUnit.s_instance.AssignCharacterNetworkId ();
            m_networkService.AssignNetworkId (netId);
        }
        public void CommandRemoveCharacter (int netId) {
            EM_ActorUnit.s_instance.RemoveCharacterByNetworkId (netId);
            EM_Sight.s_instance.SetUnitInvisible (netId);
        }
        public void CommandInitCharacterId (int netId, int charId) {
            E_Character newChar = EM_ActorUnit.s_instance.InitCharacter (netId, charId, m_characterDds.GetCharacterById (charId));
            EM_Sight.s_instance.SetUnitVisible (newChar);
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