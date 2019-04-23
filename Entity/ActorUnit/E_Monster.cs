/**
 *Entity, 怪物实体
 *创建者 yuk
 *时间 2019/4/3
 *最后修改者 yuk
 *时间 2019/4/3
 */



using System.Collections.Generic;
namespace MirRemake {
    class E_Monster : E_ActorUnit {
        public E_Monster(int networkId) {
            m_networkId = networkId;
        }
        public override List<E_Item> DropLegacy() {
            return null;
        }
    }
}