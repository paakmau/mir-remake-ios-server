using System.Collections.Generic;

namespace MirRemakeBackend {
    interface IDS_Skill {
        /// <summary>
        /// 获取所有技能
        /// 第一维为不同的技能
        /// 第二维为同一技能不同等级, 等级有序递增
        /// </summary>
        /// <returns></returns>
        DO_Skill[][] GetAllSkill ();
    }
}