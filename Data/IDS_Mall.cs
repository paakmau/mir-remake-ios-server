using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
namespace MirRemakeBackend.Data {
    interface IDS_Mall {
        (List<DO_MallItemClass>, List<DO_MallItem>) GetAllMallItemAndClass ();
    }
    class DS_MallImpl : IDS_Mall {
        public (List<DO_MallItemClass>, List<DO_MallItem>) GetAllMallItemAndClass () {
            string jsonFile = File.ReadAllText ("Data/D_Mall.json");
            JsonData s_mallData = JsonMapper.ToObject (jsonFile);
            jsonFile = File.ReadAllText("Data/D_MallClass.json");
            JsonData s_mallClasses=JsonMapper.ToObject(jsonFile);
            List<DO_MallItem> res = new List<DO_MallItem> ();
            List<DO_MallItemClass> classes = new List<DO_MallItemClass> ();
            for (int i = 0; i < s_mallData.Count; i++) {
                DO_MallItem item = new DO_MallItem ();
                item.m_itemId = short.Parse (s_mallData[i]["ItemId"].ToString ());
                item.m_chargeCyPrice = int.Parse (s_mallData[i]["Price"][0].ToString ());
                item.m_virtualCyPrice = int.Parse (s_mallData[i]["Price"][1].ToString ());
                byte c = byte.Parse (s_mallData[i]["ClassId"].ToString ());
                res.Add (item);
            }
            for(int i=0;i<s_mallClasses.Count;i++){
                classes.Add(new DO_MallItemClass(byte.Parse(s_mallClasses[i]["ClassId"].ToString()),s_mallClasses[i]["ClassName"].ToString()));
            }
            return (classes, res);
        }
    }
}