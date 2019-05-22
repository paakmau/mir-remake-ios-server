using System.Collections.Generic;

namespace MirRemakeBackend.Data {
    interface IDS_Character {
        /// <summary>
        /// 返回所有角色  
        /// 第一维是不同的角色  
        /// 第二维是不同的等级, 等级有序递增  
        /// </summary>
        /// <returns></returns>
        DO_Character[][] GetAllCharacter ();
    }
}