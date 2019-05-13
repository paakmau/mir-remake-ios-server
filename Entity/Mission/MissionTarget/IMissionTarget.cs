
namespace MirRemake {
    interface IMissionTarget {
        MissionTargetType m_TargetType { get; }
        short m_MissionId { get; }
        bool m_IsFinished { get; }
        short m_MissionProgressTargetValue { get; }
        short m_MissionProgressValue { get; }
    }
}