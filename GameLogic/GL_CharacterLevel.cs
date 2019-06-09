using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色升级
    /// </summary>
    class GL_CharacterLevel : GameLogicBase {
        public static GL_CharacterLevel s_instance;
        private IDDS_Character m_characterDds;
        public GL_CharacterLevel (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyGainExperience (E_Character charObj, int exp) {
            charObj.m_experience += exp;
            if (charObj.m_UpgradeExperienceInNeed <= charObj.m_experience)
            charObj.m_experience -= charObj.m_UpgradeExperienceInNeed;
            // TODO: 控制等级上限
            // TODO: 处理角色升级
            // IDDS_Character.
        }
    }
}