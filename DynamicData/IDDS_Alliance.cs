using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Alliance {
        // 联盟
        int InsertAlliance (DDO_Alliance ddo);
        void DeleteAllianceById (int id);
        List<DDO_Alliance> GetAllAlliance ();

        // 联盟加入申请
        List<DDO_AllianceApply> GetApplyByAllianceId (int allianceId);
        bool DeleteApplyById (int id);
        int InsertApply (DDO_AllianceApply ddo);

        // 成员管理
        List<DDO_AllianceMember> GetMemberByAllianceId (int allianceId);
        bool DeleteMemberByCharId (int charId);
        void InsertMember (DDO_AllianceMember ddo);
        void UpdateMember (DDO_AllianceMember ddo);
    }
}