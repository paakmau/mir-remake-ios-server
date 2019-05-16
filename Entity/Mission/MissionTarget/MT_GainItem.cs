namespace MirRemakeBackend {
    class MT_GainItem : IMissionTarget {
        public MissionTargetType m_TargetType { get { return MissionTargetType.GAIN_ITEM; } }
        public short m_MissionId { get; }
        public bool m_IsFinished { get { return m_MissionProgressValue >= m_MissionProgressTargetValue; } }
        public short m_MissionProgressTargetValue { get { return m_targetItemNum; } }
        public short m_MissionProgressValue { get { return m_currectItemNum; } }
        private short m_targetItemId;
        private short m_targetItemNum;
        private short m_currectItemNum;
        public MT_GainItem (short missionId, short targetItemId, short targetItemNum, short curItemNum) {
            m_MissionId = missionId;
            m_targetItemId = targetItemId;
            m_targetItemNum = targetItemNum;
            m_currectItemNum = curItemNum;
        }
        public bool GainOrLoseItem (short itemId, short curNum) {
            if (itemId != m_targetItemId)
                return false;
            if (m_currectItemNum == curNum)
                return false;
            m_currectItemNum = curNum;
            return true;
        }
    }
}