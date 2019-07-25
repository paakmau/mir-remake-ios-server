using System.Collections.Generic;
using System;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Mail {
        List<DDO_Mail> GetAllMailByReceiverCharacterId (int charId);
        bool DeleteMailById (int id);
        bool InsertMail (DDO_Mail mail);
        bool UpdateMailRead (int id, int isRead);
        bool UpdateMailReceived (int id, int isReceived);
        void DeleteMailBeforeCertainTime(DateTime time);
    }
}
