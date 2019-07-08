using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理地面物品刷新
    /// </summary>
    class GL_MonsterItem : GameLogicBase {
        public static GL_MonsterItem s_instance;
        public GL_MonsterItem (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyDropLegacy (E_Monster monObj) {
            var monDropList = monObj.m_DropItemIdList;
            List < (short, short) > dropItemIdAndNumList = new List < (short, short) > ();
            // TODO: 根据 monDropList 随机获取掉落的物品


            var itemList = EM_Item.s_instance.GenerateItemOnGround (dropItemIdAndNumList);

            // TODO: Client
        }
    }
}