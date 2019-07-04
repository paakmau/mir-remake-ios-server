using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EM_Status : EntityManagerBase {
        private class StatusFactory {
            #region StatusInitializer
            private interface IStatusInitializer {
                StatusType m_StatusType { get; }
                void ResetStatusData (DE_Status de, DEM_Status dem, E_Status resStatus);
            }
            private class SI_ChangeHp : IStatusInitializer {
                public StatusType m_StatusType { get { return StatusType.CHANGE_HP; } }
                public void ResetStatusData (DE_Status de, DEM_Status dem, E_Status resStatus) {
                    resStatus.Reset (de);
                }
            }
            private class SI_ChangeMp : IStatusInitializer {
                public StatusType m_StatusType { get { return StatusType.CHANGE_MP; } }
                public void ResetStatusData (DE_Status de, DEM_Status dem, E_Status resStatus) {
                    resStatus.Reset (de);
                }
            }
            private class SI_ConcreteAttribute : IStatusInitializer {
                public StatusType m_StatusType { get { return StatusType.CONCRETE_ATTRIBUTE; } }
                public void ResetStatusData (DE_Status de, DEM_Status dem, E_Status resStatus) {
                    resStatus.Reset (de);
                    ((E_ConcreteAttributeStatus) resStatus).ResetConAttrData (dem.GetConAttrStatusById (de.m_id));
                }
            }
            private class SI_SpecialAttribute : IStatusInitializer {
                public StatusType m_StatusType { get { return StatusType.SPECIAL_ATTRIBUTE; } }
                public void ResetStatusData (DE_Status de, DEM_Status dem, E_Status resStatus) {
                    resStatus.Reset (de);
                    ((E_SpecialAttributeStatus) resStatus).ResetSpAttrData (dem.GetSpAttrStatusById (de.m_id));
                }
            }
            #endregion
            private DEM_Status m_dem;
            private const int c_poolSize = 2000;
            private Dictionary<StatusType, ObjectPool> m_poolDict = new Dictionary<StatusType, ObjectPool> ();
            private Dictionary<StatusType, IStatusInitializer> m_initializerDict = new Dictionary<StatusType, IStatusInitializer> ();
            public StatusFactory (DEM_Status dem) {
                m_dem = dem;
                m_poolDict.Add (StatusType.CHANGE_HP, new ObjectPool<E_ChangeHpStatus> (c_poolSize));
                m_poolDict.Add (StatusType.CHANGE_MP, new ObjectPool<E_ChangeMpStatus> (c_poolSize));
                m_poolDict.Add (StatusType.CONCRETE_ATTRIBUTE, new ObjectPool<E_ConcreteAttributeStatus> (c_poolSize));
                m_poolDict.Add (StatusType.SPECIAL_ATTRIBUTE, new ObjectPool<E_SpecialAttributeStatus> (c_poolSize));

                // 实例化所有ClientCommand接口的实现类
                var type = typeof (IStatusInitializer);
                var implTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => p.IsClass && type.IsAssignableFrom (p));
                foreach (var implType in implTypes) {
                    IStatusInitializer impl = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IStatusInitializer;
                    m_initializerDict.Add (impl.m_StatusType, impl);
                }
            }
            public E_Status GetInstance (DE_Status de) {
                var res = m_poolDict[de.m_type].GetInstanceObj () as E_Status;
                m_initializerDict[de.m_type].ResetStatusData (de, m_dem, res);
                return res;
            }
            public void RecycleInstance (E_Status obj) {
                m_poolDict[obj.m_Type].RecycleInstance (obj);
            }
        }
        public static EM_Status s_instance;
        private DEM_Status m_dem;
        private StatusFactory m_fact;
        private Dictionary<int, List<E_Status>> m_statusListDict = new Dictionary<int, List<E_Status>> ();
        public EM_Status (DEM_Status dem) {
            m_dem = dem;
            m_fact = new StatusFactory (dem);
        }
        public void InitCharacterStatus (int netId) {
            m_statusListDict.TryAdd (netId, new List<E_Status> ());
        }
        public void InitAllMonster (int[] netIdArr) {
            foreach (var item in netIdArr)
                m_statusListDict.TryAdd (item, new List<E_Status> ());
        }
        public void RemoveCharacterStatus (int netId) {
            List<E_Status> statusList = null;
            m_statusListDict.TryGetValue (netId, out statusList);
            if (statusList == null) return;
            m_statusListDict.Remove (netId);
            // TODO: Status 回收实例
            // for (int i=0; i<statusList.Count; i++)
            //     s_entityPool.m_statusPool.RecycleInstance (statusList[i]);
        }
        public List<E_Status> AttachStatus (int netId, int casterNetId, (short, float, float) [] statusIdAndValueAndTimeArr) {
            // TODO: Status EM Attach
            // List<E_Status> oriStatusList = null;
            // if (!m_statusListDict.TryGetValue (netId, out oriStatusList))
            //     return null;
            // var res = new List<E_Status> ();
            // foreach (var statusInfo in statusIdAndValueAndTimeArr) {
            //     var de = m_dem.GetStatusById (statusInfo.Item1);
            //     var statusObj = s_entityPool.m_statusPool.GetInstance ();
            //     statusObj.Reset (de, statusInfo.Item1, statusInfo.Item2, statusInfo.Item3, casterNetId);
            //     oriStatusList.Add (statusObj);
            //     res.Add (statusObj);
            // }
            // return res;
            return null;
        }
        public void RemoveOrderedStatus (int netId, List<int> orderedIndexList) {
            // TODO: Status EM Remove
            // List<E_Status> statusList = null;
            // m_statusListDict.TryGetValue (netId, out statusList);
            // if (statusList == null)
            //     return;
            // for (int i = orderedIndexList.Count - 1; i >= 0; i--) {
            //     E_Status obj = statusList[orderedIndexList[i]];
            //     s_entityPool.m_statusPool.RecycleInstance (obj);
            //     statusList.RemoveAt (orderedIndexList[i]);
            // }
        }
        public Dictionary<int, List<E_Status>>.Enumerator GetStatusEn () {
            return m_statusListDict.GetEnumerator ();
        }
    }
}