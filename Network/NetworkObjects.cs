using System.Collections.Generic;
using System.Numerics;
using LiteNetLib.Utils;

namespace MirRemakeBackend.Network {
    struct NO_SkillParam {
        public int m_targetNetworkId;
        public Vector2 m_direction;
        public Vector2 m_position;
        public NO_SkillParam (int targetNetId, Vector2 direction, Vector2 position) {
            m_targetNetworkId = targetNetId;
            m_direction = direction;
            m_position = position;
        }
    }
    struct NO_Status {
        public short m_id;
        public float m_value;
        public float m_time;
        public NO_Status (short id, float value, float time) {
            m_id = id;
            m_value = value;
            m_time = time;
        }
    }
    struct NO_Effect {
        public short m_animId;
        public bool m_hit;
        public bool m_isCritical;
        public int m_deltaHp;
        public int m_deltaMp;
        public NO_Effect (short animId, bool hit, bool isCritical, int dHp, int dMp) {
            m_animId = animId;
            m_hit = hit;
            m_isCritical = isCritical;
            m_deltaHp = dHp;
            m_deltaMp = dMp;
        }
    }
    struct NO_Monster {
        public int m_netId;
        public Vector2 m_position;
        public short m_monsterId;
        public NO_Monster (int netId, Vector2 pos, short monId) {
            m_netId = netId;
            m_position = pos;
            m_monsterId = monId;
        }
    }
    struct NO_Character {
        public int m_netId;
        public Vector2 m_position;
        public OccupationType m_occupation;
        public short m_level;
        public NO_Character (int netId, Vector2 pos, OccupationType ocp, short lv) {
            m_netId = netId;
            m_position = pos;
            m_occupation = ocp;
            m_level = lv;
        }
    }
    static class NetworkObjectExtensions {
        public static void Put (this NetDataWriter writer, Vector2 value) {
            writer.Put (value.X);
            writer.Put (value.Y);
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
        public static void Put (this NetDataWriter writer, NO_Effect effect) {
            writer.Put (effect.m_animId);
            writer.Put (effect.m_hit);
            writer.Put (effect.m_isCritical);
            writer.Put (effect.m_deltaHp);
            writer.Put (effect.m_deltaMp);
        }
        public static NO_Effect GetEffect (this NetDataReader reader) {
            short animId = reader.GetShort ();
            bool isHit = reader.GetBool ();
            bool isCritical = reader.GetBool ();
            int dHp = reader.GetInt ();
            int dMp = reader.GetInt ();
            return new NO_Effect (animId, isHit, isCritical, dHp, dMp);
        }
        public static void Put (this NetDataWriter writer, NO_Monster monNo) {
            writer.Put (monNo.m_netId);
            writer.Put (monNo.m_position);
            writer.Put (monNo.m_monsterId);
        }
        public static NO_Monster GetMonster (this NetDataReader reader) {
            int netId = reader.GetInt ();
            Vector2 pos = reader.GetVector2 ();
            short monsterId = reader.GetShort ();
            return new NO_Monster (netId, pos, monsterId);
        }
        public static void Put (this NetDataWriter writer, NO_Character charNo) {
            writer.Put (charNo.m_netId);
            writer.Put (charNo.m_position);
            writer.Put ((byte) charNo.m_occupation);
            writer.Put (charNo.m_level);
        }
        public static NO_Character GetCharacter (this NetDataReader reader) {
            int netId = reader.GetInt ();
            Vector2 pos = reader.GetVector2 ();
            OccupationType ocp = (OccupationType) reader.GetByte ();
            short lv = reader.GetShort ();
            short equipNum = reader.GetByte ();
            return new NO_Character (netId, pos, ocp, lv);
        }
    }
}