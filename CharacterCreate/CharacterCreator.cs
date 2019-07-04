using System.Collections.Generic;
using MirRemakeBackend.Data;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.CharacterCreate {
    class CharacterCreator {
        public static CharacterCreator s_instance;
        private INetworkService m_netService;
        private IDDS_Character m_charDds;
        private IDDS_Skill m_skillDds;
        private IDDS_Mission m_misDds;
        private IDDS_Item m_itemDds;
        private Dictionary<OccupationType, List<short>> m_ocpSkillIdDict = new Dictionary<OccupationType, List<short>> ();
        private Dictionary<OccupationType, List<short>> m_ocpInitMisIdDict = new Dictionary<OccupationType, List<short>> ();
        public CharacterCreator (IDS_Skill skillDs, IDS_Mission misDs, IDDS_Character charDds, IDDS_Skill skillDds, IDDS_Mission misDds, IDDS_Item itemDds, INetworkService ns) {
            m_netService = ns;
            m_charDds = charDds;
            m_skillDds = skillDds;
            m_misDds = misDds;
            m_itemDds = itemDds;
            // 初始技能加载
            OccupationType[] ocpArr = new OccupationType[] { OccupationType.MAGE, OccupationType.ROGUE, OccupationType.TAOIST, OccupationType.WARRIOR };
            foreach (var ocp in ocpArr) {
                var ocpSkills = skillDs.GetSkillsByOccupation (ocp);
                var skillIdList = new List<short> (ocpSkills.Length);
                foreach (var ocpSkDo in ocpSkills)
                    skillIdList.Add (ocpSkDo.m_skillId);
                m_ocpSkillIdDict.Add (ocp, skillIdList);
            }

            // 初始任务加载
            var allMis = misDs.GetAllMission ();
            foreach (var ocp in ocpArr)
                m_ocpInitMisIdDict.Add (ocp, new List<short> ());
            foreach (var mDo in allMis)
                if (mDo.m_fatherMissionIdArr.Length == 0)
                    foreach (var ocp in ocpArr)
                        if ((ocp | mDo.m_missionOccupation) != 0)
                            m_ocpInitMisIdDict[ocp].Add (mDo.m_id);
        }
        public void CommandCreateCharacter (int playerId, OccupationType ocp) {
            // 角色 dds
            int charId = m_charDds.CreateCharacter (ocp);
            // 技能 dds
            var skillIdList = m_ocpSkillIdDict[ocp];
            for (int i = 0; i < skillIdList.Count; i++)
                m_skillDds.InsertSkill (new DDO_Skill (skillIdList[i], charId, 0, 0));
            // 任务 dds
            var misIdList = m_ocpInitMisIdDict[ocp];
            for (int i = 0; i < misIdList.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (misIdList[i], charId, MissionStatus.UNLOCKED_BUT_UNACCEPTABLE, new List<int> ()));
            // 背包和仓库 dds
            short bagSize = 3;
            short storeHouseSize = 6;
            for (short i = 0; i < bagSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.BAG, i));
            for (short i = 0; i < storeHouseSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.BAG, i));
        }
    }
}