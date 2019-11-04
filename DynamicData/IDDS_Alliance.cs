using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Alliance {
        // 联盟
        public int InsertAlliance (DDO_Alliance ddo);
        public void DeleteAllianceById (int id);

        // 联盟加入申请
        public List<DDO_AllianceApply> GetApplyByAllianceId (int allianceId);
        public bool DeleteApplyById (int id);
        public int InsertApply (DDO_AllianceApply ddo);

        // 成员管理
        public List<DDO_AllianceMember> GetMemberByAllianceId (int allianceId);
        public bool DeleteMemberByCharId (int charId);
        public void InsertMember (DDO_AllianceMember ddo);
        public void UpdateMember (DDO_AllianceMember ddo);
    }
}