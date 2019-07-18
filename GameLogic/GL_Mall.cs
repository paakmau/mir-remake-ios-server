using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Mall : GameLogicBase {
        public static GL_Mall s_instance;
        public GL_Mall (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandRequireShoppingMall (int netId) {
            var mallClassEn = EM_MallItem.s_instance.GetMallClassEn ();
            var mallClassNoList = new List<NO_MallClass> (EM_MallItem.s_instance.m_MallClassCnt);
            while (mallClassEn.MoveNext ())
                mallClassNoList.Add (mallClassEn.Current.Value.GetNo ());
            m_networkService.SendServerCommand (SC_SendShoppingMall.Instance (netId, mallClassNoList));
        }
        public void CommandBuyItemInShoppingMall (int netId, int mallItemId, CurrencyType cyType) {
            // TODO: 购买商城物品
        }
    }
}