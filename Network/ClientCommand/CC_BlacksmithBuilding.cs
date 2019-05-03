using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace MirRemake {
    public class CC_BlacksmithBuilding : IClientCommand {
        public void Execute(NetDataReader reader, int netId) {
            Console.WriteLine("客户端请求打造装备");

            Dictionary<short, short> materials = new Dictionary<short, short>();
            short NPCId = reader.GetShort();
            int count = reader.GetInt();
            for(int i = 0; i < count; i++) {
                short materialId = reader.GetShort();
                short materialNum = reader.GetShort();
                materials.Add(materialId, materialNum);
            }
            SM_ActorUnit.s_instance.CommandBlacksmithBuilding(netId, materials, NPCId);
        }
    }
}