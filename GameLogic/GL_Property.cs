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
            // TODO: 通知Client
        }
        public void NotifyLostItem (E_Character charObj, E_Item item, short num, int pos, E_RepositoryBase repo) {
            // 移除num个该物品
            bool runOut = item.RemoveNum (num);
            if (runOut) {
                repo.RemoveItemByRealId (item.m_realId);
                m_itemDds.DeleteItemByRealId (item.m_realId);
                EM_Item.s_instance.UnloadItem (item);
            } else
                m_itemDds.UpdateItem (item.GetDdo (charObj.m_characterId, ItemPlace.BAG, pos));
            // TODO: 向客户端发送道具消耗
        }
        public void NotifySwapItemPlace (E_Character charObj, E_RepositoryBase srcRepo, int srcPos, E_Item srcItem, E_RepositoryBase tarRepo, int tarPos, E_Item tarItem) {
            srcRepo.SetItem (tarItem, srcPos);
            tarRepo.SetItem (srcItem, tarPos);
        }
    }
}