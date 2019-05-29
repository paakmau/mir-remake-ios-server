using System.Collections.Generic;
using System.IO;
using LitJson;
using System;
namespace MirRemakeBackend.Data {
    interface IDS_Mission {
        DO_Mission[] GetAllMission ();
    }
    public class DS_MissionImpl : IDS_Mission {

    }
}