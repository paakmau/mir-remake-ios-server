using System.Collections.Generic;
using MirRemakeBackend.Network;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    class GameLogicCharacterCreator {
        public static GameLogicCharacterCreator s_instance;
        private INetworkService m_netService;
        private IDDS_Character m_charDds;
        private IDDS_Skill m_skillDds;
        public GameLogicCharacterCreator (IDDS_Character charDds, IDDS_Skill skillDds, INetworkService ns) {
            m_netService = ns;
        }
        public void CommandCreateCharacter (OccupationType ocp) {
            int charId = m_charDds.CreateCharacter (ocp);
            var skillIdList = EM_Skill.s_instance.GetSkillIdListByOccupation (ocp);
            for (int i=0; i<skillIdList.Count; i++)
                m_skillDds.InsertSkill (new DDO_Skill (skillIdList[i], charId, 0, 0));
        }
    }
}