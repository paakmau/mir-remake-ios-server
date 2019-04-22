using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace MirRemake {
    static class Extension {
        public static void PutVector2 (this NetDataWriter writer, Vector2 value) {
            writer.Put (value.x);
            writer.Put (value.y);
        }
        public static Vector2 GetVector2 (this NetDataReader reader) {
            return new Vector2 (reader.GetFloat (), reader.GetFloat ());
        }
        public static void PutHPAndMP (this NetDataWriter writer, Dictionary<ActorUnitConcreteAttributeType, int> value) {
            writer.Put (value[ActorUnitConcreteAttributeType.MAX_HP]);
            writer.Put (value[ActorUnitConcreteAttributeType.MAX_MP]);
            writer.Put (value[ActorUnitConcreteAttributeType.CURRENT_HP]);
            writer.Put (value[ActorUnitConcreteAttributeType.CURRENT_MP]);
        }
        public static KeyValuePair<ActorUnitConcreteAttributeType, int>[] GetHPAndMP (this NetDataReader reader) {
            int otherConcreteAttrSize = 4;
            KeyValuePair<ActorUnitConcreteAttributeType, int>[] res = new KeyValuePair<ActorUnitConcreteAttributeType, int>[otherConcreteAttrSize];
            res[0] = new KeyValuePair<ActorUnitConcreteAttributeType, int> (ActorUnitConcreteAttributeType.MAX_HP, reader.GetInt ());
            res[1] = new KeyValuePair<ActorUnitConcreteAttributeType, int> (ActorUnitConcreteAttributeType.MAX_MP, reader.GetInt ());
            res[2] = new KeyValuePair<ActorUnitConcreteAttributeType, int> (ActorUnitConcreteAttributeType.CURRENT_HP, reader.GetInt ());
            res[3] = new KeyValuePair<ActorUnitConcreteAttributeType, int> (ActorUnitConcreteAttributeType.CURRENT_MP, reader.GetInt ());
            return res;
        }
        public static TargetPosition GetTargetPosition(this NetDataReader reader) {
            int networkId = reader.GetInt();
            Vector2 pos = reader.GetVector2();
            return new TargetPosition(NetworkService.s_instance.GetActorUnitByNetworkId(networkId), pos);
        }
        public static void PutE_Status (this NetDataWriter writer, E_Status status) {
            writer.Put(status.m_id);
            writer.Put(status.m_value);
            writer.Put(status.m_leftTime);
        }
        public static E_Status GetE_Status (this NetDataReader reader) {
            short statusId = reader.GetShort();
            int value = reader.GetInt();
            float leftTime = reader.GetFloat();
            return new E_Status(statusId, value, leftTime);
        }
    }
}