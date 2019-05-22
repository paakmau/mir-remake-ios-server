using System.Collections.Generic;

namespace MirRemakeBackend.Data {
    interface IDS_Skill {
        /// <summary>
        /// 获取所有技能
        /// </summary>
        /// <returns></returns>
        DO_Skill[] GetAllSkill ();
    }
}