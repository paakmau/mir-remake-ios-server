using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Mission {
        List<DDO_Mission> GetMissionListByCharacterId (int charId);
        // TODO: DDO_Mission中已经有了charId, 不需要再次传入
        void InsertMission (DDO_Mission ddo, int charId);
        // TODO: DDO_Mission中已经有了charId, 不需要再次传入
        void UpdateMission (DDO_Mission ddo, int charId);
        void DeleteMission (short missionId, int charId);
    }
}