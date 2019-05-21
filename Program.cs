using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

using MirRemakeBackend.EntityManager;
using MirRemakeBackend.GameLogic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend {
    class Program {
        private const float c_networkFrameTime = 0.1f;
        static void Main (string[] args) {
            NetworkService networkService = new NetworkService ();
            SM_Skill skillSceneManager = new SM_Skill ();
            SM_ActorUnit.s_instance = new SM_ActorUnit (networkService);
            DateTime time = DateTime.Now;
            float netFrameTimer = 0f;
            while (true) {
                DateTime newTime = DateTime.Now;
                float dT = ((float) ((newTime - time).Ticks / 10000)) / 1000f;
                time = newTime;

                MyTimer.Tick (dT);
                networkService.Tick ();
                SM_ActorUnit.s_instance.Tick (dT);

                netFrameTimer += dT;
                while (netFrameTimer >= c_networkFrameTime) {
                    netFrameTimer -= c_networkFrameTime;
                    SM_ActorUnit.s_instance.NetworkTick ();
                }

                Thread.Sleep (20);
            }
        }
        static void InitDataEntityManager () {
            // TODO: 实现IDataService接口
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
        /// <summary>
        /// 必须在初始化DataEntityManager之后调用
        /// </summary>
        static void InitEntityManager () {
            EM_ActorUnit.s_instance = new EM_ActorUnit ();
        }
    }
}