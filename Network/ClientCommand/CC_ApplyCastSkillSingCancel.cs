using System;
using UnityEngine;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_ApplyCastSkillSingCancel : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_SING_CANCEL; } }
        public void Execute(NetDataReader reader, int netId) {
            SM_ActorUnit.s_instance.CommandApplyCastSkillSingCancel (netId);
        }
    }
}