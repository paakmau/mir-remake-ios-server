using System;
using System.Collections.Generic;
namespace MirRemakeBackend.DynamicData {
    interface IDDS_Notice {
        List<DDO_Notice> GetAllNotice ();
        int InsertNotice (DDO_Notice notice);
        bool DeleteNoticeById (int id);
        void DeleteNoticeBeforeCertainTime (DateTime time);
    }
}