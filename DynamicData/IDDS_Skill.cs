using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Skill {
        List<DDO_Skill> GetSkillListByCharacterId (int charId);
        void UpdateSkill (DDO_Skill ddo, int charId);
    }
}