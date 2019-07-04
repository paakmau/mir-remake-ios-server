using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace MirRemakeBackend.Data {
    interface IDS_Status {
        DO_Status[] GetAllChangeHpStatus ();
        DO_Status[] GetAllChangeMpStatus ();
        (DO_Status, DO_ConcreteAttributeStatus) [] GetAllConcreteAttributeStatus ();
        (DO_Status, DO_SpecialAttributeStatus) [] GetAllSpecialAttributeStatus ();
    }
    class DS_StatusImpl : IDS_Status {
        private JsonData s_statusDatas;
        private DO_Status[] res;
        private DO_SpecialAttributeStatus[] specialAttributes;
        private DO_ConcreteAttributeStatus[] concreteAttributes;
        public void LoadAllStatus () {
            string jsonFile = File.ReadAllText ("Data/D_Status.json");
            s_statusDatas = JsonMapper.ToObject (jsonFile);
            res = new DO_Status[s_statusDatas.Count];
            specialAttributes=new DO_SpecialAttributeStatus[s_statusDatas.Count];
            concreteAttributes=new DO_ConcreteAttributeStatus[s_statusDatas.Count];
            for (int i = 0; i < res.Length; i++) {
                DO_Status status = new DO_Status ();
                DO_SpecialAttributeStatus s_status=new DO_SpecialAttributeStatus();
                DO_ConcreteAttributeStatus c_status=new DO_ConcreteAttributeStatus();
                status.m_statusId = short.Parse (s_statusDatas[i]["StatusID"].ToString ());
                status.m_type = (StatusType) Enum.Parse (typeof (StatusType), s_statusDatas[i]["Type"].ToString ());
                status.m_isBuff = int.Parse(s_statusDatas[i]["IsBuff"].ToString())==1?true:false;
                if(s_statusDatas[i]["SpecialAttributes"].Count!=0)
                    s_status.m_spAttr = (ActorUnitSpecialAttributeType) Enum.Parse (typeof (ActorUnitSpecialAttributeType), s_statusDatas[i]["SpecialAttributes"][0].ToString ());
                c_status.m_conAttrArr=new ValueTuple<ActorUnitConcreteAttributeType, int>[s_statusDatas[i]["AttributeAttachArray"].Count];
                for (int m = 0; m < s_statusDatas[i]["AttributeAttachArray"].Count; m++) {
                    c_status.m_conAttrArr[m] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType) Enum.Parse (typeof (ActorUnitConcreteAttributeType), s_statusDatas[i]["AttributeAttachArray"][m].ToString ().Split (' ') [0]),
                            int.Parse (s_statusDatas[i]["AttributeAttachArray"][m].ToString ().Split (' ') [1]));
                }
                
                res[i] = status;
                specialAttributes[i]=s_status;
                concreteAttributes[i]=c_status;
            }
        }

        public DO_Status[] GetAllChangeHpStatus(){
            LoadAllStatus();
            DO_Status[] x=new DO_Status[3];
            x[0]=res[0];x[1]=res[4];x[2]=res[5];
            return x;
        }
        public DO_Status[] GetAllChangeMpStatus(){
            LoadAllStatus();
            DO_Status[] x=new DO_Status[1];
            x[0]=res[1];
            return x;
        }
        public (DO_Status, DO_ConcreteAttributeStatus) [] GetAllConcreteAttributeStatus (){
            LoadAllStatus();
            ValueTuple<DO_Status,DO_ConcreteAttributeStatus>[] x=new ValueTuple<DO_Status,DO_ConcreteAttributeStatus>[19];
            for(int i=3;i<27;i++){
                if(i==5||i==6||(i>=19 &&i<=22)){
                    continue;
                }
                x[i]=new ValueTuple<DO_Status,DO_ConcreteAttributeStatus>(res[i],concreteAttributes[i]);
            }
            return x;
        }
        public (DO_Status,DO_SpecialAttributeStatus)[] GetAllSpecialAttributeStatus(){
            LoadAllStatus();
            ValueTuple<DO_Status,DO_SpecialAttributeStatus>[] x=new ValueTuple<DO_Status,DO_SpecialAttributeStatus>[4];
            for(int i=19;i<23;i++){
                x[19-i]=new ValueTuple<DO_Status,DO_SpecialAttributeStatus>(res[i],specialAttributes[i]);
            }
            return x;
        }
    }
}