using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

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
    }
}