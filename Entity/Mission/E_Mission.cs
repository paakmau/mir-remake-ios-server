/// <summary>
/// 任务实体
/// 创建者 fn
/// 创建时间 2019/4/11
/// 最后修改者 fn
/// 最后修改时间 2019/4/11
/// </summary>

using UnityEngine;
using System.Collections.Generic;
using System;


namespace MirRemake {
    class E_Mission {
        private short m_id;
        private byte m_type;// TODO:
        private string m_title;
        public string m_Title { get { return m_title; } }
        private string m_details;
        // 接受任务时与NPC的对话
        private string[] m_conversationsWhenAccepting;
        // 交付任务时与NPC的对话
        private string[] m_conversationsWhenDelivering;
        private short m_acceptingNpcId;
        private short m_deliveringNpcId;
        private short m_requiredLevelToAccept;
        private short[] m_nextMissionIdArr;
        public short[] m_NextMissionIdArr { get { return m_nextMissionIdArr; } }
        // 任务目标, 包括杀怪, 收集道具, 与Npc对话, 技能升级
        // private IMissionTarget[] m_missionTargetArr;
        // public bool m_IsFinished {
        //     get {
        //         foreach (var mt in m_missionTargetArr)
        //             if (!mt.m_IsFinished)
        //                 return false;
        //         return true;
        //     }
        // }
        // 完成任务可获得金钱
        private int m_bonusMoney;
        // 完成任务可获得经验
        private int m_bonusExperiences;
        // 完成任务可获得物品, <id, num>
        private List<KeyValuePair<short, short>> m_bonusItems;
        public E_Mission(short missionId) {
            // TODO:database
            switch(missionId) {
                case 1:
                    this.m_id = 1;
                    this.m_title = "乘胜追鸡";
                    this.m_details = "成圣追鸡";
                    this.m_deliveringNpcId = 1;
                    break;
                case 2:
                    this.m_id = 2;
                    this.m_title = "刺杀首领";
                    this.m_details = "murder 67";
                    this.m_deliveringNpcId = 2;
                    break;
                case 3:
                    this.m_id = 3;
                    this.m_title = "逮捕复读机";
                    this.m_details = "arrest repeater arrest repeater arrest repeater arrest repeater";
                    this.m_deliveringNpcId = 1;
                    break;
            }
        }
        public void NetworkAcceptAndChangeNpc () {
            // TODO: 修改Npc的Action
            // SM_Npc.s_instance.NotifyAddAction (npcId, new NA_MissionTalk (missionId, null));
        }
        public void NetworkCancelAndChangeNpc () {
            // TODO: 重置进度并修改Npc的Action
            // SM_Npc.s_instance.NotifyRemoveAction (npcId, new NA_MissionTalk (missionId, null));
        }
        public void NetworkDeliverAndChangeNpc () {
            // TODO: 修改Npc的Action
            // SM_Npc.s_instance.NotifyRemoveAction (npcId, new NA_MissionTalk (missionId, null));
        }
        public void NetworkSetMissionProgress (byte targetNum, short value) {
            // m_missionTargetArr[targetNum].m_MissionProgressValue = value;
        }
        /// <summary>
        /// 根据击杀的怪物更新任务进度  
        /// 返回怪物是否影响任务进度
        /// </summary>
        /// <param name="monsterId">击杀的怪物Id</param>
        /// <returns>是否影响任务进度</returns>
        // public bool IsProgressChangedAfterNetworkKillMonster (short monsterId) {
        //     foreach (var target in m_missionTargetArr) {
        //         if (target.m_MissionTarget == MissionTargetType.KILL_MONSTER) {
        //             if (((MT_KillMonster)target).IsProgressChangedAfterKillMonster (monsterId))
        //                 return true;
        //         }
        //     }
        //     return false;
        // }
        /// <summary>
        /// 根据获得或失去的物品判断是否影响任务进度
        /// </summary>
        /// <param name="itemId">获得或失去的物品</param>
        /// <returns>是否影响任务进度</returns>
        // public bool IsProgressChangedAfterNetworkGainOrLoseItem (short itemId) {
        //     foreach (var target in m_missionTargetArr) {
        //         if (target.m_MissionTarget == MissionTargetType.GAIN_ITEM) {
        //             if (((MT_GainItem)target).IsProgressChangedAfterGainOrLostItem (itemId))
        //                 return true;
        //         }
        //     }
        //     return false;
        // }
        /// <summary>
        /// 根据Npc的任务对话更新任务进度  
        /// 返回是否影响任务进度
        /// </summary>
        /// <param name="npcId">NpcId</param>
        /// <returns>是否影响任务进度</returns>
        // public bool IsProgressChangedAfterNetworkTalkToMissionNpc (short npcId) {
        //     foreach (var target in m_missionTargetArr) {
        //         if (target.m_MissionTarget == MissionTargetType.TALK_TO_NPC) {
        //             if (((MT_TalkToNpc)target).IsProgressChangedAfterTalkToNpc (npcId))
        //                 return true;
        //         }
        //     }
        //     return false;
        // }
        // public bool IsProgressChangedAfterNetworkSkillLevelUp (short skillId) {
        //     foreach (var target in m_missionTargetArr) {
        //         if (target.m_MissionTarget == MissionTargetType.LEVEL_UP_SKILL) {
        //             if (((MT_LevelUpSkill)target).IsProgressChangedAfterSkillLevelUp (skillId))
        //                 return true;
        //         }
        //     }
        //     return false;
        // }
    }
}