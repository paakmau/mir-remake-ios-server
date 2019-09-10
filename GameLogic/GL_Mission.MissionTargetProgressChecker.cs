using System;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Mission {
        private interface IMissionTargetProgressChecker {
            MissionTargetType m_Type { get; }
            int GetNewProgress (int misId, IMissionTarget itarget, int curProgs, E_MissionLog logBase);
        }
        private class MTPC_KillMonster : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.KILL_MONSTER; } }
            public int GetNewProgress (int misId, IMissionTarget itarget, int curProgs, E_MissionLog logBase) {
                var target = itarget as E_MissionTargetKillMonster;
                var log = logBase as E_KillMonsterLog;
                if (target.m_MonId != log.m_monId)
                    return curProgs;
                return Math.Min (target.m_TargetNum, curProgs + 1);
            }
        }
        private class MTPC_LevelUpSkill : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.LEVEL_UP_SKILL; } }
            public int GetNewProgress (int misId, IMissionTarget itarget, int curProgs, E_MissionLog logBase) {
                var target = itarget as E_MissionTargetLevelUpSkill;
                var log = logBase as E_LevelUpSkillLog;
                if (target.m_SkillId != log.m_skillId)
                    return curProgs;
                return log.m_skillLv;
            }
        }
        private class MTPC_GainItem : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.GAIN_ITEM; } }
            public int GetNewProgress (int misId, IMissionTarget itarget, int curProgs, E_MissionLog logBase) {
                var target = itarget as E_MissionTargetGainItem;
                var log = logBase as E_GainItemLog;
                if (target.m_ItemId != log.m_itemId)
                    return curProgs;
                return curProgs + log.m_deltaNum;
            }
        }

        private class MTPC_TalkToNpc : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.TALK_TO_NPC; } }
            public int GetNewProgress (int misId, IMissionTarget itarget, int curProgs, E_MissionLog logBase) {
                var target = itarget as E_MissionTargetTalkToNpc;
                var log = logBase as E_TalkToNpcLog;
                if (misId != log.m_misId)
                    return curProgs;
                if (target.m_tarId != log.m_misTarId)
                    return curProgs;
                return 1;
            }
        }

        private class MTPC_ChargeAdequately : IMissionTargetProgressChecker {
            public MissionTargetType m_Type { get { return MissionTargetType.CHARGE_ADEQUATELY; } }
            public int GetNewProgress (int misId, IMissionTarget itarget, int curProgs, E_MissionLog logBase) {
                var target = itarget as E_MissionTargetChargeAdequately;
                var log = logBase as E_ChargeAdequatelyLog;
                return target.m_Progress + log.m_totalChargedMoney;
            }
        }

    }
}