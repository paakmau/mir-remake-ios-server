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
        public static void PutFSMAEState (this NetDataWriter writer, FSMActiveEnterState state) {
            writer.Put ((byte) state.m_type);
            switch (state.m_type) {
                case FSMStateType.CAST_BEGIN:
                    writer.Put (state.m_data, 0, 18);
                    break;
                case FSMStateType.CAST_SING_CANCEL:
                    break;
            }
        }
        public static FSMActiveEnterState GetFSMAEState (this NetDataReader reader) {
            FSMActiveEnterState res = new FSMActiveEnterState ();
            res.m_type = (FSMStateType) reader.GetByte ();
            res.m_data = new byte[30];
            switch (res.m_type) {
                case FSMStateType.CAST_BEGIN:
                    reader.GetBytes (res.m_data, 18);
                    break;
                case FSMStateType.CAST_SING_CANCEL:
                    break;
            }
            return res;
        }
        public static void PutE_Status (this NetDataWriter writer, E_Status status) {
            writer.Put (status.m_id);
            writer.Put (status.m_value);
            writer.Put (status.m_leftTime);
        }
        public static E_Status GetE_Status (this NetDataReader reader) {
            short statusId = reader.GetShort ();
            int value = reader.GetInt ();
            float leftTime = reader.GetFloat ();
            return new E_Status (statusId, value, leftTime);
        }
    }
}