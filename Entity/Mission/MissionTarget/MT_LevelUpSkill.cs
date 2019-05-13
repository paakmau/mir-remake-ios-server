namespace MirRemake {
    struct MT_LevelUpSkill : IMissionTarget {
        public MissionTargetType m_TargetType { get { return MissionTargetType.LEVEL_UP_SKILL; } }
        public short m_MissionId { get; }
        public bool m_IsFinished { get { return m_currentSkillLevel >= m_targetSkillLevel; } }
        public short m_MissionProgressTargetValue { get { return 1; } }
        public short m_MissionProgressValue { get { return m_currentSkillLevel >= m_targetSkillLevel ? (short)1 : (short)0; } }
        private short m_targetSkillId;
        private short m_targetSkillLevel;
        private short m_currentSkillLevel;
        public MT_LevelUpSkill (short missionId, short targetSkillId, short targetLv, short curLv) {
            m_MissionId = missionId;
            m_targetSkillId = targetSkillId;
            m_targetSkillLevel = targetLv;
            m_currentSkillLevel = curLv;
        }
        public bool UpdateSkillLevel (short skillId, short curLv) {
            if (m_targetSkillId != skillId) return false;
            if (m_currentSkillLevel == curLv) return false;
            m_currentSkillLevel = curLv;
            return true;
        }
    }
}