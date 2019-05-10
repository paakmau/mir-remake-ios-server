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
        public static void PutSkillParam (this NetDataWriter writer, SkillParam value) {
            writer.PutVector2 (value.m_param);
        }
        public static SkillParam GetSkillParam (this NetDataReader reader) {
            return new SkillParam (reader.GetVector2());
        }
        public static void PutE_Status (this NetDataWriter writer, E_Status status) {
            writer.Put (status.m_id);
            writer.Put (status.m_value);
            writer.Put (status.m_DurationTime);
        }
        public static E_Status GetE_Status (this NetDataReader reader) {
            short statusId = reader.GetShort ();
            int value = reader.GetInt ();
            float leftTime = reader.GetFloat ();
            return new E_Status (statusId, value, leftTime);
        }
    }
}