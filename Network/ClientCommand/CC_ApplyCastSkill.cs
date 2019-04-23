using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemake {
    class CC_ApplyCastSkill : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            short skillId = reader.GetShort();
            byte targetCnt = reader.GetByte();
            int[] targetIdArr = new int[targetCnt];
            for(int i=0; i<targetCnt; i++)
                targetIdArr[i] = reader.GetInt();
            NetworkEntityManager.s_instance.CommandApplyCastSkill(netId, skillId, targetIdArr);
        }
    }
}