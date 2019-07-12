using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
namespace MirRemakeBackend.Data {
    interface IDS_Mission {
        DO_Mission[] GetAllMission ();
        ValueTuple<DO_MissionTargetKillMonsterData[], DO_MissionTargetGainItemData[], DO_MissionTargetLevelUpSkillData[], DO_MissionTargetTalkToNpcData[]> GetAllMissionDatas ();
    }
    class DS_MissionImpl : IDS_Mission {
        private JsonData m_missionDatas;
        private JsonData m_allData;
        private DO_MissionTargetKillMonsterData[] m_killMonster;
        private JsonData killMonsterData;
        private DO_MissionTargetGainItemData[] m_gainItem;
        private JsonData gainItemData;
        private DO_MissionTargetLevelUpSkillData[] m_levelUpSkill;
        private JsonData levelUpSkillData;
        private DO_MissionTargetTalkToNpcData[] m_talkToNPC;
        private JsonData talkToNPCData;
        public DO_Mission[] GetAllMission () {
            string jsonFile = File.ReadAllText ("Data/D_Mission.json");
            m_missionDatas = JsonMapper.ToObject (jsonFile);

            DO_Mission[] missionStructs = new DO_Mission[m_missionDatas.Count];
            for (int i = 0; i < m_missionDatas.Count; i++) {
                missionStructs[i] = getMissionDatas ((short) i);
            }
            return missionStructs;
        }
        private DO_Mission getMissionDatas (short ID) {
            DO_Mission mission = new DO_Mission ();
            mission.m_id = short.Parse (m_missionDatas[ID]["MissionID"].ToString ());
            mission.m_missionOccupation = (OccupationType) Enum.Parse (typeof (OccupationType), m_missionDatas[ID]["MissionOccupation"].ToString ());
            JsonData tempData;
            mission.m_acceptingNPCID = int.Parse (m_missionDatas[ID]["AcceptingNPCID"].ToString ());
            mission.m_deliveringNPCID = int.Parse (m_missionDatas[ID]["DeliveringNPCID"].ToString ());
            tempData = m_missionDatas[ID]["ConversationsWhenAccepting"];
            /**mission.m_conversationsWhenAccepting = new String[tempData.Count];
            //for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_conversationsWhenAccepting[i] = tempData[i].ToString();
            }
            tempData = m_missionDatas[ID]["ConversationsWhenDelivering"];
            mission.m_conversationWhenDelivering = new String[tempData.Count];
            for (int i = 0; i < tempData.Count; i++)
            {
                mission.m_conversationWhenDelivering[i] = tempData[i].ToString();
            }**/
            mission.m_bonusMoney = long.Parse (m_missionDatas[ID]["BonusMoney"].ToString ());
            mission.m_bonusExperience = int.Parse (m_missionDatas[ID]["BonusExperience"].ToString ());
            mission.m_levelInNeed = short.Parse (m_missionDatas[ID]["LevelInNeed"].ToString ());
            tempData = m_missionDatas[ID]["FatherMissionList"];
            mission.m_fatherMissionIdArr = new short[tempData.Count];

            for (int i = 0; i < tempData.Count; i++) {
                mission.m_fatherMissionIdArr[i] = (short.Parse (tempData[i].ToString ()));
            }
            tempData = m_missionDatas[ID]["ChildrenMissionList"];
            mission.m_childrenMissionArr = new short[tempData.Count];

            for (int i = 0; i < tempData.Count; i++) {
                mission.m_childrenMissionArr[i] = (short.Parse (tempData[i].ToString ()));
            }
            tempData = m_missionDatas[ID]["MissionTarget"];
            mission.m_missionTargetArr = new ValueTuple<MissionTargetType, short>[tempData.Count];
            for (int i = 0; i < tempData.Count; i++) {
                String tempstr1 = tempData[i][0].ToString ();
                MissionTargetType mt = (MissionTargetType) Enum.Parse (typeof (MissionTargetType), tempstr1);
                short parameter1 = short.Parse (tempData[i][1].ToString ());

                mission.m_missionTargetArr[i] = new ValueTuple<MissionTargetType, short> (mt, parameter1);

            }
            tempData = m_missionDatas[ID]["BonusItems"];
            mission.m_bonusItemIdAndNumArr = new ValueTuple<short, short>[tempData.Count];

            for (int i = 0; i < tempData.Count; i++) {
                mission.m_bonusItemIdAndNumArr[i] = (short.Parse (tempData[i].ToString ().Split (' ') [0]), short.Parse (tempData[i].ToString ().Split (' ') [1]));
            }
            return mission;
        }
        public ValueTuple<DO_MissionTargetKillMonsterData[], DO_MissionTargetGainItemData[], DO_MissionTargetLevelUpSkillData[], DO_MissionTargetTalkToNpcData[]> GetAllMissionDatas () {
            string jsonFile = File.ReadAllText ("Data/D_MissionTarget.json");
            m_allData = JsonMapper.ToObject (jsonFile);

            killMonsterData = m_allData["KILL_MONSTER"];
            gainItemData = m_allData["GAIN_ITEM"];
            levelUpSkillData = m_allData["LEVEL_UP_SKILL"];
            talkToNPCData = m_allData["TALK_TO_NPC"];

            m_killMonster = new DO_MissionTargetKillMonsterData[killMonsterData.Count];
            for (int i = 0; i < killMonsterData.Count; i++) {
                m_killMonster[i].m_id = short.Parse (killMonsterData[i]["ID"].ToString ());
                m_killMonster[i].m_targetMonsterId = short.Parse (killMonsterData[i]["MonsterID"].ToString ());
                m_killMonster[i].m_targetNum = short.Parse (killMonsterData[i]["Num"].ToString ());
            }

            m_gainItem = new DO_MissionTargetGainItemData[gainItemData.Count];
            for (int i = 0; i < gainItemData.Count; i++) {
                m_gainItem[i].m_id = short.Parse (gainItemData[i]["ID"].ToString ());
                m_gainItem[i].m_targetItemId = short.Parse (gainItemData[i]["ItemID"].ToString ());
                m_gainItem[i].m_targetNum = short.Parse (gainItemData[i]["Num"].ToString ());
            }

            m_levelUpSkill = new DO_MissionTargetLevelUpSkillData[levelUpSkillData.Count];
            for (int i = 0; i < levelUpSkillData.Count; i++) {
                m_levelUpSkill[i].m_id = short.Parse (levelUpSkillData[i]["ID"].ToString ());
                m_levelUpSkill[i].m_targetSkillId = short.Parse (levelUpSkillData[i]["SkillID"].ToString ());
                m_levelUpSkill[i].m_targetLevel = short.Parse (levelUpSkillData[i]["Level"].ToString ());
            }

            m_talkToNPC = new DO_MissionTargetTalkToNpcData[talkToNPCData.Count];
            for (int i = 0; i < talkToNPCData.Count; i++)
                m_talkToNPC[i].m_targetNpcId = short.Parse (talkToNPCData[i]["NPCID"].ToString ());
            ValueTuple<DO_MissionTargetKillMonsterData[], DO_MissionTargetGainItemData[], DO_MissionTargetLevelUpSkillData[], DO_MissionTargetTalkToNpcData[]> res =
                new ValueTuple<DO_MissionTargetKillMonsterData[], DO_MissionTargetGainItemData[], DO_MissionTargetLevelUpSkillData[], DO_MissionTargetTalkToNpcData[]>
                (m_killMonster, m_gainItem, m_levelUpSkill, m_talkToNPC);
            return res;
        }
    }
}