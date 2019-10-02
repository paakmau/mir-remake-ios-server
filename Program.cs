using System;
using System.Threading;
using MirRemakeBackend.Data;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
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
            Console.WriteLine ("服务端开始初始化");
            s_networkService = new NetworkService ();
            Console.WriteLine ("初始化实体管理器");
            InitEntityManager ();
            Console.WriteLine ("初始化游戏逻辑");
            InitGameLogic ();
            Console.WriteLine ("初始化完成");

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
            IDDS_CharacterVipCard charVipCardDds = ddsImpl;
            IDDS_Item itemDds = ddsImpl;
            IDDS_Skill skillDds = ddsImpl;
            IDDS_Mission misDds = ddsImpl;
            IDDS_MissionLog misLogDds = ddsImpl;
            IDDS_CombatEfct combatEfctDds = ddsImpl;
            IDDS_Mail mailDds = ddsImpl;
            IDDS_Notice noticeDds = ddsImpl;
            IDDS_Title titleDds = ddsImpl;
            // EntityManager
            EM_BossDamage.s_instance = new EM_BossDamage ();
            EM_Camp.s_instance = new EM_Camp ();
            EM_Character.s_instance = new EM_Character (charDem, charDds, charAttrDds);
            EM_Item.s_instance = new EM_Item (itemDem, itemDds);
            EM_Mail.s_instance = new EM_Mail (mailDds);
            EM_MallItem.s_instance = new EM_MallItem (mallItemDem);
            EM_Mission.s_instance = new EM_Mission (misDem, misDds, titleDds);
            EM_Monster.s_instance = new EM_Monster (monDem);
            EM_MonsterSkill.s_instance = new EM_MonsterSkill ();
            EM_Notice.s_instance = new EM_Notice (noticeDds);
            EM_Rank.s_instance = new EM_Rank (combatEfctDds);
            EM_Sight.s_instance = new EM_Sight ();
            EM_Skill.s_instance = new EM_Skill (skillDem, skillDds);
            EM_Status.s_instance = new EM_Status (statusDem);
            EM_MissionLog.s_instance = new EM_MissionLog (misLogDds);
            EM_Wallet.s_instance = new EM_Wallet (charWalletDds, charVipCardDds);
            // EM init
            EntityManagerInitializer.Init (skillDem, monDem);
        }
        static void InitGameLogic () {
            // GameLogic
            GL_BattleSettle.s_instance = new GL_BattleSettle (s_networkService);
            GL_BossDamage.s_instance = new GL_BossDamage (s_networkService);
            GL_CharacterAction.s_instance = new GL_CharacterAction (s_networkService);
            GL_CharacterAttribute.s_instance = new GL_CharacterAttribute (s_networkService);
            GL_CharacterCombatEfct.s_instance = new GL_CharacterCombatEfct (s_networkService);
            GL_CharacterInit.s_instance = new GL_CharacterInit (s_networkService);
            GL_CharacterSight.s_instance = new GL_CharacterSight (s_networkService);
            GL_Chat.s_instance = new GL_Chat (s_networkService);
            GL_Console.s_instance = new GL_Console (s_networkService);
            GL_Item.s_instance = new GL_Item (s_networkService);
            GL_Mail.s_instance = new GL_Mail (s_networkService);
            GL_Mall.s_instance = new GL_Mall (s_networkService);
            GL_Mission.s_instance = new GL_Mission (s_networkService);
            GL_MissionLog.s_instance = new GL_MissionLog (s_networkService);
            GL_MonsterAction.s_instance = new GL_MonsterAction (s_networkService);
            GL_Notice.s_instance = new GL_Notice (s_networkService);
            GL_Skill.s_instance = new GL_Skill (s_networkService);
            GL_UnitBattleAttribute.s_instance = new GL_UnitBattleAttribute (s_networkService);
            GL_User.s_instance = new GL_User (s_networkService);
            GL_Wallet.s_instance = new GL_Wallet (s_networkService);
            // 放入数组中
            s_gameLogicArr = new GameLogicBase[] {
                GL_BattleSettle.s_instance,
                GL_BossDamage.s_instance,
                GL_CharacterAction.s_instance,
                GL_CharacterAttribute.s_instance,
                // GL_CharacterCombatEfct.s_instance,
                // GL_CharacterInit.s_instance,
                GL_CharacterSight.s_instance,
                // GL_Chat.s_instance,
                // GL_Console.s_instance,
                GL_Item.s_instance,
                GL_Mail.s_instance,
                GL_Mall.s_instance,
                GL_Mission.s_instance,
                GL_MissionLog.s_instance,
                GL_MonsterAction.s_instance,
                GL_Notice.s_instance,
                GL_Skill.s_instance,
                GL_UnitBattleAttribute.s_instance,
                // GL_User.s_instance
                // GL_Wallet.s_instance
            };
        }

        static void TestDynamic () {
            IDS_Character sch = new DS_CharacterImpl ();
            IDS_Item sit = new DS_ItemImpl ();
            IDS_Mission smi = new DS_MissionImpl ();
            IDS_Monster smo = new DS_MonsterImpl ();
            IDS_MonsterMap smm = new DS_MonsterMapImpl ();
            IDS_GroundItemMap sgi = new DS_GroundItemMapImpl ();
            IDS_Skill ssk = new DS_SkillImpl ();
            IDS_Status sst = new DS_StatusImpl ();
            IDS_Mall sma = new DS_MallImpl ();
            DynamicDataServiceImpl dma = new DynamicDataServiceImpl ();
            DDO_MissionLog ml = new DDO_MissionLog ();
            ml.m_charId = 123;
            ml.m_misTarType = MissionTargetType.CHARGE_ADEQUATELY;
            ml.m_parm1 = 1;
            ml.m_parm2 = 2;
            ml.m_parm3 = 3;
            dma.InsertMissionLog (ml);
            ml.m_misTarType = MissionTargetType.TALK_TO_NPC;
            ml.m_parm1 = 13;
            ml.m_parm2 = 22;
            ml.m_parm3 = 55;
            dma.InsertMissionLog (ml);
            ml.m_charId = 1234;
            ml.m_misTarType = MissionTargetType.CHARGE_ADEQUATELY;
            ml.m_parm1 = 1123;
            ml.m_parm2 = 2234;
            ml.m_parm3 = 3345;
            dma.InsertMissionLog (ml);
            System.Collections.Generic.List<DDO_MissionLog> sd = dma.GetMissionLogListByCharacterId (123);
            dma.DeleteMissionLogByCharacterId (123);
            sd = dma.GetMissionLogListByCharacterId (123);
        }
    }
}