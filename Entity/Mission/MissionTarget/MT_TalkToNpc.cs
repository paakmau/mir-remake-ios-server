// namespace MirRemake {
//     struct MT_TalkToNpc : IMissionTarget {
//         public MissionTargetType m_TargetType { get { return MissionTargetType.TALK_TO_NPC; } }
//         public bool m_IsFinished {
//             get { return m_isTalked; }
//         }
//         public bool m_IsDirty {
//             get {
//                 if (m_isTalkedPre == m_isTalked)
//                     return false;
//                 m_isTalkedPre = m_isTalked;
//                 return true;
//             }
//         }
//         public short m_MissionProgressTargetValue { get { return 1; } }
//         public short m_MissionProgressValue {
//             get { return m_isTalked ? (short) 1 : (short) 0; }
//             set {
//                 m_isTalked = value == 1;
//             }
//         }
//         private short m_targetNpcId;
//         private short m_missionId;
//         private bool m_isTalkedPre;
//         private bool m_isTalked;
//         public void Init (short targetNpcId, short missionId) {
//             m_targetNpcId = targetNpcId;
//             m_missionId = missionId;
//             m_isTalkedPre = false;
//             m_isTalked = false;
//         }
//     }
// }