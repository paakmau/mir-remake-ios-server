using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Mission {
        private DE_Mission m_dataEntity;
        public (int, int) [] m_misTargetProgressArr;
        public IReadOnlyList < (MissionTargetType, short, int) > m_MisTarget { get { return m_dataEntity.m_targetAndParamList; } }
        public short m_MissionId { get { return m_dataEntity.m_id; } }
        public IReadOnlyList<short> m_ChildrenIdList { get { return m_dataEntity.m_childrenIdList; } }
        public long m_BonusVirtualCurrency { get { return m_dataEntity.m_bonusVirtualCurrency; } }
        public int m_BonusExperience { get { return m_dataEntity.m_bonusExperience; } }
        public IReadOnlyList < (short, short) > m_BonusItemIdAndNumList { get { return m_dataEntity.m_bonusItemIdAndNumList; } }
        public bool m_IsFinished {
            get {
                foreach (var mt in m_misTargetProgressArr)
                    if (mt.Item1 != mt.Item2)
                        return false;
                return true;
            }
        }
        public void Reset (DE_Mission de, DDO_Mission ddo) {
            m_dataEntity = de;
            m_misTargetProgressArr = new (int, int) [de.m_targetAndParamList.Count];
            for (int i = 0; i < de.m_targetAndParamList.Count; i++)
                m_misTargetProgressArr[i] = (ddo.m_missionTargetProgressList[i], de.m_targetAndParamList[i].Item3);
        }
        public void Reset (DE_Mission de) {
            m_dataEntity = de;
            m_misTargetProgressArr = new (int, int) [de.m_targetAndParamList.Count];
            for (int i = 0; i < de.m_targetAndParamList.Count; i++)
                m_misTargetProgressArr[i] = (0, de.m_targetAndParamList[i].Item3);
        }
        public DDO_Mission GetDdo (int charId, MissionStatus status) {
            var progList = new List<int> (m_misTargetProgressArr.Length);
            foreach (var prog in m_misTargetProgressArr)
                progList.Add (prog.Item1);
            return new DDO_Mission (m_MissionId, charId, status, progList);
        }
    }
}