using System;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    class CC_ApplyCastSkillBegin : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_BEGIN; } }
        public void Execute(NetDataReader reader, int netId) {
            short skillId = reader.GetShort ();
            NO_SkillParam skillParm = reader.GetSkillParam ();
            Messenger.Broadcast<int, short, NO_SkillParam> ("CommandApplyCastSkillBegin", netId, skillId, skillParm);
        }
    }
}