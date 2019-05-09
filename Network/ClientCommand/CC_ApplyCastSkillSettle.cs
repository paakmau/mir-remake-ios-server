using System;
using System.Collections.Generic;
using LiteNetLib.Utils;


namespace MirRemake {
    class CC_ApplyCastSkillSettle : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_SETTLE; } }
        public void Execute(NetDataReader reader, int netId) {
            short skillId = reader.GetShort();
            byte targetCnt = reader.GetByte();
            int[] targetIdArr = new int[targetCnt];
            for(int i=0; i<targetCnt; i++)
                targetIdArr[i] = reader.GetInt();
            SM_ActorUnit.s_instance.CommandApplyCastSkillSettle(netId, skillId, targetIdArr);
        }
    }
}