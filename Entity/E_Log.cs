namespace MirRemakeBackend.Entity {
    enum GameLogType : byte {
        MONSTER_KILL,
        SKILL_LEVEL_UP,
        LEVEL_UP,
        TALK_TO_NPC,
        GAIN_ITEM
    }
    abstract class E_Log {
        public abstract GameLogType m_LogType { get; }
        public abstract void Reset (int parm1, int parm2, int parm3);
    }
    class E_MonsterKillLog : E_Log {
        public override GameLogType m_LogType { get { return GameLogType.MONSTER_KILL; } }
        public int m_killerNetId;
        public short m_monId;
        public override void Reset (int parm1, int parm2, int parm3) {
            m_killerNetId = parm1;
            m_monId = (short) parm2;
        }
    }
}