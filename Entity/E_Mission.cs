using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Mission {
        private DE_Mission m_dataEntity;
        public short m_MissionId { get { return m_dataEntity.m_id; } }
        public (int, int) [] m_missionTargetProgressArr;
        public IReadOnlyList<short> m_ChildrenIdList { get { return m_dataEntity.m_childrenIdList; } }
        public long m_BonusVirtualCurrency { get { return m_dataEntity.m_bonusVirtualCurrency; } }
        public int m_BonusExperience { get { return m_dataEntity.m_bonusExperience; } }
        public IReadOnlyList < (short, short) > m_BonusItemIdAndNumList { get { return m_dataEntity.m_bonusItemIdAndNumList; } }
        public bool m_IsFinished {
            get {
                foreach (var mt in m_missionTargetProgressArr)
                    if (mt.Item1 != mt.Item2)
                        return false;
                return true;
            }
        }
        public void Reset (DE_Mission de, DDO_Mission ddo) {
            m_dataEntity = de;
            m_missionTargetProgressArr = new (int, int) [de.m_targetAndParamList.Count];
            for (int i = 0; i < de.m_targetAndParamList.Count; i++)
                m_missionTargetProgressArr[i] = (ddo.m_missionTargetProgressList[i], de.m_targetAndParamList[i].Item3);
        }
        public void Reset (DE_Mission de) {
            m_dataEntity = de;
            m_missionTargetProgressArr = new (int, int) [de.m_targetAndParamList.Count];
            for (int i = 0; i < de.m_targetAndParamList.Count; i++)
                m_missionTargetProgressArr[i] = (0, de.m_targetAndParamList[i].Item3);
        }
        public DDO_Mission GetDdo (int charId) {
            var progList = new List<int> (m_missionTargetProgressArr.Length);
            foreach (var prog in m_missionTargetProgressArr)
                progList.Add (prog.Item1);
            return new DDO_Mission (m_MissionId, charId, true, progList);
        }
    }
}