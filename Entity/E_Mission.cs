using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Mission {
        private DE_Mission m_dataEntity;
        public short m_MissionId { get { return m_dataEntity.m_id; } }
        public ValueTuple<int, int>[] m_missionTargetProgressArr;
        public IReadOnlyList<short> m_ChildrenIdList { get { return m_dataEntity.m_childrenIdList; } }
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
            m_missionTargetProgressArr = new ValueTuple<int, int>[de.m_targetAndParamList.Count];
            for (int i=0; i<de.m_targetAndParamList.Count; i++)
                m_missionTargetProgressArr[i] = new ValueTuple<int, int> (ddo.m_missionTargetProgressList[i], de.m_targetAndParamList[i].Item3);
        }
        public void Reset (DE_Mission de) {
            m_dataEntity = de;
            m_missionTargetProgressArr = new ValueTuple<int, int>[de.m_targetAndParamList.Count];
            for (int i=0; i<de.m_targetAndParamList.Count; i++)
                m_missionTargetProgressArr[i] = new ValueTuple<int, int> (0, de.m_targetAndParamList[i].Item3);
        }
    }
}