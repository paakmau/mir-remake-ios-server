using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_Mission {
        public DE_Mission m_dataEntity;
        public ValueTuple<int, int>[] m_missionTargetProgressArr;
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
            m_missionTargetProgressArr = new ValueTuple<int, int>[ddo.m_missionTargetProgressList.Count];
            for (int i=0; i<ddo.m_missionTargetProgressList.Count; i++)
                m_missionTargetProgressArr[i] = new ValueTuple<int, int> (ddo.m_missionTargetProgressList[i], de.m_targetAndParamList[i].Item3);
        }
    }
}