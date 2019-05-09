using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class Program {
        private const float c_networkFrameTime = 0.1f;
        private static NetworkService s_networkService;
        static void Main (string[] args) {
            s_networkService = new NetworkService ();
            SM_ActorUnit.s_instance.Init (s_networkService);
            DateTime time = DateTime.Now;
            float netFrameTimer = 0f;
            while (true) {
                DateTime newTime = DateTime.Now;
                float dT = ((float) ((newTime - time).Ticks / 10000)) / 1000f;
                time = newTime;

                MyTimer.Tick (dT);
                s_networkService.Tick ();
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