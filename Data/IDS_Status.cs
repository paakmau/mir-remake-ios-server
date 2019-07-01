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
        private static JsonData s_statusDatas;
        private DO_Status[] res;
        public DO_Status[] GetAllStatus () {
            string jsonFile = File.ReadAllText ("Data/D_Status.json");
            s_statusDatas = JsonMapper.ToObject (jsonFile);
            res = new DO_Status[s_statusDatas.Count];
            for (int i = 0; i < res.Length; i++) {
                DO_Status status = new DO_Status ();
                status.m_statusId = short.Parse (s_statusDatas[i]["StatusID"].ToString ());
                status.m_type = (StatusType) Enum.Parse (typeof (StatusType), s_statusDatas[i]["Type"].ToString ());
                status.m_specialAttributeArr = new ActorUnitSpecialAttributeType[s_statusDatas[i]["SpecialAttributes"].Count];
                for (int m = 0; m < s_statusDatas[i]["SpecialAttributes"].Count; m++) {
                    // Console.WriteLine(s_statusDatas[i]["SpecialAttributes"][m].ToString());
                    status.m_specialAttributeArr[m] = (ActorUnitSpecialAttributeType) Enum.Parse (typeof (ActorUnitSpecialAttributeType), s_statusDatas[i]["SpecialAttributes"][m].ToString ());
                }
                status.m_affectAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_statusDatas[i]["AttributeAttachArray"].Count];
                for (int m = 0; m < s_statusDatas[i]["AttributeAttachArray"].Count; m++) {
                    status.m_affectAttributeArr[m] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType) Enum.Parse (typeof (ActorUnitConcreteAttributeType), s_statusDatas[i]["AttributeAttachArray"][m].ToString ().Split (' ') [0]),
                            int.Parse (s_statusDatas[i]["AttributeAttachArray"][m].ToString ().Split (' ') [1]));
                }
                status.m_delta_HP_per_second = float.Parse (s_statusDatas[i]["DHP"].ToString ());
                status.m_delta_MP_per_second = float.Parse (s_statusDatas[i]["DMP"].ToString ());
                res[i] = status;
            }
            return res;
        }
    }
}