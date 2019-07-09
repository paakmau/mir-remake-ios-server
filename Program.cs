using System;
using System.Threading;
using MirRemakeBackend.CharacterCreate;
using MirRemakeBackend.Data;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.GameLogic;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend {
    class Program {
        private const float c_networkFrameTime = 0.1f;
        private static NetworkService s_networkService;
        private static GameLogicBase[] s_gameLogicArr;
        static void Main (string[] args) {
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
            IDS_Item itemDs = new DS_ItemImpl ();
            IDS_GroundItemMap gndItemDs = new DS_GroundItemMapImpl ();
            IDS_Mission misDs = new DS_MissionImpl ();
            // DataEntity
            DEM_Unit actorUnitDem = new DEM_Unit (monsterDs, charDs, mapDs);
            DEM_Status statusDem = new DEM_Status (statusDs);
            DEM_Skill skillDem = new DEM_Skill (skillDs);
            DEM_Item itemDem = new DEM_Item (itemDs, gndItemDs);
            DEM_Mission misDem = new DEM_Mission (misDs);
            // DynamicDataService
            var ddsImpl = new DynamicDataServiceImpl ();
            IDDS_Character charDds = ddsImpl;
            IDDS_CharacterPosition charPosDds = ddsImpl;
            IDDS_Item itemDds = ddsImpl;
            IDDS_Skill skillDds = ddsImpl;
            IDDS_Mission misDds = ddsImpl;
            // EntityManager
            EM_Camp.s_instance = new EM_Camp ();
            EM_Item.s_instance = new EM_Item (itemDem, itemDds);
            EM_Mission.s_instance = new EM_Mission (misDem, misDds);
            EM_MonsterSkill.s_instance = new EM_MonsterSkill (skillDem, actorUnitDem);
            EM_Sight.s_instance = new EM_Sight ();
            EM_Skill.s_instance = new EM_Skill (skillDem, skillDds);
            EM_Status.s_instance = new EM_Status (statusDem);
            EM_Unit.s_instance = new EM_Unit (actorUnitDem, charDds, charPosDds);
            EM_Log.s_instance = new EM_Log ();
            // 角色创建器
            CharacterCreator.s_instance = new CharacterCreator (new DS_SkillImpl (), new DS_MissionImpl (), charDds, charPosDds, skillDds, misDds, itemDds, s_networkService);
        }
        static void InitGameLogic () {
            // GameLogic
            GL_BattleSettle.s_instance = new GL_BattleSettle (s_networkService);
            GL_CharacterAction.s_instance = new GL_CharacterAction (s_networkService);
            GL_CharacterAttribute.s_instance = new GL_CharacterAttribute (s_networkService);
            GL_Item.s_instance = new GL_Item (s_networkService);
            GL_Mission.s_instance = new GL_Mission (s_networkService);
            GL_MonsterAction.s_instance = new GL_MonsterAction (s_networkService);
            GL_CharacterSight.s_instance = new GL_CharacterSight (s_networkService);
            GL_Skill.s_instance = new GL_Skill (s_networkService);
            GL_UnitBattleAttribute.s_instance = new GL_UnitBattleAttribute (s_networkService);
            GL_Log.s_instance = new GL_Log (s_networkService);
            GL_Chat.s_instance = new GL_Chat (s_networkService);
            // 单位初始化器
            UnitInitializer.s_instance = new UnitInitializer ();
            // 放入数组中
            s_gameLogicArr = new GameLogicBase[] {
                GL_BattleSettle.s_instance,
                GL_CharacterAction.s_instance,
                GL_CharacterAttribute.s_instance,
                GL_Item.s_instance,
                GL_Mission.s_instance,
                GL_MonsterAction.s_instance,
                GL_CharacterSight.s_instance,
                GL_Skill.s_instance,
                GL_UnitBattleAttribute.s_instance,
                GL_Log.s_instance,
                GL_Chat.s_instance
            };
        }
    }
}