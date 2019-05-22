using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.GameLogic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend {
    class Program {
        private const float c_networkFrameTime = 0.1f;
        private static NetworkService s_networkService;
        private static IDDS_Character s_characterDds;
        private static IDDS_Item s_itemDds;
        private static IDDS_Skill s_skillDds;
        private static GameLogicBase[] s_gameLogicArr;
        static void Main (string[] args) {
            s_networkService = new NetworkService ();
            InitDynamicDataService ();
            InitDataEntityManager ();
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
        static void InitDataEntityManager () {
            // TODO: 实例化IDataService接口的实现
            IDS_Map mapDs = null;
            IDS_Character charDs = null;
            IDS_Monster monsterDs = null;
            IDS_Status statusDs = null;
            IDS_Skill skillDs = null;
            IDS_Item itemDs = null;
            DEM_Map.s_instance = new DEM_Map (mapDs);
            DEM_Character.s_instance = new DEM_Character (charDs);
            DEM_Monster.s_instance = new DEM_Monster (monsterDs);
            DEM_Status.s_instance = new DEM_Status (statusDs);
            DEM_Skill.s_instance = new DEM_Skill (skillDs);
            DEM_Item.s_instance = new DEM_Item (itemDs);
        }
        static void InitDynamicDataService () {
            // TODO: 实例化IDynamicDataService接口的实现
            s_characterDds = null;
            s_itemDds = null;
            s_skillDds = null;
        }
        /// <summary>
        /// 须在初始化DataEntityManager之后调用
        /// </summary>
        static void InitEntityManager () {
            EM_ActorUnit.s_instance = new EM_ActorUnit ();
            EM_Sight.s_instance = new EM_Sight ();
            EM_Skill.s_instance = new EM_Skill ();
            EM_Item.s_instance = new EM_Item ();
        }
        static void InitGameLogic () {
            s_gameLogicArr = new GameLogicBase[] {
                new GL_Login (s_characterDds, s_networkService),
                new GL_CharacterAction (s_networkService),
                new GL_MonsterAction (s_networkService),
                new GL_BattleSettle (s_networkService),
                new GL_Item (s_itemDds, s_networkService),
                new GL_Skill (s_networkService),
                new GL_Sight (s_networkService)
            };
        }
    }
}