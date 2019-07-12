namespace MirRemakeBackend.Data
{
    interface I_MissionTargetData {
        short m_Id { get; }
        MissionTargetType m_TargetType { get; }
    }
}