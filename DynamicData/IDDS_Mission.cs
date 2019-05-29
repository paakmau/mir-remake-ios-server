using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Mission {
        List<DDO_Mission> GetMissionListByCharacterId (int charId);
        void InsertMission (DDO_Mission ddo, int charId);
        void UpdateMission (DDO_Mission ddo, int charId);
        void DeleteMission (short missionId, int charId);
    }
}