using System.Collections.Generic;
using System;
using LitJson;
using System.IO;

namespace MirRemakeBackend.Data {
    interface IDS_Status {
        DO_Status[] GetAllStatus ();
    }
    class IDS_StatusImpl : IDS_Status {
        private static JsonData s_statusDatas;
        private DO_Status[] res;
        public IDS_StatusImpl() {
            string jsonFile = File.ReadAllText("D_Character.json");
            s_statusDatas = JsonMapper.ToObject(jsonFile);
            res = new DO_Status[s_statusDatas.Count];
            for(int i = 0; i < res.Length; i++) {
                DO_Status status = new DO_Status();
                status.m_statusId = short.Parse(s_statusDatas[i]["StatusID"].ToString());
                status.m_type = (StatusType)Enum.Parse(typeof(StatusType), s_statusDatas[i]["Type"].ToString());
                status.m_specialAttributeArr = new ActorUnitSpecialAttributeType[s_statusDatas[i]["SpecialAttributes"].Count];
                for(int m = 0; m < s_statusDatas[i]["SpecialAttributes"].Count; m++) {
                    status.m_specialAttributeArr[i] = (ActorUnitSpecialAttributeType)Enum.Parse
                        (typeof(ActorUnitSpecialAttributeType),s_statusDatas["SpecialAttributes"][m].ToString());
                }
                status.m_affectAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_statusDatas[i]["AttributeAttachArray"].Count];
                for(int m = 0; m < s_statusDatas[i]["AttributeAttachArray"].Count; m++) {
                    status.m_affectAttributeArr[m] = new ValueTuple<ActorUnitConcreteAttributeType,int>
                        ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType),s_statusDatas[i]["AttributeAttachArray"][m].ToString().Split(' ')[0]),
                        int.Parse(s_statusDatas[i]["AttributeAttachArray"].ToString().Split(' ')[1]));
                }
                res[i] = status;
            }



        }
        public DO_Status[] GetAllStatus() {
            return res;
        }
}