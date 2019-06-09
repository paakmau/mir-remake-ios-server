using System.Collections.Generic;
using System;
using System.Data;
namespace MirRemakeBackend.DynamicData {
    interface IDDS_Character {
        /// <summary>
        /// 创建一个角色
        /// </summary>
        /// <returns>角色的id</returns>
        int CreateCharacter (OccupationType occupation);
        DDO_Character GetCharacterById (int characterId);
        void UpdateCharacter (DDO_Character charObj);
    }
}