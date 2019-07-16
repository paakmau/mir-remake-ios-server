using System;
using LitJson;
using System.Collections.Generic;
using System.IO;
namespace MirRemakeBackend.Data
{
    interface IDS_Mall
    {

        (List<DO_MallItemClass>,List<DO_MallItem>) GetAllMallItemAndClass();
    }
    class DS_MallImpl : IDS_Mall{
        public (List<DO_MallItemClass>,List<DO_MallItem>) GetAllMallItemAndClass(){
            string jsonFile = File.ReadAllText ("Data/D_Mall.json");
            JsonData s_mallData = JsonMapper.ToObject (jsonFile);
            List<DO_MallItem> res=new List<DO_MallItem>();
            List<DO_MallItemClass> classes=new List<DO_MallItemClass>();
            int len=0;
            for(int i=0;i<s_mallData.Count;i++){
                DO_MallItem item=new DO_MallItem();
                item.m_itemId=short.Parse(s_mallData[i]["ItemId"].ToString());
                item.m_chargeCyPrice=int.Parse(s_mallData[i]["Price"][0].ToString());
                item.m_virtualCyPrice=int.Parse(s_mallData[i]["Price"][1].ToString());
                byte c=byte.Parse(s_mallData[i]["ClassId"].ToString());
                item.m_mallItemClassId=c;
                if(c>len){
                    classes.Add(new DO_MallItemClass(c,s_mallData[i]["ClassName"].ToString()));
                }
                res.Add(item);
            }
            return (classes,res);
        }
    }
}