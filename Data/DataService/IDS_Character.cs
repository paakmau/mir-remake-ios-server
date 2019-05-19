using System.Collections.Generic;

namespace MirRemakeBackend {
    interface IDS_Character {
        DO_Character GetCharacterByOccupationAndLevel (OccupationType oc, short level);
    }
}