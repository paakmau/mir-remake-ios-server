using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;

using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    class GL_Item : GameLogicBase {
        private IDDS_Item m_itemDds;
        public GL_Item (IDDS_Item itemDds, INetworkService netService) : base (netService) {
            m_itemDds = itemDds;
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int, long> ("CommandApplyUseConsumableItem", CommandApplyUseConsumableItem);
            Messenger.AddListener<int, long> ("CommandApplyUseEquipmentItem", CommandApplyUseEquipmentItem);
        }
        public override void Tick(float dT) { }
        public override void NetworkTick() { }
        public void CommandInitCharacterId (int netId, int charId) {
            // TODO: 读取角色bag, storeHouse, equiped道具, 并发送给Client
        }
        public void CommandApplyUseConsumableItem (int netId, long realId) {
            EM_Item.s_instance.GetItemByRealId (realId);
        }
        public void CommandApplyUseEquipmentItem (int netId, long realId) {

        }
    }
}