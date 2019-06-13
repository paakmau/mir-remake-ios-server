using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.GameLogic {
    class GL_Mall : GameLogicBase {
        // TODO: 这里都是xjb写的
        public static GL_Mall s_instance;
        DEM_Item m_itemDem;
        List<short> m_allMallItemIdList;
        List<long> m_allMallItemVirtualPriceList;
        public GL_Mall (DEM_Item itemDem, IDS_Mall mallDs, INetworkService ns) : base (ns) {
            m_itemDem = itemDem;
            var allMallItem = mallDs.GetMallItems ();
            for (int i=0; i<allMallItem.Count; i++) {
                m_allMallItemIdList.Add (allMallItem[i].m_itemIdAndPrice.Item1);
                foreach (var cy in allMallItem[i].m_itemIdAndPrice.Item2)
                    if (cy.Item1 == CurrencyType.VIRTUAL) {
                        m_allMallItemVirtualPriceList.Add (cy.Item2);
                        break;
                    }
            }
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandApplySellItemInBag (int netId, long realId, short num) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_Repository bag = EM_Item.s_instance.GetBag (netId);
            if (charObj == null || bag == null)
                return;
            short pos;
            E_Item item = bag.GetItemByRealId (realId, out pos);
            if (item == null)
                return;
            var virCy = (int) (item.m_Price * num * 0.8f);
            GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, virCy);
            GL_Property.s_instance.NotifyLostItem (charObj, item, num, pos, bag);
        }
        public void CommandApplyBuyItemIntoBag (int netId, short itemId, short num) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null)
                return;
            DE_Item itemDe = m_itemDem.GetItemById (itemId);
            var virCy = itemDe.m_price * num;
            GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -virCy);
            GL_Property.s_instance.NotifyGainItem (charObj, new List < (short, short) > {
                (itemId, num)
            });
        }
        public void CommandRequireShoppingMallNormal (int netId) {
            m_networkService.SendServerCommand (SC_SendShoppingMallNormal.Instance (netId, m_allMallItemIdList, m_allMallItemVirtualPriceList));
        }
    }
}