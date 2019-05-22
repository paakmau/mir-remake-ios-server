
using UnityEngine;
using System.Collections.Generic;
using System;


namespace MirRemakeBackend.Entity {
    class E_Mission {
        public short m_id;
        public string m_title;
        public short[] m_nextMissionIdArr;
        // 任务目标, 包括杀怪, 收集道具, 与Npc对话, 技能升级
        private IMissionTarget[] m_missionTargetArr;
        public bool m_IsFinished {
            get {
                foreach (var mt in m_missionTargetArr)
                    if (!mt.m_IsFinished)
                        return false;
                return true;
            }
        }
        // 完成任务可获得金钱
        private int m_bonusMoney;
        // 完成任务可获得经验
        private int m_bonusExperiences;
        // 完成任务可获得物品, <id, num>
        private List<KeyValuePair<short, short>> m_bonusItems;
    }
}