using System.Collections.Generic;

namespace MirRemakeBackend {
    interface IDS_Item {
        DO_Item GetItemById (short itemId);
        DO_EquipmentInfo GetEquipmentInfoById (short itemId);
        DO_ConsumableInfo GetConsumableInfoById (short itemId);
    }
}