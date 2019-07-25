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
            E_Character charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            E_MallItem itemObj = EM_MallItem.s_instance.GetMallItemByMallItemId (mallItemId);
            if (charObj == null || itemObj == null) return;
            long needCy = cyType == CurrencyType.VIRTUAL ? itemObj.m_VirtualCyPrice : itemObj.m_ChargeCyPrice;
            if (needCy == -1) return;
            needCy *= num;
            long charCy = cyType == CurrencyType.VIRTUAL ? charObj.m_virtualCurrency : charObj.m_chargeCurrency;
            if (charCy < needCy) return;
            // 扣钱
            GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, cyType, -needCy);
            // gain item
            GL_Item.s_instance.NotifyCharacterGainItem (netId, charObj.m_characterId, itemObj.m_ItemId, num);
        }
    }
}