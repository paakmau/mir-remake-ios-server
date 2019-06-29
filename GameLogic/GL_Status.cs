using System;
using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理单位战斗相关属性与状态
    /// </summary>
    class GL_Status : GameLogicBase {
        public static GL_Status s_instance;
        public GL_Status (INetworkService netService) : base (netService) { }
        private float m_secondTimer = 0;
        public override void Tick (float dT) {
        }
        public override void NetworkTick () {
        }
    }
}