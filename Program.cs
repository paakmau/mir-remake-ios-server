using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using MirRemakeBackend.Data;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.GameLogic;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend {
    class Program {
        private const float c_networkFrameTime = 0.1f;
        private static NetworkService s_networkService;
        private static GameLogicBase[] s_gameLogicArr;
        static void Main (string[] args) {
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
            // TODO: 实例化IDataService接口的实现
            IDS_Map mapDs = null;
            IDS_Character charDs = null;
            IDS_Monster monsterDs = null;
            IDS_Status statusDs = null;
            IDS_Skill skillDs = null;
            IDS_Item itemDs = null;
            // 实例化DataEntity
            DEM_ActorUnit actorUnitDem = new DEM_ActorUnit (monsterDs, charDs, mapDs);
            DEM_Status statusDem = new DEM_Status (statusDs);
            DEM_Skill skillDem = new DEM_Skill (skillDs);
            DEM_Item itemDem = new DEM_Item (itemDs);
            // 实例化EntityManager
            EM_ActorUnit.s_instance = new EM_ActorUnit (actorUnitDem);
            EM_Status.s_instance = new EM_Status (statusDem);
            EM_Sight.s_instance = new EM_Sight ();
            EM_Skill.s_instance = new EM_Skill (skillDem);
            EM_MonsterSkill.s_instance = new EM_MonsterSkill (skillDem, actorUnitDem);
            EM_Item.s_instance = new EM_Item (itemDem);
        }
        static void InitGameLogic () {
            // TODO: 实例化IDynamicDataService接口的实现
            IDDS_Character characterDds = null;
            IDDS_Item itemDds = null;
            IDDS_Skill skillDds = null;
            // 实例化GL层
            GL_ActorUnit.s_instance = new GL_ActorUnit (s_networkService);
            GL_BattleSettle.s_instance = new GL_BattleSettle (s_networkService);
            GL_Character.s_instance = new GL_Character (characterDds, s_networkService);
            GL_CharacterAction.s_instance = new GL_CharacterAction (s_networkService);
            GL_Effect.s_instance = new GL_Effect (s_networkService);
            GL_Item.s_instance = new GL_Item (itemDds, s_networkService);
            GL_MonsterAction.s_instance = new GL_MonsterAction (s_networkService);
            GL_Sight.s_instance = new GL_Sight (s_networkService);
            GL_Skill.s_instance = new GL_Skill (skillDds, s_networkService);
            GL_Status.s_instance = new GL_Status (s_networkService);
            // 放入数组中
            s_gameLogicArr = new GameLogicBase[] {
                GL_ActorUnit.s_instance,
                GL_BattleSettle.s_instance,
                GL_Character.s_instance,
                GL_CharacterAction.s_instance,
                GL_Effect.s_instance,
                GL_Item.s_instance,
                GL_MonsterAction.s_instance,
                GL_Sight.s_instance,
                GL_Skill.s_instance,
                GL_Status.s_instance
            };
        }
    }
}