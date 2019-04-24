using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class Program
    {
        static void Main(string[] args)
        {
            NetworkService.s_instance.Init();
            DateTime time = DateTime.Now;
            while(true) {
                DateTime newTime = DateTime.Now;
                float dT = ((float)((newTime-time).Ticks/10000))/100f;
                time = newTime;

                NetworkService.s_instance.Tick(dT);
                NetworkEntityManager.s_instance.Tick(dT);

                Thread.Sleep(20);
            }
        }
    }
}
