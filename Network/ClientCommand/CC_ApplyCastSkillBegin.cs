using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemake {
    class CC_ApplyCastSkillBegin : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_BEGIN; } }
        public void Execute(NetDataReader reader, int netId) {
            short skillId = reader.GetShort ();
            NO_TargetPosition tarPos = reader.GetTargetPosition ();
            NO_SkillParam skillParm = reader.GetSkillParam ();
            SM_ActorUnit.s_instance.CommandApplyCastSkillBegin (netId, skillId, new TargetPosition (tarPos), new SkillParam (skillParm));
        }
    }
}