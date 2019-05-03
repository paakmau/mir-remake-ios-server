using System.Collections.Generic;

namespace MirRemake {
    /// <summary>
    /// 打铁NPC
    /// </summary>
    class E_BlacksmithNPC : E_NPC {
        public BuildingEquipmentFortune LookIntoTheMirror(Dictionary<short, short> materials) {
            return BuildingEquipmentFortune.TIGGER;
        }
        
    }
}
