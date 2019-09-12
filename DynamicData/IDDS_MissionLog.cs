using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_MissionLog {
        List<DDO_MissionLog> GetMissionLogListByCharacterId (int charId);
        void DeleteMissionLogByCharacterId (int charId);
        void InsertMissionLog (DDO_MissionLog ddo);
    }
}