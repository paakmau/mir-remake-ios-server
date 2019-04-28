using System.Collections.Generic;


namespace MirRemake {
    class E_Mission {
        private short m_id;
        public short m_Id { get { return m_id; } }
        private byte m_type;// TODO:

        private string m_title;

        private string m_details;
        
        // 接受任务时与NPC的对话
        private string[] m_conversationsWhenAccepting;
        public string[] m_ConversationsWhenAccepting {get {return m_conversationsWhenAccepting;}}

        // 交付任务时与NPC的对话
        private string[] m_conversationsWhenDelivering;
        public string[] m_ConversationsWhenDelivering {get {return m_conversationsWhenDelivering;}}
        private E_NPC m_acceptingNPC;
        private E_NPC m_deliveringNPC;

        // 接受此任务需要等级
        private short m_levelInNeed;
        public short m_LevelInNeed {get {return this.m_levelInNeed;}}

        // 父任务，接受此任务的必要条件：父任务已完成
        private short[] m_fatherMissionId;
        public short[] m_FatherMissionId {get {return this.m_fatherMissionId;}}
        
        // 子任务，完成此任务后解锁的任务
        private List<E_Mission> m_childMissions;
        public List<E_Mission> m_ChildMissions {get {return this.m_childMissions;}}
        
        // 完成任务所需物品
        private Dictionary<short, short> m_necessaryItemsToFinishingThisMission;
        public Dictionary<short, short> m_NecessaryItemsToFinishingThisMission {get {return m_necessaryItemsToFinishingThisMission;}}
        
        // 以获得的任务物品
        private Dictionary<short, short> m_numOfNecessaryItemsToFinishingThisMission;
        public Dictionary<short, short> m_NumOfNecessaryItemsToFinishingThisMission {get {return m_numOfNecessaryItemsToFinishingThisMission;}}
        
        // 完成任务所需怪物击杀
        private Dictionary<short, short> m_necessaryKillsToFinishingThisMission;
        public Dictionary<short, short> m_NecessaryKillsToFinishingThisMission {get {return m_necessaryKillsToFinishingThisMission;}}
        
        // 已击杀怪物
        private Dictionary<short, short> m_numOfNecessaryKillsToFinishingThisMission;
        public Dictionary<short, short> m_NumOfNecessaryKillsToFinishingThisMission {get {return m_numOfNecessaryKillsToFinishingThisMission;}}

        // 网络id
        private int m_networkId;
        public int m_NetworkId { get{ return this.m_networkId;} set{} }

        // 完成任务可获得金钱
        private int m_bonusMoney;
        public int m_BonusMoney { get { return m_bonusMoney;}}

        // 完成任务可获得经验
        private int m_bonusExperiences;
        public int m_BonusExperiences { get { return m_bonusExperiences;}}

        // 完成任务可获得物品, <id, num>
        private Dictionary<short, short> m_bonusItems;
        public Dictionary<short, short> m_BonusItems { get { return m_bonusItems;}}


        /// <summary>
        /// 完成任务
        /// </summary>
        /// <returns>是否达成完成条件，若以达成，则取消该任务对MassagManage的所有监听，并返回true；若未达成，直接返回false</returns>
        public bool isCompleted() {
            foreach(KeyValuePair<short, short> n_item in m_NecessaryItemsToFinishingThisMission) {
                if(m_numOfNecessaryItemsToFinishingThisMission.ContainsKey(n_item.Key)) {
                    if(m_numOfNecessaryItemsToFinishingThisMission[n_item.Key] < n_item.Value) {
                        return false;
                    }
                    continue;
                }
                return false;
            }
            foreach(KeyValuePair<short, short> n_kill in m_necessaryKillsToFinishingThisMission) {
                if(m_numOfNecessaryKillsToFinishingThisMission.ContainsKey(n_kill.Key)) {
                    if(m_numOfNecessaryKillsToFinishingThisMission[n_kill.Key] < n_kill.Value) {
                        return false;
                    }
                    continue;
                }
                return false;
            }

            return true;
        }
    }
}