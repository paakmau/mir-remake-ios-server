using System;
using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemake {
    class CC_ApplyCastSkill : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端请求释放技能");
            short skillId = reader.GetShort();
            byte targetCnt = reader.GetByte();
            int[] targetIdArr = new int[targetCnt];
            for(int i=0; i<targetCnt; i++)
                targetIdArr[i] = reader.GetInt();
            SM_ActorUnit.s_instance.CommandApplyCastSkill(netId, skillId, targetIdArr);
        }
    }
}