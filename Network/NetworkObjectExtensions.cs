using LiteNetLib.Utils;
using UnityEngine;


namespace MirRemake {
    static class NetworkObjectExtensions {
        public static void Put (this NetDataWriter writer, Vector2 value) {
            writer.Put (value.x);
            writer.Put (value.y);
        }
        public static Vector2 GetVector2 (this NetDataReader reader) {
            return new Vector2 (reader.GetFloat (), reader.GetFloat ());
        }
        public static void Put (this NetDataWriter writer, NO_SkillParam value) {
            writer.Put (value.m_data);
        }
        public static NO_SkillParam GetSkillParam (this NetDataReader reader) {
            return new NO_SkillParam (reader.GetVector2());
        }
        public static void Put (this NetDataWriter writer, NO_TargetPosition value) {
            writer.Put (value.m_isMovable);
            if (value.m_isMovable)
                writer.Put (value.m_targetNetworkId);
            else
                writer.Put (value.m_targetPosition);
        }
        public static NO_TargetPosition GetTargetPosition (this NetDataReader reader) {
            bool isMovable = reader.GetBool ();
            if (isMovable)
                return new NO_TargetPosition (reader.GetInt ());
            else
                return new NO_TargetPosition (reader.GetVector2 ());
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