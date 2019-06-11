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
        private IDDS_Mission m_misDds;
        private IDDS_Item m_itemDds;
        public GameLogicCharacterCreator (IDDS_Character charDds, IDDS_Skill skillDds, IDDS_Mission misDds, IDDS_Item itemDds, INetworkService ns) {
            m_netService = ns;
            m_charDds = charDds;
            m_skillDds = skillDds;
            m_misDds = misDds;
            m_itemDds = itemDds;
        }
        public void CommandCreateCharacter (int playerId, OccupationType ocp) {
            // TODO: 关联 playerId
            // TODO: 不应当使用用EM, 需要另外设计配置模块
            // 角色 dds
            int charId = m_charDds.CreateCharacter (ocp);
            // 技能 dds
            var skillIdList = EM_Skill.s_instance.GetAllSkillIdListByOccupation (ocp);
            for (int i=0; i<skillIdList.Count; i++)
                m_skillDds.InsertSkill (new DDO_Skill (skillIdList[i], charId, 0, 0));
            // 任务 dds
            var misIdList = EM_Mission.s_instance.GetAllInitUnlockMisDes(ocp);
            for (int i=0; i<misIdList.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (misIdList[i], charId, false, new List<int> ()));
            // 背包和仓库 dds
            short bagSize = 3;
            short storeHouseSize = 6;
            for (short i=0; i<bagSize; i++)
                m_itemDds.InsertItem (E_Item.s_emptyItem.GetItemDdo (charId, ItemPlace.BAG, i));
            for (short i=0; i<storeHouseSize; i++)
                m_itemDds.InsertItem (E_Item.s_emptyItem.GetItemDdo (charId, ItemPlace.STORE_HOUSE, i));
        }
    }
}