using System.Collections.Generic;
using System;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Mail {
        List<DDO_Mail> GetAllMailByReceiverCharacterId (int charId);
        bool DeleteMailById (int id);
        int InsertMail (DDO_Mail mail);
        bool UpdateMailRead (int id, bool isRead);
        bool UpdateMailReceived (int id, bool isReceived);
        void DeleteMailBeforeCertainTime(DateTime time);
    }
}
