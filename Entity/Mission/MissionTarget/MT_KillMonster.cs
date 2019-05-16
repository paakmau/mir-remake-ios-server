namespace MirRemakeBackend {
    struct MT_KillMonster : IMissionTarget {
        public MissionTargetType m_TargetType { get { return MissionTargetType.KILL_MONSTER; } }
        public short m_MissionId { get; }
        public bool m_IsFinished { get { return m_currentMonsterCnt >= m_targetMonsterNum; } }
        public short m_MissionProgressTargetValue { get { return m_targetMonsterNum; } }
        public short m_MissionProgressValue { get { return m_currentMonsterCnt; } }
        private short m_targetMonsterId;
        private short m_targetMonsterNum;
        private short m_currentMonsterCnt;
        public MT_KillMonster (short missionId, short targetMonsterId, short targetMonsterNum, short curMonsterNum) {
            m_MissionId = missionId;
            m_targetMonsterId = targetMonsterId;
            m_targetMonsterNum = targetMonsterNum;
            m_currentMonsterCnt = curMonsterNum;
        }
        public bool KillMonster (short monsterId) {
            if (monsterId != m_targetMonsterId)
                return false;
            m_currentMonsterCnt ++;
            return true;
        }
    }
}