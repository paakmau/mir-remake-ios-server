using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Mission {
        List<DDO_Mission> GetMissionListByCharacterId (int charId);
        void InsertMission (DDO_Mission ddo);
        void UpdateMission (DDO_Mission ddo);
        void DeleteMission (short missionId, int charId);
        List<DDO_Mission> GetTitleMissionListByCharacterId (int charId);
        void InsertTitleMission (DDO_Mission ddo);
        void UpdateTitleMission (DDO_Mission ddo);
        void DeleteTitleMission (short missionId, int charId);
    }
}