using System.Collections.Generic;

using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理物品的使用, 存取 (背包, 仓库), 回收
    /// 装备强化, 附魔, 镶嵌
    /// </summary>
    class GL_Item {
        private INetworkService m_networkService;
        public GL_Item (INetworkService netService) {
            m_networkService = netService;

        }
    }
}