using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.Entity {
    interface IMissionTarget {
        MissionTargetType m_Type { get; }
        bool m_IsFinish { get; }
        int m_Progress { get; set; }
    }
    class E_MissionTargetTalkToNpc : IMissionTarget {
        public MissionTargetType m_Type { get { return MissionTargetType.TALK_TO_NPC; } }
        public bool m_IsFinish { get { return m_isTalk; } }
        public int m_Progress { get { return m_isTalk ? 1 : 0; } set { m_isTalk = value != 0; } }
        private bool m_isTalk;
        public short m_tarId;
        public void Reset (short tarId) {
            m_isTalk = false;
            m_tarId = tarId;
        }
    }
    class E_MissionTargetKillMonster : IMissionTarget {
        public MissionTargetType m_Type { get { return MissionTargetType.KILL_MONSTER; } }
        public bool m_IsFinish { get { return m_num >= m_TargetNum; } }
        public int m_Progress { get { return m_num; } set { m_num = value; } }
        private DE_MissionTargetKillMonster m_de;
        public short m_MonId { get { return m_de.m_monsterId; } }
        public int m_TargetNum { get { return m_de.m_targetNum; } }
        private int m_num;
        public void Reset (DE_MissionTargetKillMonster de, int num) {
            m_de = de;
            m_num = num;
        }
    }
    class E_MissionTargetGainItem : IMissionTarget {
        public MissionTargetType m_Type { get { return MissionTargetType.GAIN_ITEM; } }
        public bool m_IsFinish { get { return m_num >= m_TargetNum; } }
        public int m_Progress { get { return m_num; } set { m_num = value; } }
        private DE_MissionTargetGainItem m_de;
        private int m_TargetNum { get { return m_de.m_targetNum; } }
        public short m_ItemId { get { return m_de.m_itemId; } }
        private int m_num;
        public void Reset (DE_MissionTargetGainItem de, int num) {
            m_de = de;
            m_num = num;
        }
    }
    class E_MissionTargetLevelUpSkill : IMissionTarget {
        public MissionTargetType m_Type { get { return MissionTargetType.LEVEL_UP_SKILL; } }
        public bool m_IsFinish { get { return m_lv >= m_TargetLv; } }
        public int m_Progress { get { return m_lv; } set { m_lv = value; } }
        private DE_MissionTargetLevelUpSkill m_de;
        private int m_TargetLv { get { return m_de.m_targetLv; } }
        public short m_SkillId { get { return m_de.m_skillId; } }
        private int m_lv;
        public void Reset (DE_MissionTargetLevelUpSkill de, int lv) {
            m_de = de;
            m_lv = lv;
        }
    }
    class E_Mission {
        private DE_Mission m_de;
        public List<IMissionTarget> m_tarList = new List<IMissionTarget> ();
        public short m_MissionId { get { return m_de.m_id; } }
        public IReadOnlyList<short> m_ChildrenIdList { get { return m_de.m_childrenIdList; } }
        public long m_BonusVirtualCurrency { get { return m_de.m_bonusVirtualCurrency; } }
        public int m_BonusExperience { get { return m_de.m_bonusExperience; } }
        public IReadOnlyList < (short, short) > m_BonusItemIdAndNumList { get { return m_de.m_bonusItemIdAndNumList; } }
        public bool m_IsFinished {
            get {
                for (int i = 0; i < m_tarList.Count; i++)
                    if (!m_tarList[i].m_IsFinish)
                        return false;
                return true;
            }
        }
        public void Reset (DE_Mission de, List<IMissionTarget> tarList) {
            m_de = de;
            m_tarList = tarList;
        }
        public DDO_Mission GetDdo (int charId, MissionStatus status) {
            var progList = new List<int> (m_tarList.Count);
            for (int i = 0; i < m_tarList.Count; i++)
                progList.Add (m_tarList[i].m_Progress);
            return new DDO_Mission (m_MissionId, charId, status, progList);
        }
        public NO_Mission GetNo () {
            var misProgressList = new List<int> (m_tarList.Count);
            for (int i = 0; i < m_tarList.Count; i++)
                misProgressList.Add (m_tarList[i].m_Progress);
            return new NO_Mission (m_MissionId, misProgressList);
        }
    }

}