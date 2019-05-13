// namespace MirRemake {
//     struct MT_LevelUpSkill : IMissionTarget {
//         public MissionTargetType m_TargetType { get { return MissionTargetType.LEVEL_UP_SKILL; } }
//         public bool m_IsFinished {
//             get {
//                 return SM_Skill.s_instance.GetSkillLevelById (m_targetSkillId) >= m_targetSkillLevel;
//             }
//         }
//         public bool m_IsDirty {
//             get {
//                 var curLevel = SM_Skill.s_instance.GetSkillLevelById (m_targetSkillId);
//                 if (m_skillLevelPre == curLevel)
//                     return false;
//                 m_skillLevelPre = curLevel;
//                 return true;
//             }
//         }
//         public short m_MissionProgressTargetValue { get { return m_targetSkillLevel; } }
//         public short m_MissionProgressValue { get { return SM_Skill.s_instance.GetSkillLevelById (m_targetSkillLevel); } set { } }
//         private short m_targetSkillId;
//         private short m_targetSkillLevel;
//         private short m_skillLevelPre;
//         public void Init (short targetSkillId, short targetLevelNum) {
//             m_targetSkillId = targetSkillId;
//             m_targetSkillLevel = targetLevelNum;
//             m_skillLevelPre = SM_Skill.s_instance.GetSkillLevelById (targetSkillId);
//         }
//     }
// }