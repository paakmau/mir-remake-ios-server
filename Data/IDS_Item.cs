using System.Collections.Generic;

namespace MirRemakeBackend.Data {
    interface IDS_Item {
        DO_Item[] GetAllItem();
        DO_Equipment[] GetAllEquipment ();
        DO_Consumable[] GetAllConsumable ();
        DO_Gem[] GetAllGem ();
    }
}