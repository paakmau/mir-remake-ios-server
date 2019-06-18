namespace MirRemakeBackend.Entity {
    enum GameLogType : byte {
        KILL_MONSTER,
        LEVEL_UP_SKILL,
        LEVEL_UP,
        TALK_TO_NPC,
        GAIN_ITEM
    }
    abstract class E_Log {
        public abstract GameLogType m_LogType { get; }
        public abstract void Reset (int parm1, int parm2, int parm3);
    }
    class E_KillMonsterLog : E_Log {
        public override GameLogType m_LogType { get { return GameLogType.KILL_MONSTER; } }
        public int m_killerNetId;
        public short m_monId;
        public override void Reset (int parm1, int parm2, int parm3) {
            m_killerNetId = parm1;
            m_monId = (short) parm2;
        }
    }
    class E_LevelUpSkillLog : E_Log {
        public override GameLogType m_LogType { get { return GameLogType.LEVEL_UP_SKILL; } }
        public int m_netId;
        public short m_skillId;
        public short m_skillLv;
        public override void Reset (int parm1, int parm2, int parm3) {
            m_netId = parm1;
            m_skillId = (short) parm2;
            m_skillLv = (short) parm3;
        }
    }
    class E_LevelUpLog : E_Log {
        public override GameLogType m_LogType { get { return GameLogType.LEVEL_UP; } }
        public int m_netId;
        public short m_lv;
        public override void Reset (int parm1, int parm2, int parm3) {
            m_netId = parm1;
            m_lv = (short) parm2;
        }
    }
    class E_TalkToNpcLog : E_Log {
        public override GameLogType m_LogType { get { return GameLogType.TALK_TO_NPC; } }
        public int m_netId;
        public short m_npcId;
        public short m_misId;
        public override void Reset (int parm1, int parm2, int parm3) {
            m_netId = parm1;
            m_npcId = (short) parm2;
            m_misId = (short) parm3;
        }
    }
    class E_GainItemLog : E_Log {
        public override GameLogType m_LogType { get { return GameLogType.GAIN_ITEM; } }
        public int m_netId;
        public short m_itemId;
        public short m_deltaNum;
        public override void Reset (int parm1, int parm2, int parm3) {
            m_netId = parm1;
            m_itemId = (short) parm2;
            m_deltaNum = (short) parm3;
        }
    }
}