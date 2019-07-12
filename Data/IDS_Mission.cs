using System.Collections.Generic;
using System.IO;
using LitJson;
using System;
namespace MirRemakeBackend.Data {
    interface IDS_Mission {
        DO_Mission[] GetAllMission ();
        ValueTuple<DO_MissionTargetKillMonsterData[],DO_MissionTargetGainItemData[],DO_MissionTargetLevelUpSkillData[],DO_MissionTargetTalkToNpcData[]> GetAllMissionDatas();
    }
    class DS_MissionImpl : IDS_Mission
    {
        private JsonData s_missionDatas;
        private JsonData s_allData;
        private DO_MissionTargetKillMonsterData[] s_killMonster;
        private JsonData killMonsterData;
        private DO_MissionTargetGainItemData[] s_gainItem;
        private JsonData gainItemData;
        private DO_MissionTargetLevelUpSkillData[] s_levelUpSkill;
        private JsonData levelUpSkillData;
        private DO_MissionTargetTalkToNpcData[] s_talkToNPC;
        private JsonData talkToNPCData;
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
            mission.m_bonusMoney = long.Parse(s_missionDatas[ID]["BonusMoney"].ToString());
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
            mission.m_missionTargetArr = new ValueTuple<MissionTargetType, short>[tempData.Count];
            for (int i = 0; i < tempData.Count; i++)
            {
                String tempstr1 = tempData[i][0].ToString();
                MissionTargetType mt = (MissionTargetType)Enum.Parse(typeof(MissionTargetType), tempstr1);
                short parameter1 = short.Parse(tempData[i][1].ToString());

                mission.m_missionTargetArr[i] = new ValueTuple<MissionTargetType, short>(mt, parameter1);


            }
            tempData = s_missionDatas[ID]["BonusItems"];
            mission.m_bonusItemIdAndNumArr = new ValueTuple<short,short>[tempData.Count];
            
            for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_bonusItemIdAndNumArr[i]=(short.Parse(tempData[i].ToString().Split(' ')[0]), short.Parse(tempData[i].ToString().Split(' ')[1]));
            }
            return mission;
        }
        public DO_MissionTargetArrangement GetAllMissionDatas(){
            string jsonFile = File.ReadAllText("Data/D_MissionTarget.json");
            s_allData = JsonMapper.ToObject(jsonFile);

            killMonsterData=s_allData["KILL_MONSTER"];
            gainItemData=s_allData["GAIN_ITEM"];
            levelUpSkillData=s_allData["LEVEL_UP_SKILL"];
            talkToNPCData=s_allData["TALK_TO_NPC"];

            s_killMonster=new DO_MissionTargetKillMonsterData[killMonsterData.Count];
            for(int i=0;i<killMonsterData.Count;i++){
                s_killMonster[i].m_id=short.Parse(killMonsterData[i]["ID"].ToString());
                s_killMonster[i].m_targetMonsterId=short.Parse(killMonsterData[i]["MonsterID"].ToString());
                s_killMonster[i].m_targetNum=short.Parse(killMonsterData[i]["Num"].ToString());
            }
            
            s_gainItem=new DO_MissionTargetGainItemData[gainItemData.Count];
            for(int i=0;i<gainItemData.Count;i++){
                s_gainItem[i].m_id=short.Parse(gainItemData[i]["ID"].ToString());
                s_gainItem[i].m_targetItemId=short.Parse(gainItemData[i]["ItemID"].ToString());
                s_gainItem[i].m_targetNum=short.Parse(gainItemData[i]["Num"].ToString());
            }

            s_levelUpSkill=new DO_MissionTargetLevelUpSkillData[levelUpSkillData.Count];
            for(int i=0;i<levelUpSkillData.Count;i++){
                s_levelUpSkill[i].m_id=short.Parse(levelUpSkillData[i]["ID"].ToString());
                s_levelUpSkill[i].m_targetSkillId=short.Parse(levelUpSkillData[i]["SkillID"].ToString());
                s_levelUpSkill[i].m_targetLevel=short.Parse(levelUpSkillData[i]["Level"].ToString());
            }

            s_talkToNPC=new DO_MissionTargetTalkToNpcData[talkToNPCData.Count];
            for(int i=0;i<talkToNPCData.Count;i++){
                s_talkToNPC[i].m_targetNpcId=short.Parse(talkToNPCData[i]["NPCID"].ToString());
                s_talkToNPC[i].m_conversation=new string[talkToNPCData[i]["Conversation"].Count];
                for(int j=0;j<talkToNPCData[i]["Conversation"].Count;j++){
                    s_talkToNPC[i].m_conversation[i]=talkToNPCData[i]["Conversation"][j].ToString();
                }
            }
            ValueTuple<DO_MissionTargetKillMonsterData[],DO_MissionTargetGainItemData[],DO_MissionTargetLevelUpSkillData[],DO_MissionTargetTalkToNpcData[]> res=
                new ValueTuple<DO_MissionTargetKillMonsterData[],DO_MissionTargetGainItemData[],DO_MissionTargetLevelUpSkillData[],DO_MissionTargetTalkToNpcData[]>
                (s_killMonster,s_gainItem,s_levelUpSkill,s_talkToNPC);
            return res;
        }
    }
}