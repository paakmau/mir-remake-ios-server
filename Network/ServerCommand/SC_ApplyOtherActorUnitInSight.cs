using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    struct SC_ApplyOtherActorUnitInSight : IServerCommand {
        // TODO: 修改为, 当视野变化的时候才会发送, 且只发送新增与离开的单位, 并发送新增单位的初始信息(FSM, HP, MP, Level, Status)
        // TODO: 区分玩家与怪物的接口(因为玩家需要得到装备)
        // TODO: 视野需要有上限
        public NetworkToClientDataType m_DataType { get { return NetworkToClientDataType.APPLY_OTHER_ACTOR_UNIT_IN_SIGHT; } }
        public DeliveryMethod m_DeliveryMethod { get { return DeliveryMethod.Sequenced; } }
        public List<int> m_ToClientList { get; }
        private ActorUnitType m_actorUnitType;
        private List<int> m_otherNetIdList;
        public SC_ApplyOtherActorUnitInSight (List<int> toClientList, ActorUnitType actorUnitType, List<int> otherNetIdList) {
            m_ToClientList = toClientList;
            m_actorUnitType = actorUnitType;
            m_otherNetIdList = otherNetIdList;
        }
        public void PutData (NetDataWriter writer) {
            writer.Put ((byte) m_actorUnitType);
            writer.Put ((byte) m_otherNetIdList.Count);
            for (int i = 0; i < m_otherNetIdList.Count; i++) {
                writer.Put (m_otherNetIdList[i]);
            }
        }
    }
}