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
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_MallItem itemObj = EM_MallItem.s_instance.GetMallItemByMallItemId (mallItemId);
            if (charObj == null || itemObj == null) return;
            long needCy = cyType == CurrencyType.VIRTUAL ? itemObj.m_VirtualCyPrice : itemObj.m_ChargeCyPrice;
            if (needCy == -1) return;
            long charCy = cyType == CurrencyType.VIRTUAL ? charObj.m_virtualCurrency : charObj.m_chargeCurrency;
            if (charCy < needCy) return;
            // 扣钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, cyType, -needCy);
            // gain item
            GL_Item.s_instance.NotifyCharacterGainItem (netId, charObj.m_characterId, new List < (short, short) > () {
                (itemObj.m_ItemId, 1) });
        }
    }
}