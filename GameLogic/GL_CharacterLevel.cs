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
        private IDDS_Character m_charDds;
        const int c_maxLevel = 100;
        public GL_CharacterLevel (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_charDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyGainExperience (E_Character charObj, int exp) {
            if (charObj.m_level == c_maxLevel)
                return;
            charObj.m_experience += exp;
            if (charObj.m_UpgradeExperienceInNeed <= charObj.m_experience) {
                charObj.m_experience -= charObj.m_UpgradeExperienceInNeed;
                charObj.m_level ++;
            }
            m_charDds.UpdateCharacter (charObj.GetDdo ());
            m_networkService.SendServerCommand (SC_ApplySelfLevelAndExp.Instance (charObj.m_networkId, charObj.m_level, charObj.m_experience));
        }
    }
}