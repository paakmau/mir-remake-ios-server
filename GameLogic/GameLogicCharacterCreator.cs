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
        private IDDS_Item m_itemDds;
        public GameLogicCharacterCreator (IDDS_Character charDds, IDDS_Skill skillDds, IDDS_Item itemDds, INetworkService ns) {
            m_netService = ns;
        }
        public void CommandCreateCharacter (int playerId, OccupationType ocp) {
            // TODO: 关联 playerId
            // 角色 dds
            int charId = m_charDds.CreateCharacter (ocp);
            // 技能 dds
            var skillIdList = EM_Skill.s_instance.GetSkillIdListByOccupation (ocp);
            for (int i=0; i<skillIdList.Count; i++)
                m_skillDds.InsertSkill (new DDO_Skill (skillIdList[i], charId, 0, 0));
            // 背包和仓库 dds TODO: 写成配置文件
            short bagSize = 20;
            short storeHouseSize = 50;
            for (short i=0; i<bagSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.BAG, i));
            for (short i=0; i<storeHouseSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.STORE_HOUSE, i));
        }
    }
}