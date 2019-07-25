using System.Collections.Generic;

namespace MirRemakeBackend.DynamicData {
    interface IDDS_Mail {
        List<DDO_Mail> GetAllMailByCharacterId (int charId);
        bool DeleteMailById (int id);
        bool InsertMail (DDO_Mail mail);
    }
}