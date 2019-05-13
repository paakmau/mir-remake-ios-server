namespace MirRemake {
    struct MT_TalkToNpc : IMissionTarget {
        public MissionTargetType m_TargetType { get { return MissionTargetType.TALK_TO_NPC; } }
        public short m_MissionId { get; }
        public bool m_IsFinished { get { return m_isTalked; } }
        public short m_MissionProgressTargetValue { get { return 1; } }
        public short m_MissionProgressValue { get { return m_isTalked ? (short) 1 : (short) 0; } }
        private short m_targetNpcId;
        private bool m_isTalked;
        public MT_TalkToNpc (short missionId, short targetNpcId, bool isTalked) {
            m_MissionId = missionId;
            m_targetNpcId = targetNpcId;
            m_isTalked = isTalked;
        }
        public bool TalkToNpc (short npcId, short missionId) {
            if (npcId != m_targetNpcId) return false;
            if (missionId != m_MissionId) return false;
            m_isTalked = true;
            return true;
        }
    }
}