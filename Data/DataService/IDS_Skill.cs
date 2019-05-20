using System.Collections.Generic;

namespace MirRemakeBackend {
    interface IDS_Skill {
        /// <summary>
        /// 根据技能Id与等级获取技能数据
        /// </summary>
        DO_Skill GetSkillByIdAndLevel (short skillId, short skillLevel);
        /// <summary>
        /// 根据技能Id获取该技能所有等级的数据
        /// </summary>
        DO_Skill[] GetSkillAllLevelById (short skillId);
        /// <summary>
        /// 获取所有技能
        /// 第一维为不同的技能
        /// 第二维为同一技能不同等级
        /// </summary>
        /// <returns></returns>
        DO_Skill[][] GetSkill ();
    }
}