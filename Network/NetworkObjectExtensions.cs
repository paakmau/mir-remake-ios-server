using LiteNetLib.Utils;
using UnityEngine;


namespace MirRemakeBackend {
    static class NetworkObjectExtensions {
        public static void Put (this NetDataWriter writer, Vector2 value) {
            writer.Put (value.x);
            writer.Put (value.y);
        }
        public static Vector2 GetVector2 (this NetDataReader reader) {
            return new Vector2 (reader.GetFloat (), reader.GetFloat ());
        }
        public static void Put (this NetDataWriter writer, NO_SkillParam value) {
            writer.Put (value.m_targetNetworkId);
            writer.Put (value.m_direction);
            writer.Put (value.m_position);
        }
        public static NO_SkillParam GetSkillParam (this NetDataReader reader) {
            return new NO_SkillParam (reader.GetInt (), reader.GetVector2 (), reader.GetVector2 ());
        }
        public static void Put (this NetDataWriter writer, NO_Status status) {
            writer.Put (status.m_id);
            writer.Put (status.m_value);
            writer.Put (status.m_time);
        }
        public static NO_Status GetStatus (this NetDataReader reader) {
            short statusId = reader.GetShort ();
            int value = reader.GetInt ();
            float time = reader.GetFloat ();
            return new NO_Status (statusId, value, time);
        }
    }
}