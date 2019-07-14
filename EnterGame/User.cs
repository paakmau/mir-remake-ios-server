using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Data;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.GameLogic;
using MirRemakeBackend.Network;

/// <summary>
/// 处理进入游戏之前的逻辑
/// </summary>
namespace MirRemakeBackend.EnterGame {
    class User {
        public static User s_instance;
        private INetworkService m_netService;
        private IDDS_User m_userDds;
        private IDDS_Character m_charDds;
        private IDDS_CharacterPosition m_charPosDds;
        private IDDS_Skill m_skillDds;
        private IDDS_Mission m_misDds;
        private IDDS_Item m_itemDds;
        private Dictionary<OccupationType, List<short>> m_ocpSkillIdDict = new Dictionary<OccupationType, List<short>> ();
        private Dictionary<OccupationType, List<short>> m_ocpInitMisIdDict = new Dictionary<OccupationType, List<short>> ();
        public User (IDS_Skill skillDs, IDS_Mission misDs, IDDS_User userDds, IDDS_Character charDds, IDDS_CharacterPosition charPosDds, IDDS_Skill skillDds, IDDS_Mission misDds, IDDS_Item itemDds, INetworkService ns) {
            m_netService = ns;
            m_userDds = userDds;
            m_charDds = charDds;
            m_charPosDds = charPosDds;
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
            
            // 出事排行榜加载
        }
        public int AssignNetworkId () {
            return EM_Unit.s_instance.AssignNetworkId ();
        }
        public void CommandConnect (int netId) {
            m_netService.SendServerCommand (SC_InitSelfNetworkId.Instance (netId));
        }
        public void CommandDisconnect (int netId) {
            UnitInitializer.s_instance.CommandRemoveCharacter (netId);
        }
        public void CommandRegister (int netId, string username, string pwd) {
            var playId = m_userDds.InsertUser (new DDO_User (-1, username, pwd));
            if (playId == -1)
                m_netService.SendServerCommand (SC_InitSelfRegister.Instance (netId, false));
            else
                m_netService.SendServerCommand (SC_InitSelfRegister.Instance (netId, true));
        }
        public void CommandLogin (int netId, string username, string pwd) {
            DDO_User userDdo;
            var hasUser = m_userDds.GetUserByUsername (username, out userDdo);
            if (!hasUser || userDdo.m_pwd != pwd)
                m_netService.SendServerCommand (SC_InitSelfLogin.Instance (netId, false, userDdo.m_playerId));
            else
                m_netService.SendServerCommand (SC_InitSelfLogin.Instance (netId, true, userDdo.m_playerId));
        }
        public void CommandCreateCharacter (int netId, int playerId, OccupationType ocp) {
            // 角色 dds
            int charId = m_charDds.CreateCharacter (ocp);
            m_charPosDds.InsertCharacterPosition (new DDO_CharacterPosition (charId, new Vector2 (42, 24)));
            // 技能 dds
            var skillIdList = m_ocpSkillIdDict[ocp];
            for (int i = 0; i < skillIdList.Count; i++)
                m_skillDds.InsertSkill (new DDO_Skill (skillIdList[i], charId, 0, 0));
            // 任务 dds
            var misIdList = m_ocpInitMisIdDict[ocp];
            for (int i = 0; i < misIdList.Count; i++)
                m_misDds.InsertMission (new DDO_Mission (misIdList[i], charId, MissionStatus.ACCEPTABLE, new List<int> ()));
            // 背包, 仓库, equipment dds
            short bagSize = 3;
            short storeHouseSize = 6;
            short eqSize = 10;
            for (short i = 0; i < bagSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.BAG, i));
            for (short i = 0; i < storeHouseSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.BAG, i));
            for (short i = 0; i<eqSize; i++)
                m_itemDds.InsertItem (new DDO_Item (-1, -1, charId, 0, ItemPlace.EQUIPMENT_REGION, i));
            m_netService.SendServerCommand (SC_InitSelfCreateCharacter.Instance (netId, true));
        }
    }
}