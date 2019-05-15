using LiteNetLib.Utils;
using UnityEngine;

namespace MirRemake {
    struct NO_SkillParam {
        public Vector2 m_data;
        public NO_SkillParam (Vector2 data) {
            m_data = data;
        }
    }
    struct NO_TargetPosition {
        public bool m_isMovable;
        public int m_targetNetworkId;
        public Vector2 m_targetPosition;
        public NO_TargetPosition (int netId) {
            m_isMovable = true;
            m_targetNetworkId = netId;
            m_targetPosition = Vector2.zero;
        }
        public NO_TargetPosition (Vector2 pos) {
            m_isMovable = false;
            m_targetNetworkId = 0;
            m_targetPosition = pos;
        }
    }
    struct NO_Status {
        public short m_id;
        public int m_value;
        public float m_time;
        public NO_Status (short id, int value, float time) {
            m_id = id;
            m_value = value;
            m_time = time;
        }
    }
}