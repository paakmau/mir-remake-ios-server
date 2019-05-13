// namespace MirRemake {
//     struct MT_KillMonster : IMissionTarget {
//         public MissionTargetType m_TargetType { get { return MissionTargetType.KILL_MONSTER; } }
//         public bool m_IsFinished {
//             get {
//                 return m_currentMonsterCnt >= m_targetMonsterNum;
//             }
//         }
//         public bool m_IsDirty {
//             get {
//                 if (m_currentMonsterCntPre == m_currentMonsterCnt)
//                     return false;
//                 m_currentMonsterCntPre = m_currentMonsterCnt;
//                 return true;
//             }
//         }
//         public short m_MissionProgressTargetValue { get { return m_targetMonsterNum; } }
//         public short m_MissionProgressValue { get { return m_currentMonsterCnt; } set { m_currentMonsterCnt = value; } }
//         private short m_targetMonsterId;
//         private short m_targetMonsterNum;
//         private short m_currentMonsterCnt;
//         private short m_currentMonsterCntPre;
//         public void Init (short targetMonsterId, short targetMonsterNum) {
//             m_targetMonsterId = targetMonsterId;
//             m_targetMonsterNum = targetMonsterNum;
//             m_currentMonsterCnt = 0;
//             m_currentMonsterCntPre = 0;
//         }
//     }
// }