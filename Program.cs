using System;
using System.Threading;
using MirRemakeBackend.Data;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.EnterGame;
using MirRemakeBackend.Entity;
using MirRemakeBackend.GameLogic;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend {
    class Program {
        private const float c_networkFrameTime = 0.2f;
        private static NetworkService s_networkService;
        private static GameLogicBase[] s_gameLogicArr;
        static void Main (string[] args) {
            //TestStatic();
            //TestDynamic();
            // if (Test() == 1) {
            //     Console.WriteLine("Modle Succeed");

            //     return;
            // }
            s_networkService = new NetworkService ();
            InitEntityManager ();
            InitGameLogic ();

            DateTime time = DateTime.Now;
            float netFrameTimer = 0f;
            while (true) {
                DateTime newTime = DateTime.Now;
                float dT = ((float) ((newTime - time).Ticks / 10000)) / 1000f;
                time = newTime;

                MyTimer.Tick (dT);
                s_networkService.Tick ();
                foreach (var gl in s_gameLogicArr)
                    gl.Tick (dT);

                netFrameTimer += dT;
                while (netFrameTimer >= c_networkFrameTime) {
                    netFrameTimer -= c_networkFrameTime;
                    foreach (var gl in s_gameLogicArr)
                        gl.NetworkTick ();
                }

                Thread.Sleep (20);
            }
        }
        static void InitEntityManager () {
            // DataService
            IDS_MonsterMap mapDs = new DS_MonsterMapImpl ();
            IDS_Character charDs = new DS_CharacterImpl ();
            IDS_Monster monsterDs = new DS_MonsterImpl ();
            IDS_Status statusDs = new DS_StatusImpl ();
            IDS_Skill skillDs = new DS_SkillImpl ();
            IDS_Mall mallDs = new DS_MallImpl ();
            IDS_Item itemDs = new DS_ItemImpl ();
            IDS_GroundItemMap gndItemDs = new DS_GroundItemMapImpl ();
            IDS_Mission misDs = new DS_MissionImpl ();
            // DataEntity
            DEM_Character charDem = new DEM_Character (charDs);
            DEM_Status statusDem = new DEM_Status (statusDs);
            DEM_Skill skillDem = new DEM_Skill (skillDs);
            DEM_MallItem mallItemDem = new DEM_MallItem (mallDs);
            DEM_Mission misDem = new DEM_Mission (misDs);
            DEM_Monster monDem = new DEM_Monster (monsterDs, mapDs);
            DEM_Item itemDem = new DEM_Item (itemDs, gndItemDs);
            // DynamicDataService
            var ddsImpl = new DynamicDataServiceImpl ();
            IDDS_User userDds = ddsImpl;
            IDDS_Character charDds = ddsImpl;
            IDDS_CharacterAttribute charAttrDds = ddsImpl;
            IDDS_CharacterWallet charWalletDds = ddsImpl;
            IDDS_CharacterPosition charPosDds = ddsImpl;
            IDDS_Item itemDds = ddsImpl;
            IDDS_Skill skillDds = ddsImpl;
            IDDS_Mission misDds = ddsImpl;
            IDDS_CombatEfct combatEfctDds = ddsImpl;
            // EntityManager
            EM_BossDamage.s_instance = new EM_BossDamage ();
            EM_Camp.s_instance = new EM_Camp ();
            EM_Character.s_instance = new EM_Character (charDem, charDds, charAttrDds, charWalletDds, charPosDds);
            EM_MallItem.s_instance = new EM_MallItem (mallItemDem);
            EM_Item.s_instance = new EM_Item (itemDem, itemDds);
            EM_Mission.s_instance = new EM_Mission (misDem, misDds);
            EM_Monster.s_instance = new EM_Monster (monDem);
            EM_MonsterSkill.s_instance = new EM_MonsterSkill ();
            EM_Rank.s_instance = new EM_Rank (combatEfctDds);
            EM_Sight.s_instance = new EM_Sight ();
            EM_Skill.s_instance = new EM_Skill (skillDem, skillDds);
            EM_Status.s_instance = new EM_Status (statusDem);
            EM_MissionLog.s_instance = new EM_MissionLog ();
            // EM init
            EntityManagerInitializer.Init (skillDem, monDem);
            // 角色创建器
            User.s_instance = new User (new DS_SkillImpl (), new DS_MissionImpl (), userDds, charDds, charAttrDds, charWalletDds, charPosDds, skillDds, misDds, itemDds, combatEfctDds, s_networkService);

            // TODO: 创建角色
            // User.s_instance.CommandCreateCharacter (1, 1, OccupationType.WARRIOR, "nzynb!");
            // User.s_instance.CommandCreateCharacter (1, 1, OccupationType.WARRIOR, "nzynb!");
            // User.s_instance.CommandCreateCharacter (1, 1, OccupationType.WARRIOR, "nzynb!");
        }
        static void InitGameLogic () {
            // 单位初始化器
            CharacterInitializer.s_instance = new CharacterInitializer ();
            // GameLogic
            GL_BattleSettle.s_instance = new GL_BattleSettle (s_networkService);
            GL_BossDamage.s_instance = new GL_BossDamage (s_networkService);
            GL_CharacterAction.s_instance = new GL_CharacterAction (s_networkService);
            GL_CharacterAttribute.s_instance = new GL_CharacterAttribute (s_networkService);
            GL_CharacterCombatEfct.s_instance = new GL_CharacterCombatEfct (s_networkService);
            GL_Item.s_instance = new GL_Item (s_networkService);
            GL_Mall.s_instance = new GL_Mall (s_networkService);
            GL_Mission.s_instance = new GL_Mission (s_networkService);
            GL_MissionLog.s_instance = new GL_MissionLog (s_networkService);
            GL_MonsterAction.s_instance = new GL_MonsterAction (s_networkService);
            GL_CharacterSight.s_instance = new GL_CharacterSight (s_networkService);
            GL_Skill.s_instance = new GL_Skill (s_networkService);
            GL_UnitBattleAttribute.s_instance = new GL_UnitBattleAttribute (s_networkService);
            GL_Chat.s_instance = new GL_Chat (s_networkService);
            // 放入数组中
            s_gameLogicArr = new GameLogicBase[] {
                GL_BattleSettle.s_instance,
                GL_BossDamage.s_instance,
                GL_CharacterAction.s_instance,
                GL_CharacterAttribute.s_instance,
                GL_Item.s_instance,
                GL_Mall.s_instance,
                GL_Mission.s_instance,
                GL_MissionLog.s_instance,
                GL_MonsterAction.s_instance,
                GL_CharacterSight.s_instance,
                GL_Skill.s_instance,
                GL_UnitBattleAttribute.s_instance,
                GL_Chat.s_instance
            };
        }

        static void TestStatic () {
            IDS_Character sch = new DS_CharacterImpl ();
            IDS_Item sit = new DS_ItemImpl ();
            IDS_Mission smi = new DS_MissionImpl ();
            IDS_Monster smo = new DS_MonsterImpl ();
            IDS_MonsterMap smm = new DS_MonsterMapImpl ();
            IDS_GroundItemMap sgi = new DS_GroundItemMapImpl ();
            IDS_Skill ssk = new DS_SkillImpl ();
            IDS_Status sst = new DS_StatusImpl ();
            IDS_Mall sma = new DS_MallImpl ();
        }
    }
}