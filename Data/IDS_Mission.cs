using System.Collections.Generic;
using System.IO;
using LitJson;
using System;
namespace MirRemakeBackend.Data {
    interface IDS_Mission {
        DO_Mission[] GetAllMission ();
    }
    class DS_MissionImpl : IDS_Mission
    {
        private JsonData s_missionDatas;
        public DO_Mission[] GetAllMission()
        {
            string jsonFile = File.ReadAllText("Data/D_Mission.json");
            s_missionDatas = JsonMapper.ToObject(jsonFile);

            DO_Mission[] missionStructs = new DO_Mission[s_missionDatas.Count];
            for (int i = 0; i < s_missionDatas.Count; i++)
            {
                missionStructs[i] = getMissionDatas((short)i);
            }
            return missionStructs;
        }
        private DO_Mission getMissionDatas(short ID)
        {
            DO_Mission mission = new DO_Mission();
            mission.m_id = short.Parse(s_missionDatas[ID]["MissionID"].ToString());
            mission.m_missionOccupation = (OccupationType)Enum.Parse(typeof(OccupationType), s_missionDatas[ID]["MissionOccupation"].ToString());
            JsonData tempData;
            mission.m_acceptingNPCID = int.Parse(s_missionDatas[ID]["AcceptingNPCID"].ToString());
            mission.m_deliveringNPCID = int.Parse(s_missionDatas[ID]["DeliveringNPCID"].ToString());
            tempData = s_missionDatas[ID]["ConversationsWhenAccepting"];
            /**mission.m_conversationsWhenAccepting = new String[tempData.Count];
            //for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_conversationsWhenAccepting[i] = tempData[i].ToString();
            }
            tempData = s_missionDatas[ID]["ConversationsWhenDelivering"];
            mission.m_conversationWhenDelivering = new String[tempData.Count];
            for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_conversationWhenDelivering[i] = tempData[i].ToString();
            }**/
            mission.m_bonusCoin = int.Parse(s_missionDatas[ID]["BonusMoney"].ToString());
            mission.m_bonusExperience = int.Parse(s_missionDatas[ID]["BonusExperience"].ToString());
            mission.m_levelInNeed = short.Parse(s_missionDatas[ID]["LevelInNeed"].ToString());
            tempData = s_missionDatas[ID]["FatherMissionList"];
            mission.m_fatherMissionIdArr = new short[tempData.Count];
            
            for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_fatherMissionIdArr[i]=(short.Parse(tempData[i].ToString()));
            }
            tempData = s_missionDatas[ID]["ChildrenMissionList"];
            mission.m_childrenMissionArr = new short[tempData.Count];
            
            for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_childrenMissionArr[i] = (short.Parse(tempData[i].ToString()));
            }
            tempData = s_missionDatas[ID]["MissionTarget"];
            mission.m_missionTargetArr = new ValueTuple<MissionTargetType, short, int>[tempData.Count];
            for (int i = 0; i < tempData.Count; i++)
            {
                String tempstr1 = tempData[i][0].ToString();
                MissionTargetType mt = (MissionTargetType)Enum.Parse(typeof(MissionTargetType), tempstr1);
                short parameter1 = short.Parse(tempData[i][1].ToString().Split(' ')[0]);
                int parameter2 = int.Parse(tempData[i][1].ToString().Split(' ')[1]);

                mission.m_missionTargetArr[i] = new ValueTuple<MissionTargetType, short, int>(mt, parameter1, parameter2);


            }
            tempData = s_missionDatas[ID]["BonusItems"];
            mission.m_bonusItemIdAndNumArr = new ValueTuple<short,short>[tempData.Count];
            
            for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_bonusItemIdAndNumArr[i]=(short.Parse(tempData[i].ToString().Split(' ')[0]), short.Parse(tempData[i].ToString().Split(' ')[1]));
            }
            return mission;
        }
    }
}