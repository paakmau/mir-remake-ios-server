using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.GameLogic {
    class GL_Mall : GameLogicBase {
        // TODO: 这里都是xjb写的
        public static GL_Mall s_instance;
        List<short> m_allMallItemIdList;
        List<long> m_allMallItemVirtualPriceList;
        public GL_Mall (INetworkService ns) : base (ns) {
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandRequireShoppingMall (int netId) {
            // TODO: 商城信息发送
        }
    }
}