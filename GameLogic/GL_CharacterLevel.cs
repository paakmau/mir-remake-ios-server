using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色升级
    /// 以及具体属性等
    /// </summary>
    class GL_CharacterLevel : GameLogicBase {
        public static GL_Character s_instance;
        private IDDS_Character m_characterDds;
        public GL_CharacterLevel (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_characterDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
    }
}