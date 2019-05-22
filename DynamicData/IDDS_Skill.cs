using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Skill {
        List<DDO_Skill> GetSkillListByCharacterId (int charId);
        void SetSkillLevelByIdAndCharacterId (short skillLevel, short skillId, int charId);
        void SetSkillMasterlyByIdAndCharacterId (int masterly, short skillId, int charId);
    }
}