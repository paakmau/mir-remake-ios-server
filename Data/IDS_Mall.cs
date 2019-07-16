using System;
using LitJson;
using System.Collections.Generic;
using System.IO;
namespace MirRemakeBackend.Data
{
    interface IDS_Mall
    {
        List<DO_MallItemClass> GetAllMallItemClass();
        List<DO_MallItem> GetAllMallItem();
    }
    class DS_MallImpl : IDS_Mall{
        public List<DO_MallItem> GetMallItems(){
            string jsonFile = File.ReadAllText ("Data/D_Mall.json");
            JsonData s_mallData = JsonMapper.ToObject (jsonFile);
            List<DO_MallItem> res=new List<DO_MallItem>();
            for(int i=0;i<s_mallData.Count;i++){
                DO_MallItem item=new DO_MallItem();
                ValueTuple<CurrencyType,int>[] x=new ValueTuple<CurrencyType,int>[2];
                x[0]=new ValueTuple<CurrencyType,int>(CurrencyType.CHARGE,int.Parse(s_mallData[i]["Price"][0].ToString()));
                x[1]=new ValueTuple<CurrencyType,int>(CurrencyType.VIRTUAL,int.Parse(s_mallData[i]["Price"][1].ToString()));
                item.m_itemIdAndPrice=new ValueTuple<short,ValueTuple<CurrencyType,int>[]>
                (short.Parse(s_mallData[i]["ItemId"].ToString()),x);
                res.Add(item);
            }
            return res;
        }
    }
}