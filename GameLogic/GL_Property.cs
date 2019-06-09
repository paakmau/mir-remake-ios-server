using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Property : GameLogicBase {
        public static GL_Property s_instance;
        private IDDS_Item m_itemDds;
        private IDDS_Character m_charDds;
        public GL_Property (IDDS_Item itemDds, IDDS_Character charDds, INetworkService ns) : base (ns) {
            m_itemDds = itemDds;
            m_charDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyUpdateCurrency (E_Character charObj, CurrencyType type, long dC) {
            charObj.m_currencyDict[type] += dC;
            m_charDds.UpdateCharacter (charObj.GetDdo ());
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                charObj.m_networkId, charObj.m_VirtualCurrency, charObj.m_ChargeCurrency));
        }
        public void NotifyLostItem (E_Character charObj, E_Item item, short num, short pos, E_RepositoryBase repo) {
            // 移除num个该物品
            bool runOut = item.RemoveNum (num);
            long realId = item.m_realId;
            short curNum = item.m_num;
            // 实例 与 数据
            if (runOut) {
                repo.RemoveItemByRealId (item.m_realId);
                m_itemDds.DeleteItemByRealId (item.m_realId);
                EM_Item.s_instance.UnloadItem (item);
            } else
                m_itemDds.UpdateItem (item.GetDdo (charObj.m_characterId, ItemPlace.BAG, pos));
            // Client
            m_networkService.SendServerCommand (SC_ApplySelfUpdateItemNum.Instance (
                new List<int> { charObj.m_networkId }, new List<long> { realId }, new List<short> { curNum }));
        }
        public void NotifySwapItemPlace (E_Character charObj, E_RepositoryBase srcRepo, short srcPos, E_Item srcItem, E_RepositoryBase tarRepo, short tarPos, E_Item tarItem) {
            srcRepo.SetItem (tarItem, srcPos);
            tarRepo.SetItem (srcItem, tarPos);
            m_networkService.SendServerCommand (SC_ApplySelfMoveItem.Instance (
                new List<int> { charObj.m_networkId }, srcRepo.m_repositoryPlace, srcPos, tarRepo.m_repositoryPlace, tarPos));
        }
        public void NotifyGainItem (E_Character charObj, IReadOnlyList < (short, short) > itemIdAndNumList) {
            // m_itemDds.InsertItem
            // EM_Item.s_instance.InitItemList (itemIdAndNumList); 
            // TODO: 处理报酬结算 GL_Property 发送到Client
        }
    }
}