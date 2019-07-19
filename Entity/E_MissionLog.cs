namespace MirRemakeBackend.Entity {
    enum MissionLogType : byte {
        KILL_MONSTER,
        LEVEL_UP_SKILL,
        LEVEL_UP,
        TALK_TO_NPC,
        GAIN_ITEM
    }
    abstract class E_MissionLog {
        public int m_netId;
        public abstract MissionLogType m_LogType { get; }
        public void Reset (int netId) { m_netId = netId; }
        public abstract void Reset (int netId, int parm1, int parm2, int parm3);
    }
    class E_KillMonsterLog : E_MissionLog {
        public override MissionLogType m_LogType { get { return MissionLogType.KILL_MONSTER; } }
        public short m_monId;
        public override void Reset (int netId, int parm1, int parm2, int parm3) {
            base.Reset (netId);
            m_monId = (short) parm1;
        }
    }
    class E_LevelUpSkillLog : E_MissionLog {
        public override MissionLogType m_LogType { get { return MissionLogType.LEVEL_UP_SKILL; } }
        public short m_skillId;
        public short m_skillLv;
        public override void Reset (int netId, int parm1, int parm2, int parm3) {
            base.Reset (netId);
            m_skillId = (short) parm1;
            m_skillLv = (short) parm2;
        }
    }
    class E_LevelUpLog : E_MissionLog {
        public override MissionLogType m_LogType { get { return MissionLogType.LEVEL_UP; } }
        public short m_lv;
        public override void Reset (int netId, int parm1, int parm2, int parm3) {
            base.Reset (netId);
            m_lv = (short) parm1;
        }
    }
    class E_TalkToNpcLog : E_MissionLog {
        public override MissionLogType m_LogType { get { return MissionLogType.TALK_TO_NPC; } }
        public short m_misId;
        public short m_misTarId;
        public override void Reset (int netId, int parm1, int parm2, int parm3) {
            base.Reset (netId);
            m_misId = (short) parm1;
            m_misTarId = (short) parm2;
        }
    }
    class E_GainItemLog : E_MissionLog {
        public override MissionLogType m_LogType { get { return MissionLogType.GAIN_ITEM; } }
        public short m_itemId;
        public short m_deltaNum;
        public override void Reset (int netId, int parm1, int parm2, int parm3) {
            base.Reset (netId);
            m_itemId = (short) parm1;
            m_deltaNum = (short) parm2;
        }
    }
}