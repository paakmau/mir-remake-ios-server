using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Mall : GameLogicBase {
        public static GL_Mall s_instance;
        public GL_Mall (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandRequireMall (int netId) {
            var mallClassEn = EM_MallItem.s_instance.GetMallClassEn ();
            var mallClassNoList = new List<NO_MallClass> (EM_MallItem.s_instance.m_MallClassCnt);
            while (mallClassEn.MoveNext ())
                mallClassNoList.Add (mallClassEn.Current.Value.GetNo ());
            m_networkService.SendServerCommand (SC_ApplySelfShowMall.Instance (netId, mallClassNoList));
        }
        public void CommandBuyItemInMall (int netId, int mallItemId, short num, CurrencyType cyType) {
            if (num == 0) return;
            int charId = EM_Character.s_instance.GetCharIdByNetId (netId);
            E_MallItem itemObj = EM_MallItem.s_instance.GetMallItemByMallItemId (mallItemId);
            var wallet = EM_Wallet.s_instance.GetWallet (netId);
            var bag = EM_Item.s_instance.GetBag (netId);
            if (charId == -1 || itemObj == null || wallet.Item1 == -1 || bag == null) return;
            long needCy = cyType == CurrencyType.VIRTUAL ? itemObj.m_VirtualCyPrice : itemObj.m_ChargeCyPrice;
            if (needCy == -1) {
                GL_Chat.s_instance.NotifyBuyItemCyErrSendMessage (netId);
                return;
            }
            needCy *= num;
            long charCy = cyType == CurrencyType.VIRTUAL ? wallet.Item1 : wallet.Item2;
            if (charCy < needCy) {
                GL_Chat.s_instance.NotifyBuyItemShortOfCySendMessage (netId);
                return;
            }
            // 扣钱
            GL_Wallet.s_instance.NotifyUpdateCurrencyOnline (netId, charId, cyType, -needCy);
            // 物品
            if (!bag.CanPutItem (itemObj.m_ItemId, num)) {
                GL_Chat.s_instance.NotifyBuyItemBagFullSendToMailBoxSendMessage (netId);
                GL_Mail.s_instance.NotifySendMallItem (netId, charId, new (short, short) [] {
                    (itemObj.m_ItemId, num)
                });
            } else
                GL_Item.s_instance.NotifyCharacterGainItem (netId, charId, bag, itemObj.m_ItemId, num);
        }
    }
}